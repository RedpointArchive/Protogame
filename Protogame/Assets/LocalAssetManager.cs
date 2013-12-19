//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
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

        private IKernel m_Kernel;
        private IRawAssetLoader m_RawAssetLoader;
        private IRawAssetSaver m_RawAssetSaver;
        private Dictionary<string, IAsset> m_Assets = new Dictionary<string, IAsset>();
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

        public void Dirty(string asset)
        {
        }
        
        public void RescanAssets()
        {
            foreach (var asset in this.m_RawAssetLoader.ScanRawAssets())
                this.GetUnresolved(asset);
            GC.Collect();
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
                        this.m_Assets.Add(asset, result);
                        result = this.m_TransparentAssetCompiler.Handle(result);
                        if (result.SourceOnly)
                        {
                            // We can't have source only assets past this point.  The compilation
                            // failed, but we definitely do have a source representation, so let's
                            // keep that around if we need to throw an exception.
                            failedDueToCompilation = true;
                            break;
                        }
                        return result;
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
                this.RescanAssets();
                return this.m_Assets.Values.ToArray();
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
                    var result = saver.Handle(asset, bake ? AssetTarget.SourceFile : AssetTarget.Runtime);
                    this.m_Assets[asset.Name] = asset;
                    if (bake)
                        this.m_RawAssetSaver.SaveRawAsset(asset.Name, result);
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
            this.m_TransparentAssetCompiler.Handle(asset, true);
        }
    }
}

