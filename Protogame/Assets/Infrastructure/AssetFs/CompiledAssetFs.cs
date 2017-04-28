using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Protogame
{
    public class CompiledAssetFs : ICompiledAssetFs, IAssetDependencies
    {
        private readonly IAssetCompiler[] _compilers;
        private readonly IAssetFs _assetFs;
        private readonly Dictionary<string, IAssetFsFile> _compiledOverlay;
        private readonly Dictionary<string, SemaphoreSlim> _compilerLocks;
        private readonly SemaphoreSlim _compilerLock;
        private readonly TargetPlatform _targetPlatform;

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
                    await compiler.CompileAsync(file, this, _targetPlatform, serializedAsset).ConfigureAwait(false);
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

        private class CompiledFsFile : IAssetFsFile
        {
            private readonly MemoryStream _stream;

            public CompiledFsFile(string name, DateTimeOffset compiledDateTimeOffset, MemoryStream stream)
            {
                Name = name;
                ModificationTimeUtcTimestamp = compiledDateTimeOffset;
                _stream = stream;
                _stream.Seek(0, SeekOrigin.Begin);
            }

            public string Name { get; }

            public string Extension => "bin";

            public DateTimeOffset ModificationTimeUtcTimestamp { get; }

            public async Task<Stream> GetContentStream()
            {
                var stream = new MemoryStream();
                await _stream.CopyToAsync(stream).ConfigureAwait(false);
                stream.Seek(0, SeekOrigin.Begin);
                _stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }

        Task<IAssetFsFile[]> IAssetDependencies.GetAvailableCompileTimeFiles()
        {
            return _assetFs.List();
        }

        async Task<SerializedAsset> IAssetDependencies.GetOptionalCompileTimeCompiledDependency(string name)
        {
            var compiledAssetFile = await EnsureCompiled(await _assetFs.Get(name).ConfigureAwait(false)).ConfigureAwait(false);
            if (compiledAssetFile == null)
            {
                return null;
            }
            using (var stream = await compiledAssetFile.GetContentStream().ConfigureAwait(false))
            {
                return await SerializedAsset.FromStream(stream).ConfigureAwait(false);
            }
        }

        Task<IAssetFsFile> IAssetDependencies.GetOptionalCompileTimeFileDependency(string name)
        {
            return _assetFs.Get(name);
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
}
