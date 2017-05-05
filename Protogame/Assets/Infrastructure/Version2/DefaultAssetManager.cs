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
    public class DefaultAssetManager : IAssetManager, IDisposable
    {
        private readonly IKernel _kernel;
        private readonly ICompiledAssetFs _compiledAssetFs;
        private readonly IProfiler _profiler;
        private readonly IConsoleHandle _consoleHandle;

        private readonly Dictionary<string, ISingleAssetReference<IAsset>> _assets;
        private readonly ConcurrentQueue<ISingleAssetReference<IAsset>> _assetsToLoad;
        private readonly ConcurrentQueue<Tuple<IAsset, ISingleAssetReference<IAsset>>> _assetsToFinalize;
        private Thread _loadingThread;

        public DefaultAssetManager(
            IKernel kernel,
            ICompiledAssetFs compiledAssetFs,
            IProfiler[] profilers,
            ICoroutine coroutine,
            IConsoleHandle consoleHandle)
        {
            _kernel = kernel;
            _compiledAssetFs = compiledAssetFs;
            _profiler = profilers.Length > 0 ? profilers[0] : null;
            _consoleHandle = consoleHandle;

            _assets = new Dictionary<string, ISingleAssetReference<IAsset>>();
            _assetsToLoad = new ConcurrentQueue<ISingleAssetReference<IAsset>>();
            _assetsToFinalize = new ConcurrentQueue<Tuple<IAsset, ISingleAssetReference<IAsset>>>();

            _compiledAssetFs.RegisterUpdateNotifier(OnAssetUpdated);

            coroutine.Run(FinalizeAssets);
        }

        public void Dispose()
        {
            _compiledAssetFs.UnregisterUpdateNotifier(OnAssetUpdated);
        }

        public async Task OnAssetUpdated(string assetName)
        {
            if (!_assets.ContainsKey(assetName))
            {
                // The asset has never been loaded.
                return;
            }

            EnsureLoadingThreadStarted();
            _assetsToLoad.Enqueue(_assets[assetName]);
        }

        private async Task FinalizeAssets()
        {
            while (true)
            {
                Tuple<IAsset, ISingleAssetReference<IAsset>> assetTuple;
                if (_assetsToFinalize.TryDequeue(out assetTuple))
                {
                    var asset = assetTuple.Item1 as INativeAsset;
                    var assetReference = assetTuple.Item2;
                    if (asset == null)
                    {
                        _consoleHandle.LogInfo(assetReference.Name + ": No native component; immediately marking as ready.");
                        assetReference.Update(assetTuple.Item1, AssetReferenceState.Ready);
                        continue;
                    }

                    // Perform the asset finalization on the game thread.
                    try
                    {
                        _consoleHandle.LogInfo(assetReference.Name + ": Requesting load of native components.");
                        asset.ReadyOnGameThread();
                        assetReference.Update(assetTuple.Item1, AssetReferenceState.Ready);
                        _consoleHandle.LogInfo(assetReference.Name + ": Native components loaded successfully; asset marked as ready.");
                    }
                    catch (NoAssetContentManagerException)
                    {
                        assetReference.Update(assetTuple.Item1, AssetReferenceState.Ready);
                        _consoleHandle.LogInfo(assetReference.Name + ": No asset content manager Native components loaded successfully; asset marked as ready.");
                    }
                    catch (Exception ex)
                    {
                        // Only store exceptions if we don't already have a readied
                        // asset (due to live reload scenarios).
                        if (!assetReference.IsReady)
                        {
                            assetReference.Update(ex);
                        }
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
                        _assetToLoadInGetUnresolved = assetReference.Name;
                        var assetTask = Task.Run(GetUnresolvedFromRunAssetLoadingThread);
                        while (!assetTask.IsCompleted)
                        {
                            Thread.Sleep(0);
                        }

                        // Only move into a partially ready status if the asset isn't already loaded.
                        // When we do live reload, we want to keep the old asset (with all of it's
                        // loaded resources) as is until the new asset has been readied on the
                        // game thread.
                        if (!assetReference.IsReady)
                        {
                            assetReference.Update(assetTask.Result, AssetReferenceState.PartiallyReady);
                        }

                        _assetsToFinalize.Enqueue(new Tuple<IAsset, ISingleAssetReference<IAsset>>(assetTask.Result, assetReference));
                    }
                    catch (Exception e)
                    {
                        assetReference.Update(e);
                    }
                }

                Thread.Sleep(0);
            }
        }

        private string _assetToLoadInGetUnresolved;

        /// <remarks>
        /// We store our to-be-loaded asset name in <see cref="_assetToLoadInGetUnresolved"/> to reduce
        /// memory allocations, as passing an async lambda to Task.Run results in a lot of garbage collection.
        /// </remarks>
        private Task<IAsset> GetUnresolvedFromRunAssetLoadingThread()
        {
            return GetUnresolved(_assetToLoadInGetUnresolved);
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
            using (var serializedAsset = ReadableSerializedAsset.FromStream(await compiledAsset.GetContentStream().ConfigureAwait(false), false))
            {
                var loaderType = serializedAsset.GetLoader();
                var loaderInstance = (IAssetLoader)_kernel.TryGet(loaderType);
                if (loaderInstance == null)
                {
                    throw new InvalidOperationException(
                        "Unable to load asset '" + name + "'.  " + "No loader for this asset could be found.");
                }
                return await loaderInstance.Load(name, serializedAsset, this).ConfigureAwait(false);
            }
        }

        public IAssetReference<T> GetPreferred<T>(string[] namePreferenceList) where T : class, IAsset
        {
            var assetReferenceList = namePreferenceList.Select(Get<T>).ToArray();
            return new PreferenceListAssetReference<T>(assetReferenceList);
        }
    }
}