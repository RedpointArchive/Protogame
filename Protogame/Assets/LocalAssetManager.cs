namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ninject;

    /// <summary>
    /// An implementation of an asset manager that is designed
    /// to edit and build files locally, without the asset
    /// manager program being connected to a running game.
    /// </summary>
    public class LocalAssetManager : IAssetManager
    {
        /// <summary>
        /// The m_ asset loaders.
        /// </summary>
        private readonly IAssetLoader[] m_AssetLoaders;

        /// <summary>
        /// The m_ asset savers.
        /// </summary>
        private readonly IAssetSaver[] m_AssetSavers;

        /// <summary>
        /// The m_ assets.
        /// </summary>
        private readonly Dictionary<string, IAsset> m_Assets = new Dictionary<string, IAsset>();

        /// <summary>
        /// The m_ profiler.
        /// </summary>
        private readonly IProfiler m_Profiler;

        /// <summary>
        /// The m_ raw asset loader.
        /// </summary>
        private readonly IRawAssetLoader m_RawAssetLoader;

        /// <summary>
        /// The m_ raw asset saver.
        /// </summary>
        private readonly IRawAssetSaver m_RawAssetSaver;

        /// <summary>
        /// The m_ transparent asset compiler.
        /// </summary>
        private readonly ITransparentAssetCompiler m_TransparentAssetCompiler;

        /// <summary>
        /// The m_ generate runtime proxies.
        /// </summary>
        private bool m_GenerateRuntimeProxies;

        /// <summary>
        /// The m_ has scanned.
        /// </summary>
        private bool m_HasScanned;

        /// <summary>
        /// The m_ kernel.
        /// </summary>
        private IKernel m_Kernel;

        /// <summary>
        /// The m_ proxies locked.
        /// </summary>
        private bool m_ProxiesLocked;

        /// <summary>
        /// Whether or not the assets are currently being scanned.
        /// </summary>
        private bool m_IsScanning;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalAssetManager"/> class.
        /// </summary>
        /// <param name="kernel">
        /// The kernel.
        /// </param>
        /// <param name="profilers">
        /// The profilers.
        /// </param>
        /// <param name="rawLoader">
        /// The raw loader.
        /// </param>
        /// <param name="rawSaver">
        /// The raw saver.
        /// </param>
        /// <param name="loaders">
        /// The loaders.
        /// </param>
        /// <param name="savers">
        /// The savers.
        /// </param>
        /// <param name="transparentAssetCompiler">
        /// The transparent asset compiler.
        /// </param>
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
        /// <value>
        /// The allow source only.
        /// </value>
        public bool AllowSourceOnly { get; set; }

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
                        "Assets have already been loaded in this asset manager; you can not "
                        + "change GenerateRuntimeProxies after assets have been loaded.");
                }

                this.m_GenerateRuntimeProxies = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is remoting.
        /// </summary>
        /// <value>
        /// The is remoting.
        /// </value>
        public bool IsRemoting
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Indicates that the local asset manager should not pass assets through
        /// the <see cref="ITransparentAssetCompiler"/> interface before returning them.  This
        /// option is used in ProtogameAssetTool where we need to cross-compile for different platforms
        /// and thus we always want to have the only the source information in the assets.
        /// </summary>
        /// <value>
        /// The skip compilation.
        /// </value>
        public bool SkipCompilation { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// The bake.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        public void Bake(IAsset asset)
        {
            this.SaveOrBake(asset, true);
        }

        /// <summary>
        /// The dirty.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
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

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Get<T>(string asset) where T : class, IAsset
        {
            using (this.m_Profiler.Measure("asset-manager-get: " + asset))
            {
                return this.GetUnresolved(asset).Resolve<T>();
            }
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IAsset[]"/>.
        /// </returns>
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

        /// <summary>
        /// The get unresolved.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        /// <exception cref="AssetNotFoundException">
        /// </exception>
        /// <exception cref="AssetNotCompiledException">
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// </exception>
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

            var candidatesWithTimes = this.m_RawAssetLoader.LoadRawAssetCandidatesWithModificationDates(asset);
            var loaders = this.m_AssetLoaders.ToArray();
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
                            this.m_TransparentAssetCompiler.Handle(result);

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

            if (!hasMoreThanZeroCandidates)
            {
                this.m_Assets[asset] = null;

                throw new AssetNotFoundException(asset);
            }

            // NOTE: We don't use asset defaults with the local asset manager, if it
            // doesn't exist, the load fails.
            throw new InvalidOperationException(
                "Unable to load asset '" + asset + "'.  " + "No loader for this asset could be found.");
        }

        /// <summary>
        /// The recompile.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
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

        /// <summary>
        /// Gets the names of all known assets.
        /// </summary>
        /// <returns>The names of all known assets.</returns>
        public string[] GetAllNames()
        {
            if (!this.m_HasScanned)
            {
                return this.m_RawAssetLoader.ScanRawAssets().Distinct().ToArray();
            }

            return this.m_Assets.Select(x => x.Key).ToArray();
        }

        /// <summary>
        /// The rescan assets.
        /// </summary>
        public void RescanAssets()
        {
            if (this.m_IsScanning)
            {
                throw new InvalidOperationException("GetAll() and RescanAssets() can not be called from within an asset loader or compiler.");
            }

            this.m_IsScanning = true;

            foreach (var asset in this.m_RawAssetLoader.ScanRawAssets())
            {
                this.GetUnresolved(asset);
            }

            this.m_IsScanning = false;

            this.m_HasScanned = true;
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        public void Save(IAsset asset)
        {
            this.SaveOrBake(asset, false);
        }

        /// <summary>
        /// The try get.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T TryGet<T>(string asset) where T : class, IAsset
        {
            using (this.m_Profiler.Measure("asset-manager-try-get: " + asset))
            {
                if (string.IsNullOrWhiteSpace(asset))
                {
                    return null;
                }

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

        /// <summary>
        /// The save or bake.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <param name="bake">
        /// The bake.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
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
                "Unable to save asset '" + asset + "'.  " + "No saver for this asset could be found.");
        }
    }
}
