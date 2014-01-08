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
#if DEBUG
        private Dictionary<string, LocalAsset> m_Assets = new Dictionary<string, LocalAsset>();
#else
        private Dictionary<string, IAsset> m_Assets = new Dictionary<string, IAsset>();
#endif
        private IAssetLoader[] m_AssetLoaders;
        private IAssetSaver[] m_AssetSavers;
        private ITransparentAssetCompiler m_TransparentAssetCompiler;

        public LocalAssetManager(
            IKernel kernel,
            IRawAssetLoader rawLoader,
            IRawAssetSaver rawSaver,
            IAssetLoader[] loaders,
            IAssetSaver[] savers,
            ITransparentAssetCompiler transparentAssetCompiler)
        {
            this.m_Kernel = kernel;
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

        public void Dirty(string asset)
        {
#if DEBUG
            lock (this.m_Assets)
            {
                var assetObj = this.m_Assets[asset];
                this.m_Assets.Remove(asset);
                assetObj.Dirty();
            }
#endif
        }
        
        public void RescanAssets()
        {
            foreach (var asset in this.m_RawAssetLoader.ScanRawAssets())
                this.GetUnresolved(asset);
        }

        public IAsset GetUnresolved(string asset)
        {
            if (this.m_Assets.ContainsKey(asset))
                return this.m_Assets[asset];
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

                            if (result.SourceOnly && !this.AllowSourceOnly)
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
#if DEBUG
                        var local = new LocalAsset(asset, result, this);
                        this.m_Assets.Add(asset, local);
                        return local;
#else
                        this.m_Assets.Add(asset, result);
                        return result;
#endif
                    }
                }
            }

            if (failedDueToCompilation)
            {
                throw new AssetNotCompiledException(asset);
            }

            // NOTE: We don't use asset defaults with the local asset manager, if it
            // doesn't exist, the load fails.
            throw new InvalidOperationException(
                "Unable to load asset '" + asset + "'.  " +
                "No loader for this asset could be found.");
        }

        public T Get<T>(string asset) where T : class, IAsset
        {
            return this.GetUnresolved(asset).Resolve<T>();
        }
        
        public T TryGet<T>(string asset) where T : class, IAsset
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

        public IAsset[] GetAll()
        {
            lock (this.m_Assets)
            {
                if (!this.m_HasScanned)
                {
                    this.RescanAssets();
                    this.m_HasScanned = true;
                }
#if DEBUG
                return this.m_Assets.Values.Select(x => x.Instance).ToArray();
#else
                return this.m_Assets.Values.ToArray();
#endif
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
#if DEBUG
                    this.m_Assets[asset.Name] = 
                        new LocalAsset(asset.Name, asset, this);
#else
                    this.m_Assets[asset.Name] = asset;
#endif
                    if (bake)
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

