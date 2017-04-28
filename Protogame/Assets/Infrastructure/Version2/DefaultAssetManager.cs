using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Protoinject;

namespace Protogame
{
    public class DefaultAssetManager : IAssetManager
    {
        private readonly IKernel _kernel;
        private readonly ICompiledAssetFs _compiledAssetFs;
        private readonly IProfiler _profiler;
        private readonly IConsoleHandle _consoleHandle;

        private readonly Dictionary<string, ISingleAssetReference<IAsset>> _assets;
        private readonly ConcurrentQueue<ISingleAssetReference<IAsset>> _assetsToLoad;
        private readonly ConcurrentQueue<ISingleAssetReference<IAsset>> _assetsToFinalize;
        private Thread _loadingThread;

        public DefaultAssetManager(
            IKernel kernel,
            ICompiledAssetFs compiledAssetFs,
            IProfiler[] profilers,
            ICoroutine coroutine,
            IConsoleHandle consoleHandle)
        {
            _kernel = kernel;
            _profiler = profilers.Length > 0 ? profilers[0] : null;
            _consoleHandle = consoleHandle;

            _assets = new Dictionary<string, ISingleAssetReference<IAsset>>();
            _assetsToLoad = new ConcurrentQueue<ISingleAssetReference<IAsset>>();
            _assetsToFinalize = new ConcurrentQueue<ISingleAssetReference<IAsset>>();

            coroutine.Run(FinalizeAssets);
        }

        private async Task FinalizeAssets()
        {
            while (true)
            {
                ISingleAssetReference<IAsset> assetReference;
                if (_assetsToFinalize.TryDequeue(out assetReference))
                {
                    var asset = assetReference.Asset as INativeAsset;
                    if (asset == null)
                    {
                        _consoleHandle.LogInfo(assetReference.Name + ": No native component; immediately marking as ready.");
                        assetReference.Update(AssetReferenceState.Ready);
                        continue;
                    }

                    // Perform the asset finalization on the game thread.
                    try
                    {
                        _consoleHandle.LogInfo(assetReference.Name + ": Requesting load of native components.");
                        asset.ReadyOnGameThread();
                        assetReference.Update(AssetReferenceState.Ready);
                        _consoleHandle.LogInfo(assetReference.Name + ": Native components loaded successfully; asset marked as ready.");
                    }
                    catch (NoAssetContentManagerException)
                    {
                        assetReference.Update(AssetReferenceState.Ready);
                        _consoleHandle.LogInfo(assetReference.Name + ": No asset content manager Native components loaded successfully; asset marked as ready.");
                    }
                    catch (Exception ex)
                    {
                        assetReference.Update(ex);
                    }
                }

                await Task.Yield();
            }
        }

        public IAssetReference<T> Get<T>(string name) where T : class, IAsset
        {
            if (_assets.ContainsKey(name))
            {
                return (IAssetReference<T>)_assets[name];
            }

            var assetReference = new DefaultAssetReference<T>(name);
            EnsureLoadingThreadStarted();
            _assetsToLoad.Enqueue(assetReference);
            _assets[name] = assetReference;
            return assetReference;
        }

        private void EnsureLoadingThreadStarted()
        {
            if (_loadingThread == null)
            {
                _loadingThread = new Thread(RunAssetLoading);
                _loadingThread.IsBackground = true;
                _loadingThread.Name = "Asset Loading Thread";
                _loadingThread.Start();
            }
        }

        private void RunAssetLoading()
        {
            while (true)
            {
                ISingleAssetReference<IAsset> assetReference;
                if (_assetsToLoad.TryDequeue(out assetReference))
                {
                    try
                    {
                        var assetTask = Task.Run(async () => await GetUnresolved(assetReference.Name));
                        while (!assetTask.IsCompleted)
                        {
                            Thread.Sleep(0);
                        }

                        assetReference.Update(assetTask.Result, AssetReferenceState.PartiallyReady);
                        _assetsToFinalize.Enqueue(assetReference);
                    }
                    catch (Exception e)
                    {
                        assetReference.Update(e);
                    }
                }

                Thread.Sleep(0);
            }
        }

        public bool SkipCompilation { get; set; }

        public bool AllowSourceOnly { get; set; }

        public async Task<IAsset> GetUnresolved(string name)
        {
            var compiledAsset = await _compiledAssetFs.Get(name);
            if (compiledAsset == null)
            {
                throw new AssetNotFoundException(name);
            }
            SerializedAsset serializedAsset;
            using (var stream = await compiledAsset.GetContentStream().ConfigureAwait(false))
            {
                serializedAsset = await SerializedAsset.FromStream(stream).ConfigureAwait(false);
            }
            var loaderType = serializedAsset.GetLoader();
            var loaderInstance = (IAssetLoader)_kernel.TryGet(loaderType);
            if (loaderInstance == null)
            {
                throw new InvalidOperationException(
                    "Unable to load asset '" + name + "'.  " + "No loader for this asset could be found.");
            }
            return await loaderInstance.Load(name, serializedAsset, this).ConfigureAwait(false);
        }

        public IAssetReference<T> GetPreferred<T>(string[] namePreferenceList) where T : class, IAsset
        {
            var assetReferenceList = namePreferenceList.Select(Get<T>).ToArray();
            return new PreferenceListAssetReference<T>(assetReferenceList);
        }
    }
}