using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Protogame
{
    public class CompiledAssetFs : ICompiledAssetFs, IDisposable
    {
        private readonly IAssetCompiler[] _compilers;
        private readonly IAssetFs _assetFs;
        private readonly Dictionary<string, IAssetFsFile> _compiledOverlay;
        private readonly Dictionary<string, SemaphoreSlim> _compilerLocks;
        private readonly SemaphoreSlim _compilerLock;
        private readonly TargetPlatform _targetPlatform;
        private readonly HashSet<Func<string, Task>> _onAssetUpdated;
        private Task _updatingTask;

        protected CompiledAssetFs(
            IAssetFs assetFs,
            IAssetCompiler[] compilers,
            TargetPlatform targetPlatform)
        {
            _assetFs = assetFs;
            _compilers = compilers;
            _compiledOverlay = new Dictionary<string, IAssetFsFile>();
            _compilerLocks = new Dictionary<string, SemaphoreSlim>();
            _compilerLock = new SemaphoreSlim(1);
            _targetPlatform = targetPlatform;
            _onAssetUpdated = new HashSet<Func<string, Task>>();

            _assetFs.RegisterUpdateNotifier(OnAssetUpdated);
        }

        private async Task OnAssetUpdated(string assetName)
        {
            // TODO: Detect and update known dependencies as well.

            foreach (var handler in _onAssetUpdated)
            {
                if (_compiledOverlay.ContainsKey(assetName))
                {
                    _compiledOverlay.Remove(assetName);
                }

                await handler(assetName);
            }
        }

        public void RegisterUpdateNotifier(Func<string, Task> onAssetUpdated)
        {
            if (!_onAssetUpdated.Contains(onAssetUpdated))
            {
                _onAssetUpdated.Add(onAssetUpdated);
            }
        }

        public void UnregisterUpdateNotifier(Func<string, Task> onAssetUpdated)
        {
            if (_onAssetUpdated.Contains(onAssetUpdated))
            {
                _onAssetUpdated.Remove(onAssetUpdated);
            }
        }

        protected virtual void OnCompileStart(IAssetFsFile assetFile, TargetPlatform targetPlatform)
        {
        }

        protected virtual void OnCompilerMissing(IAssetFsFile assetFile, TargetPlatform targetPlatform)
        {
        }

        protected virtual void OnCompileFinish(IAssetFsFile assetFile, IAssetFsFile compiledAssetFile, TargetPlatform targetPlatform)
        {
        }

        public async Task<IAssetFsFile> Get(string name)
        {
            return await EnsureCompiled(await _assetFs.Get(name).ConfigureAwait(false)).ConfigureAwait(false);
        }

        private async Task<IEnumerable<Task<IAssetFsFile>>> ListGetCompilationTasks()
        {
            var sourceAssetsList = await _assetFs.List().ConfigureAwait(false);
            return sourceAssetsList.Select(x => EnsureCompiled(x));
        }

        public async Task<IAssetFsFile[]> List()
        {
            return (await Task.WhenAll(await ListGetCompilationTasks().ConfigureAwait(false)).ConfigureAwait(false)).Where(x => x != null).ToArray();
        }

        public async Task<List<Task<IAssetFsFile>>> ListTasks()
        {
            return (await ListGetCompilationTasks().ConfigureAwait(false)).ToList();
        }

        private async Task<IAssetFsFile> EnsureCompiled(IAssetFsFile file)
        {
            if (file == null)
            {
                return null;
            }

            if (file.Extension == "bin")
            {
                return file;
            }

            SemaphoreSlim localCompilerLock;
            await _compilerLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_compiledOverlay.ContainsKey(file.Name))
                {
                    return _compiledOverlay[file.Name];
                }

                if (_compilerLocks.ContainsKey(file.Name))
                {
                    localCompilerLock = _compilerLocks[file.Name];
                }
                else
                {
                    localCompilerLock = new SemaphoreSlim(1);
                    _compilerLocks[file.Name] = localCompilerLock;
                }
            }
            finally
            {
                _compilerLock.Release();
            }

            await localCompilerLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await _compilerLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    if (_compiledOverlay.ContainsKey(file.Name))
                    {
                        return _compiledOverlay[file.Name];
                    }
                }
                finally
                {
                    _compilerLock.Release();
                }

                var compilers = _compilers.Where(x => x.Extensions.Contains(file.Extension)).ToArray();
                if (compilers.Length == 0)
                {
                    // No compiler available for this type.
                    OnCompilerMissing(file, _targetPlatform);
                    return null;
                }

                OnCompileStart(file, _targetPlatform);
                var serializedAsset = new SerializedAsset();
                foreach (var compiler in compilers)
                {
                    await compiler.CompileAsync(file, new AssetDependencies(this, serializedAsset), _targetPlatform, serializedAsset).ConfigureAwait(false);
                }

                var memory = new MemoryStream();
                await serializedAsset.WriteTo(memory).ConfigureAwait(false);
                var compiledFsFile = new CompiledFsFile(file.Name, DateTimeOffset.UtcNow, memory);
                OnCompileFinish(file, compiledFsFile, _targetPlatform);
                _compiledOverlay[file.Name] = compiledFsFile;
                return _compiledOverlay[file.Name];
            }
            finally
            {
                localCompilerLock.Release();
            }
        }

        public void Dispose()
        {
            _assetFs.UnregisterUpdateNotifier(OnAssetUpdated);
        }

        private class AssetDependencies : IAssetDependencies
        {
            private readonly CompiledAssetFs _compiledAssetFs;
            private readonly SerializedAsset _serializedAsset;

            public AssetDependencies(CompiledAssetFs compiledAssetFs, SerializedAsset serializedAsset)
            {
                _compiledAssetFs = compiledAssetFs;
                _serializedAsset = serializedAsset;
            }

            Task<IAssetFsFile[]> IAssetDependencies.GetAvailableCompileTimeFiles()
            {
                return _compiledAssetFs._assetFs.List();
            }

            async Task<SerializedAsset> IAssetDependencies.GetOptionalCompileTimeCompiledDependency(string name)
            {
                // Always add the dependency, since the compiler will do different things if the dependency
                // appears on disk.
                _serializedAsset.AddCompilationDependency(name);

                var compiledAssetFile = await _compiledAssetFs.EnsureCompiled(await _compiledAssetFs._assetFs.Get(name).ConfigureAwait(false)).ConfigureAwait(false);
                if (compiledAssetFile == null)
                {
                    return null;
                }
                using (var stream = await compiledAssetFile.GetContentStream().ConfigureAwait(false))
                {
                    return await SerializedAsset.FromStream(stream, false).ConfigureAwait(false);
                }
            }

            Task<IAssetFsFile> IAssetDependencies.GetOptionalCompileTimeFileDependency(string name)
            {
                return _compiledAssetFs._assetFs.Get(name);
            }

            async Task<SerializedAsset> IAssetDependencies.GetRequiredCompileTimeCompiledDependency(string name)
            {
                var asset = await ((IAssetDependencies)this).GetOptionalCompileTimeCompiledDependency(name).ConfigureAwait(false);
                if (asset == null)
                {
                    throw new InvalidOperationException("The required compile-time dependency was not satisified: " + name);
                }
                return asset;
            }

            async Task<IAssetFsFile> IAssetDependencies.GetRequiredCompileTimeFileDependency(string name)
            {
                var asset = await ((IAssetDependencies)this).GetOptionalCompileTimeFileDependency(name).ConfigureAwait(false);
                if (asset == null)
                {
                    throw new InvalidOperationException("The required compile-time dependency was not satisified: " + name);
                }
                return asset;
            }
        }

        private class CompiledFsFile : IAssetFsFile
        {
            private readonly MemoryStream _stream;
            private readonly SemaphoreSlim _streamSemaphore;
            private string[] _dependencies;

            public CompiledFsFile(string name, DateTimeOffset compiledDateTimeOffset, MemoryStream stream)
            {
                Name = name;
                ModificationTimeUtcTimestamp = compiledDateTimeOffset;
                _stream = stream;
                _streamSemaphore = new SemaphoreSlim(1);
            }

            public string Name { get; }

            public string Extension => "bin";

            public DateTimeOffset ModificationTimeUtcTimestamp { get; }

            public async Task<Stream> GetContentStream()
            {
                await _streamSemaphore.WaitAsync();
                try
                {
                    var stream = new MemoryStream();
                    _stream.Seek(0, SeekOrigin.Begin);
                    await _stream.CopyToAsync(stream).ConfigureAwait(false);
                    stream.Seek(0, SeekOrigin.Begin);
                    return stream;
                }
                finally
                {
                    _streamSemaphore.Release();
                }
            }

            public async Task<string[]> GetDependentOnAssetFsFileNames()
            {
                if (_dependencies != null)
                {
                    return _dependencies;
                }

                await _streamSemaphore.WaitAsync();
                try
                {
                    _stream.Seek(0, SeekOrigin.Begin);
                    _dependencies = (await SerializedAsset.FromStream(_stream, true).ConfigureAwait(false)).Dependencies.ToArray();
                    return _dependencies;
                }
                finally
                {
                    _streamSemaphore.Release();
                }
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
