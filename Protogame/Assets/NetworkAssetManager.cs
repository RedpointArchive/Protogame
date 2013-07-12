//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Process4.Attributes;
using System.Collections.Generic;
using Ninject;
using System.Linq;
using System;

namespace Protogame
{
    /// <summary>
    /// An implementation of an asset manager where the assets can be
    /// modified and changed over the network (usually by the asset
    /// manager program).
    /// </summary>
    [Distributed]
    public class NetworkAssetManager : IAssetManager
    {
        public string Status { get; set; }
        public bool IsRemoting { get { return true; } }

        [ClientCallable]
        public bool IsReady()
        {
            return this.m_Kernel.TryGet<IRawAssetLoader>() != null;
        }

        [Local]
        private IRawAssetLoader m_RawAssetLoader;

        [Local]
        private IRawAssetSaver m_RawAssetSaver;

        [Local]
        private IKernel m_Kernel;

        [Local]
        private Dictionary<string, NetworkAsset> m_Assets;

        [Local]
        private Dictionary<string, object> m_RawAssets;

        public NetworkAssetManager()
        {
            this.m_Assets = new Dictionary<string, NetworkAsset>();
            this.m_RawAssets = new Dictionary<string, object>();
        }

        public void Dirty(string asset)
        {
            lock (this.m_Assets)
            {
                this.m_Assets[asset].Dirty = true;
            }
        }

        public IAsset[] GetAll()
        {
            lock (this.m_Assets)
            {
                return this.m_Assets.Values.ToArray();
            }
        }

        [ClientCallable]
        public IAsset Get(string asset)
        {
            lock (this.m_Assets)
            {
                // If the network asset already exists, return it.
                if (this.m_Assets.ContainsKey(asset))
                {
                    if (!this.m_Assets[asset].Dirty)
                        return this.m_Assets[asset];
                    this.m_Assets.Remove(asset);
                }

                // Otherwise load the raw asset if that doesn't exist.
                object raw;
                NetworkAsset result;
                if (!this.m_RawAssets.ContainsKey(asset))
                {
                    if (this.m_RawAssetLoader == null)
                        this.m_RawAssetLoader = this.m_Kernel.Get<IRawAssetLoader>();
                    try
                    {
                        raw = this.m_RawAssetLoader.LoadRawAsset(asset);
                    }
                    catch (AssetNotFoundException ex)
                    {
                        result = new NetworkAsset(this.m_Kernel.GetAll<IAssetLoader>(), null, asset, this);
                        this.m_Assets.Add(asset, result);
                        return result;
                    }
                }
                else
                    raw = this.m_RawAssets[asset];

                // We now have our raw asset and we need to wrap it in
                // a NetworkAsset.
                result = new NetworkAsset(this.m_Kernel.GetAll<IAssetLoader>(), raw, asset, this);
                this.m_Assets.Add(asset, result);
                return result;
            }
        }

        public void Save(IAsset asset)
        {
            var savers = this.m_Kernel.GetAll<IAssetSaver>().ToArray();
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
                    this.m_Assets[asset.Name].Dirty = true;
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
            if (this.m_RawAssetSaver == null)
                this.m_RawAssetSaver = this.m_Kernel.Get<IRawAssetSaver>();
            this.m_RawAssetSaver.SaveRawAsset(asset.Name, this.m_RawAssets[asset.Name]);
        }
    }
}

