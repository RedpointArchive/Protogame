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
        private readonly IAssetLoader[] _assetLoaders;
        private readonly IAssetSaver[] _assetSavers;
        private readonly IProfiler _profiler;
        private readonly IRawAssetLoader _rawAssetLoader;
        private readonly IRawAssetSaver _rawAssetSaver;
        private readonly ITransparentAssetCompiler _transparentAssetCompiler;
        private readonly IConsoleHandle _consoleHandle;

        private readonly Dictionary<string, ISingleAssetReference<IAsset>> _assets;
        private readonly ConcurrentQueue<ISingleAssetReference<IAsset>> _assetsToLoad;
        private readonly ConcurrentQueue<ISingleAssetReference<IAsset>> _assetsToFinalize;
        private Thread _loadingThread;

        public DefaultAssetManager(
            IKernel kernel,
            IAssetLoader[] assetLoaders,
            IAssetSaver[] assetSavers,
            IProfiler[] profilers,
            IRawAssetLoader rawAssetLoader,
            IRawAssetSaver rawAssetSaver,
            ITransparentAssetCompiler transparentAssetCompiler,
            ICoroutine coroutine,
            IConsoleHandle consoleHandle)
        {
            _kernel = kernel;
            _assetLoaders = assetLoaders;
            _assetSavers = assetSavers;
            _profiler = profilers.Length > 0 ? profilers[0] : null;
            _rawAssetLoader = rawAssetLoader;
            _rawAssetSaver = rawAssetSaver;
            _transparentAssetCompiler = transparentAssetCompiler;
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
                        assetReference.Update(GetUnresolved(assetReference.Name), AssetReferenceState.PartiallyReady);
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

        public IAsset GetUnresolved(string name)
        {
            var candidatesWithTimes = _rawAssetLoader.LoadRawAssetCandidatesWithModificationDates(name);
            var loaders = _assetLoaders.ToArray();
            var failedDueToCompilation = false;
            var hasMoreThanZeroCandidates = false;

            var candidates = candidatesWithTimes.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();

            foreach (var candidate in candidates)
            {
                hasMoreThanZeroCandidates = true;

                foreach (var loader in loaders)
                {
                    var canLoad = false;
                    try
                    {
                        canLoad = loader.CanLoad(candidate);
                    }
                    catch (Exception)
                    {
                    }

                    if (canLoad)
                    {
                        var result = loader.Load(name, candidate);
                        
                        if (!this.SkipCompilation)
                        {
                            _transparentAssetCompiler.Handle(result);

                            if (result.SourceOnly && (!this.AllowSourceOnly || name == "font.Default"))
                            {
                                // We can't have source only assets past this point.  The compilation
                                // failed, but we definitely do have a source representation, so let's
                                // keep that around if we need to throw an exception.
                                failedDueToCompilation = true;
                                Console.WriteLine(
                                    "WARNING: Unable to compile " + name
                                    + " at runtime (a compiled version may be used).");
                                break;
                            }
                        }
                        
                        return result;
                    }
                }
            }

            if (failedDueToCompilation)
            {
                throw new AssetNotCompiledException(name);
            }

            if (!hasMoreThanZeroCandidates)
            {
                throw new AssetNotFoundException(name);
            }

            // NOTE: We don't use asset defaults with the local asset manager, if it
            // doesn't exist, the load fails.
            throw new InvalidOperationException(
                "Unable to load asset '" + name + "'.  " + "No loader for this asset could be found.");
        }

        public IAssetReference<T> GetPreferred<T>(string[] namePreferenceList) where T : class, IAsset
        {
            var assetReferenceList = namePreferenceList.Select(Get<T>).ToArray();
            return new PreferenceListAssetReference<T>(assetReferenceList);
        }
    }
}