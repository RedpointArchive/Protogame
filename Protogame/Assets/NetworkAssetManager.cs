
#if FALSE
using System;
using System.Collections.Generic;
using System.Linq;
using Dx.Runtime;
using Ninject;

namespace Protogame
{
    // <summary>
    /// An implementation of an asset manager where the assets can be
    /// modified and changed over the network (usually by the asset
    /// manager program).
    /// </summary>
    [Distributed]
    public class NetworkAssetManager : IAssetManager
    {
        public bool IsRemoting { get { return true; } }

        [ClientCallable]
        public bool IsReady()
        {
            if (this.m_Kernel == null)
                return false;
            return this.m_Kernel.TryGet<IRawAssetLoader>() != null;
        }

        [Local]
        private IRawAssetLoader m_RawAssetLoader;

        [Local]
        private IRawAssetSaver m_RawAssetSaver;

        [Local]
        private ITransparentAssetCompiler m_TransparentAssetCompiler;

        [Local]
        private IKernel m_Kernel;

        [Local]
        private Dictionary<string, NetworkAsset> m_Assets;

        [Local]
        private Dictionary<string, object[]> m_RawAssets;
        
        [Local]
        private Dictionary<string, IAsset> m_ClientCache;

        public NetworkAssetManager()
        {
            this.m_Assets = new Dictionary<string, NetworkAsset>();
            this.m_RawAssets = new Dictionary<string, object[]>();
        }
        
        [Local]
        public void SetKernel(IKernel kernel)
        {
            this.m_Kernel = kernel;
        }

        public void Dirty(string asset)
        {
            lock (this.m_Assets)
            {
                this.m_Assets[asset].Dirty();
            }
        }

        [Local]
        public IAsset[] GetAll()
        {
            if ((this as ITransparent).Node.IsServer)
            {
                lock (this.m_Assets)
                {
                    return this.m_Assets.Values.ToArray();
                }
            }
            
            if (this.m_ClientCache == null)
                this.m_ClientCache = new Dictionary<string, IAsset>();
            return this.m_ClientCache.Values.ToArray();
        }

        [ClientCallable]
        public IAsset GetUnresolved(string asset)
        {
            lock (this.m_Assets)
            {
                // If the network asset already exists, return it.
                if (this.m_Assets.ContainsKey(asset))
                {
                    if (!this.m_Assets[asset].IsDirty)
                        return this.m_Assets[asset];
                    this.m_Assets.Remove(asset);
                }

                // Otherwise load the raw assets if that doesn't exist.
                object[] candidates;
                NetworkAsset result;
                if (!this.m_RawAssets.ContainsKey(asset))
                {
                    if (this.m_RawAssetLoader == null)
                        this.m_RawAssetLoader = this.m_Kernel.Get<IRawAssetLoader>();
                    try
                    {
                        candidates = this.m_RawAssetLoader.LoadRawAssetCandidates(asset);
                    }
                    catch (AssetNotFoundException)
                    {
                        result = new NetworkAsset(this.m_Kernel.GetAll<IAssetLoader>().ToArray(), this.m_TransparentAssetCompiler, null, asset, this);
                        this.m_Assets.Add(asset, result);
                        return result;
                    }
                }
                else
                    candidates = this.m_RawAssets[asset];

                // We now have our raw asset and we need to wrap it in
                // a NetworkAsset.
                result = new NetworkAsset(this.m_Kernel.GetAll<IAssetLoader>().ToArray(), this.m_TransparentAssetCompiler, candidates, asset, this);
                this.m_Assets.Add(asset, result);
                return result;
            }
        }

        [Local]
        public T Get<T>(string asset) where T : class, IAsset
        {
            // Calling GetUnresolved from the client is an expensive operation,
            // so we do everything we can to cache the result.  The NetworkAsset
            // proxy magic means that we'll automatically know on the client side
            // when we need to pull down new data, and this is handled inside
            // the NetworkAssetProxy.  Thus we can cache our resolved asset
            // forever.
            if (!(this as ITransparent).Node.IsServer)
            {
                // If the network asset already exists, return it.
                if (this.m_ClientCache == null)
                    this.m_ClientCache = new Dictionary<string, IAsset>();
                if (this.m_ClientCache.ContainsKey(asset))
                    return (T)this.m_ClientCache[asset];
                    
                // Otherwise we really do need to make a request over
                // the network to get an initial NetworkAsset.
                NetworkAsset networkAsset = null;
                while (networkAsset == null)
                    networkAsset = (NetworkAsset)this.GetUnresolved(asset);
                networkAsset.InjectLoaders(this.m_Kernel.GetAll<IAssetLoader>().ToArray());
                this.m_ClientCache.Add(asset, networkAsset.Resolve<T>());
                return (T)this.m_ClientCache[asset];
            }
            
            // We can efficiently call this on the server as much as we like.
            return this.GetUnresolved(asset).Resolve<T>();
        }
        
        [Local]
        public T TryGet<T>(string asset) where T : class, IAsset
        {
            try
            {
                return this.Get<T>(asset);
            }
            catch (AssetNotFoundException)
            {
                return null;
            }
        }
        
        [Local]
        public void Save(IAsset asset)
        {
            // If the client saves an asset, it's usually a client-side generated
            // asset (like a texture atlas calculated when the game is loaded).  Therefore
            // we just store it in the client cache and don't attempt to send it to the
            // network asset manager (since the data associated with the asset won't be
            // implicitly serializable).
            if (!(this as ITransparent).Node.IsServer)
            {
                this.m_ClientCache[asset.Name] = asset;
                return;
            }
            
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
                    var result = saver.Handle(asset, AssetTarget.Runtime);
                    if (result == null)
                    {
                        // We can handle this asset; but we explicitly do not want to save it.
                        // This is used when we load a raw asset from a PNG, and we don't want to
                        // save a .asset file back to disk (because then we'll have two matching
                        // assets in the asset folder, both representing the exact same texture).
                        return;
                    }

                    this.m_RawAssets[asset.Name] = result;
                    this.m_Assets[asset.Name].Dirty();
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

        [Local]
        public void Recompile(IAsset asset)
        {
            this.m_TransparentAssetCompiler.Handle(asset, true);
        }
    }
}

#endif