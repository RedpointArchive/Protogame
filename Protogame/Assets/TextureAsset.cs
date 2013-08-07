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
        
        public TextureAsset(
            IContentCompiler contentCompiler,
            IAssetContentManager assetContentManager,
            string name,
            Texture2D texture)
        {
            if (texture == null)
                throw new ArgumentNullException("texture");
            this.Name = name;
            this.Texture = texture;
            this.SourcePath = null;
            this.Data = null;
            this.m_AssetContentManager = assetContentManager;
            this.m_ContentCompiler = contentCompiler;
            
            // This doesn't work because MonoGame doesn't support SaveAsPng yet.
            /*
            using (var memory = new MemoryStream())
            {
                this.Texture.SaveAsPng(memory, this.Texture.Width, this.Texture.Height);
                memory.Read(this.Data, 0, (int)memory.Position);
            }
            this.RebuildTexture();
            */
        }
        
        public void ReloadTexture()
        {
            if (this.m_AssetContentManager != null && this.Data != null)
            {
                using (var stream = new MemoryStream(this.Data))
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
        
        public void RebuildTexture()
        {
            try
            {
                if (this.m_ContentCompiler != null)
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
            }
            catch (Exception ex)
            {
                // This is the type of exception that our dummy content compiler throws.
                if (ex is NotImplementedException)
                    throw;
                
                // The developer may have supplied an already built XNB asset.
                var reloaded = false;
                try
                {
                    this.ReloadTexture();
                    reloaded = true;
                }
                catch { }
                
                // If we weren't able to reload the texture, then we really didn't rebuild
                // correct, so rethrow the original exception.
                if (!reloaded)
                    throw;
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

