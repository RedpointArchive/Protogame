using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace Protogame
{    
    public class AssetContentManager : ContentManager, IAssetContentManager
    {
        private readonly Dictionary<string, Stream> _memoryStreams = new Dictionary<string, Stream>();
        
        public AssetContentManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }
        
        public override T Load<T>(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new ArgumentNullException("assetName");
            }

            T result = default(T);

            // Check for a previously loaded asset first
            object asset = null;
            if (this.LoadedAssets.TryGetValue(assetName, out asset))
            {
                if (asset is T)
                {
                    return (T)asset;
                }
            }

            // Load the asset.
            result = this.ReadAsset<T>(assetName, x => { });

            this.LoadedAssets[assetName] = result;
            return result;
        }
        
        public void Purge(string assetName)
        {
            if (!this.LoadedAssets.ContainsKey(assetName))
            {
                return;
            }

            var asset = this.LoadedAssets[assetName];
            if (asset is IDisposable)
            {
                (asset as IDisposable).Dispose();
            }

            this.LoadedAssets.Remove(assetName);
        }
        
        public void SetStream(string assetName, Stream stream)
        {
            this._memoryStreams[assetName] = stream;
        }
        
        public void UnsetStream(string assetName)
        {
#if !PLATFORM_ANDROID
            this._memoryStreams[assetName] = null;
#endif
        }
        
        protected override Stream OpenStream(string assetName)
        {
#if PLATFORM_ANDROID
            // We have to make a copy on Android so we can reload assets.
            var copy = new MemoryStream();
            this._memoryStreams[assetName].Seek(0, SeekOrigin.Begin);
            this._memoryStreams[assetName].CopyTo(copy);
            copy.Seek(0, SeekOrigin.Begin);
            return copy;
#else
            return this._memoryStreams[assetName];
#endif
        }
    }
}