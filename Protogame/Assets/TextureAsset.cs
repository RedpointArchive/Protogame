using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class TextureAsset : MarshalByRefObject, IAsset
    {
        private IAssetContentManager m_AssetContentManager;
        public string Name { get; private set; }
        public Texture2D Texture { get; private set; }
        public byte[] RawData { get; set; }
        public PlatformData PlatformData { get; set; }

        public TextureAsset(
            IAssetContentManager assetContentManager,
            string name,
            byte[] rawData,
            PlatformData data)
        {
            this.Name = name;
            this.RawData = rawData;
            this.PlatformData = data;
            this.m_AssetContentManager = assetContentManager;

            if (this.PlatformData != null)
            {
                this.ReloadTexture();
            }
        }

        public bool SourceOnly
        {
            get
            {
                return this.PlatformData == null;
            }
        }

        public bool CompiledOnly
        {
            get
            {
                return this.RawData == null;
            }
        }
        
        /// <summary>
        /// Create a new TextureAsset from a in-memory texture.  Used when you need to pass in a 
        /// generated texture into a function that accepts a TextureAsset.  TextureAssets created
        /// with this constructor can not be saved or baked by an asset manager.
        /// </summary>
        public TextureAsset(Texture2D texture)
        {
            if (texture == null)
                throw new ArgumentNullException("texture");
            this.Name = null;
            this.Texture = texture;
            this.RawData = null;
            this.PlatformData = null;
            this.m_AssetContentManager = null;
        }
        
        public void ReloadTexture()
        {
            if (this.m_AssetContentManager == null)
                throw new InvalidOperationException("Unable to reload dynamic texture.");
            if (this.m_AssetContentManager != null && this.PlatformData != null)
            {
                using (var stream = new MemoryStream(this.PlatformData.Data))
                {
                    this.m_AssetContentManager.SetStream(this.Name, stream);
                    this.m_AssetContentManager.Purge(this.Name);
                    var newTexture = this.m_AssetContentManager.Load<Texture2D>(this.Name);
                    if (newTexture != null)
                        this.Texture = newTexture;
                    this.m_AssetContentManager.UnsetStream(this.Name);
                }
            }
        }
        
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(TextureAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to TextureAsset.");
        }
    }
}

