using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class TextureAsset : MarshalByRefObject, IAsset
    {
        private IAssetContentManager m_AssetContentManager;
        private IContentCompiler m_ContentCompiler;
        public string Name { get; private set; }
        public Texture2D Texture { get; private set; }
        public byte[] Data { get; set; }
        public string SourcePath { get; set; }

        public TextureAsset(
            IContentCompiler contentCompiler,
            IAssetContentManager assetContentManager,
            string name,
            string sourcePath,
            byte[] data)
        {
            this.Name = name;
            this.SourcePath = sourcePath;
            this.Data = data;
            this.m_AssetContentManager = assetContentManager;
            this.m_ContentCompiler = contentCompiler;
            
            this.ReloadTexture();
        }
        
        public void ReloadTexture()
        {
            if (this.m_AssetContentManager != null && this.Data != null)
            {
                using (var stream = new MemoryStream(this.Data))
                {
                    this.m_AssetContentManager.SetStream(this.Name, stream);
                    this.m_AssetContentManager.Purge(this.Name);
                    this.Texture = this.m_AssetContentManager.Load<Texture2D>(this.Name);
                    this.m_AssetContentManager.UnsetStream(this.Name);
                }
            }
        }
        
        public void RebuildTexture()
        {
            using (var stream = new MemoryStream(this.Data))
            {
                var data = this.m_ContentCompiler.BuildTexture2D(stream);
                if (data != null)
                {
                    this.Data = data;
                    this.ReloadTexture();
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

