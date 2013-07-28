//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using System.IO;

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

        private IRawAssetLoader m_RawAssetLoader;
        private IRawAssetSaver m_RawAssetSaver;
        private Dictionary<string, IAsset> m_Assets = new Dictionary<string, IAsset>();
        private Dictionary<string, object> m_RawAssets = new Dictionary<string, object>();
        private IEnumerable<IAssetLoader> m_AssetLoaders;
        private IEnumerable<IAssetSaver> m_AssetSavers;

        public LocalAssetManager(
            IRawAssetLoader rawLoader,
            IRawAssetSaver rawSaver,
            IEnumerable<IAssetLoader> loaders,
            IEnumerable<IAssetSaver> savers)
        {
            this.m_RawAssetLoader = rawLoader;
            this.m_RawAssetSaver = rawSaver;
            this.m_AssetLoaders = loaders;
            this.m_AssetSavers = savers;
        }

        public void Dirty(string asset)
        {
        }
        
        public void RescanAssets()
        {
            foreach (var asset in this.m_RawAssetLoader.ScanRawAssets())
                this.GetUnresolved(asset);
        }

        public IAsset GetUnresolved(string asset)
        {
            object obj;
            if (this.m_Assets.ContainsKey(asset))
                return this.m_Assets[asset];
            if (this.m_RawAssets.ContainsKey(asset))
                obj = this.m_RawAssets[asset];
            else
            {
                obj = this.m_RawAssetLoader.LoadRawAsset(asset);
                this.m_RawAssets.Add(asset, obj);
            }
            var loaders = this.m_AssetLoaders.ToArray();
            if (obj != null)
            {
                foreach (var loader in loaders)
                {
                    var canLoad = false;
                    try
                    {
                        canLoad = loader.CanHandle(obj);
                    }
                    catch (Exception)
                    {
                    }
                    if (canLoad)
                    {
                        var result = loader.Handle(asset, obj);
                        this.m_Assets.Add(asset, result);
                        return result;
                    }
                }
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
            try
            {
                return this.GetUnresolved(asset).Resolve<T>();
            }
            catch (AssetNotFoundException ex)
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

        public void Save(IAsset asset)
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
                    var result = saver.Handle(asset);
                    this.m_RawAssets[asset.Name] = result;
                    return;
                }
            }
            throw new InvalidOperationException(
                "Unable to save asset '" + asset + "'.  " +
                "No saver for this asset could be found.");
        }

        public void Bake(IAsset asset)
        {
            this.Save(asset);
            this.m_RawAssetSaver.SaveRawAsset(asset.Name, this.m_RawAssets[asset.Name]);
        }
    }
}

