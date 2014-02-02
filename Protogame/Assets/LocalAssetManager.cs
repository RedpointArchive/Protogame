using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;

namespace Protogame
{
    /// <summary>
    /// An implementation of an asset manager that is designed
    /// to edit and build files locally, without the asset
    /// manager program being connected to a running game.
    /// </summary>
    public class LocalAssetManager : IAssetManager
    {
        public string Status { get; set; }
        public bool IsRemoting { get { return false; } }

        private bool m_HasScanned;
        private IKernel m_Kernel;
        private IRawAssetLoader m_RawAssetLoader;
        private IRawAssetSaver m_RawAssetSaver;

        private readonly IProfiler m_Profiler;

        private Dictionary<string, IAsset> m_Assets = new Dictionary<string, IAsset>();
        private IAssetLoader[] m_AssetLoaders;
        private IAssetSaver[] m_AssetSavers;
        private ITransparentAssetCompiler m_TransparentAssetCompiler;

        private bool m_GenerateRuntimeProxies;

        private bool m_ProxiesLocked = false;

        public LocalAssetManager(
            IKernel kernel,
            IProfiler[] profilers,
            IRawAssetLoader rawLoader,
            IRawAssetSaver rawSaver,
            IAssetLoader[] loaders,
            IAssetSaver[] savers,
            ITransparentAssetCompiler transparentAssetCompiler)
        {
            this.m_Kernel = kernel;
            this.m_Profiler = profilers.Length > 0 ? profilers[0] : new NullProfiler();
            this.m_RawAssetLoader = rawLoader;
            this.m_RawAssetSaver = rawSaver;
            this.m_AssetLoaders = loaders;
            this.m_AssetSavers = savers;
            this.m_TransparentAssetCompiler = transparentAssetCompiler;
        }

        /// <summary>
        /// Indicates that source-only assets should be returned from the asset manager.  Generally
        /// this option is only useful for the asset manager.
        /// </summary>
        public bool AllowSourceOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates that the local asset manager should not pass assets through
        /// the <see cref="ITransparentAssetCompiler"/> interface before returning them.  This
        /// option is used in ProtogameAssetTool where we need to cross-compile for different platforms
        /// and thus we always want to have the only the source information in the assets.
        /// </summary>
        public bool SkipCompilation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the local asset manager should proxy all assets, allowing assets to be
        /// reloaded on the fly.  Generally this is not very useful for the local asset manager, unless
        /// you are building some sort of reloading mechanism into your game (such as to reload assets when
        /// the user presses F5).
        /// </summary>
        /// <value>
        /// Whether the local asset manager should proxy all assets.
        /// </value>
        public bool GenerateRuntimeProxies
        {
            get
            {
                return this.m_GenerateRuntimeProxies;
            }
            set
            {
                if (this.m_ProxiesLocked)
                {
                    throw new InvalidOperationException(
                        "Assets have already been loaded in this asset manager; you can not " +
                        "change GenerateRuntimeProxies after assets have been loaded.");
                }

                this.m_GenerateRuntimeProxies = value;
            }
        }

        public void Dirty(string asset)
        {
            if (this.GenerateRuntimeProxies)
            {
                lock (this.m_Assets)
                {
                    var assetObj = this.m_Assets[asset];
                    this.m_Assets.Remove(asset);
                    if (assetObj != null)
                    {
                        ((LocalAsset)assetObj).Dirty();
                    }
                }
            }
        }
        
        public void RescanAssets()
        {
            foreach (var asset in this.m_RawAssetLoader.ScanRawAssets())
                this.GetUnresolved(asset);

            this.m_HasScanned = true;
        }

        public IAsset GetUnresolved(string asset)
        {
            if (this.m_Assets.ContainsKey(asset))
            {
                if (this.m_Assets[asset] == null)
                {
                    throw new AssetNotFoundException(asset);
                }

                return this.m_Assets[asset];
            }

            var candidates = this.m_RawAssetLoader.LoadRawAsset(asset);
            var loaders = this.m_AssetLoaders.ToArray();
            var failedDueToCompilation = false;

            foreach (var candidate in candidates)
            {
                foreach (var loader in loaders)
                {
                    var canLoad = false;
                    try
                    {
                        canLoad = loader.CanHandle(candidate);
                    }
                    catch (Exception)
                    {
                    }
                    if (canLoad)
                    {
                        var result = loader.Handle(this, asset, candidate);
                        if (!this.SkipCompilation)
                        {
                            result = this.m_TransparentAssetCompiler.Handle(result);

                            if (result.SourceOnly && (!this.AllowSourceOnly || asset == "font.Default"))
                            {
                                // We can't have source only assets past this point.  The compilation
                                // failed, but we definitely do have a source representation, so let's
                                // keep that around if we need to throw an exception.
                                failedDueToCompilation = true;
                                Console.WriteLine(
                                    "WARNING: Unable to compile " + asset
                                    + " at runtime (a compiled version may be used).");
                                break;
                            }
                        }

                        this.m_ProxiesLocked = true;
                        if (this.GenerateRuntimeProxies)
                        {
                            var local = new LocalAsset(asset, result, this);
                            this.m_Assets.Add(asset, local);
                            return local;
                        }
                        
                        this.m_Assets.Add(asset, result);
                        return result;
                    }
                }
            }

            if (failedDueToCompilation)
            {
                throw new AssetNotCompiledException(asset);
            }

            if (candidates.Length == 0)
            {
                this.m_Assets[asset] = null;

                throw new AssetNotFoundException(asset);
            }

            // NOTE: We don't use asset defaults with the local asset manager, if it
            // doesn't exist, the load fails.
            throw new InvalidOperationException(
                "Unable to load asset '" + asset + "'.  " +
                "No loader for this asset could be found.");
        }

        public T Get<T>(string asset) where T : class, IAsset
        {
            using (this.m_Profiler.Measure("asset-manager-get: " + asset))
            {
                return this.GetUnresolved(asset).Resolve<T>();
            }
        }
        
        public T TryGet<T>(string asset) where T : class, IAsset
        {
            using (this.m_Profiler.Measure("asset-manager-try-get: " + asset))
            {
                if (string.IsNullOrWhiteSpace(asset))
                    return null;
                try
                {
                    return this.GetUnresolved(asset).Resolve<T>();
                }
                catch (AssetNotFoundException)
                {
                    return null;
                }
            }
        }

        public IAsset[] GetAll()
        {
            using (this.m_Profiler.Measure("asset-manager-get-all"))
            {
                lock (this.m_Assets)
                {
                    this.m_ProxiesLocked = true;

                    if (!this.m_HasScanned)
                    {
                        this.RescanAssets();
                    }

                    if (this.GenerateRuntimeProxies)
                    {
                        return this.m_Assets.Values.OfType<LocalAsset>().Select(x => x.Instance).ToArray();
                    }

                    return this.m_Assets.Values.ToArray();
                }
            }
        }
        
        private void SaveOrBake(IAsset asset, bool bake)
        {
            var savers = this.m_AssetSavers.ToArray();
            foreach (var saver in savers)
            {
                var canSave = false;
                try
                {
                    canSave = saver.CanHandle(asset);
                }
                catch (Exception)
                {
                }
                if (canSave)
                {
                    this.m_ProxiesLocked = true;
                    if (this.GenerateRuntimeProxies)
                    {
                        this.m_Assets[asset.Name] = new LocalAsset(asset.Name, asset, this);
                    }
                    else
                    {
                        this.m_Assets[asset.Name] = asset;
                    }

                    if (bake && !asset.CompiledOnly)
                    {
                        var result = saver.Handle(asset, AssetTarget.SourceFile);
                        if (result == null)
                        {
                            // We can handle this asset; but we explicitly do not want to save it.
                            // This is used when we load a raw asset from a PNG, and we don't want to
                            // save a .asset file back to disk (because then we'll have two matching
                            // assets in the asset folder, both representing the exact same texture).
                            return;
                        }

                        this.m_RawAssetSaver.SaveRawAsset(asset.Name, result);
                    }

                    return;
                }
            }

            throw new InvalidOperationException(
                "Unable to save asset '" + asset + "'.  " +
                "No saver for this asset could be found.");
        }

        public void Save(IAsset asset)
        {
            this.SaveOrBake(asset, false);
        }

        public void Bake(IAsset asset)
        {
            this.SaveOrBake(asset, true);
        }

        public void Recompile(IAsset asset)
        {
            try
            {
                this.m_TransparentAssetCompiler.Handle(asset, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

