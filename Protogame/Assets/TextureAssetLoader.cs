using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Protogame
{
    public class TextureAssetLoader : IAssetLoader
    {
        private IAssetContentManager m_AssetContentManager;
        private IContentCompiler m_ContentCompiler;
        
        public TextureAssetLoader(
            IAssetContentManager assetContentManager,
            IContentCompiler contentCompiler)
        {
            this.m_AssetContentManager = assetContentManager;
            this.m_ContentCompiler = contentCompiler;
        }
    
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(TextureAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            byte[] xnaData = null;
            if (data.TextureData != null && data.TextureData is JArray)
                xnaData = ((JArray)data.TextureData).Select(x => (byte)x).ToArray();
            var texture = new TextureAsset(
                this.m_ContentCompiler,
                this.m_AssetContentManager,
                name,
                (string)data.SourcePath,
                xnaData);
            #if DEBUG
            if (data.TextureData != null && data.TextureData is byte[])
            {
                texture.Data = (byte[])data.TextureData;
                texture.RebuildTexture();
            }
            #endif
            return texture;
        }

        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            throw new InvalidOperationException();
        }
        
        public bool CanNew()
        {
            return true;
        }
        
        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return new TextureAsset(
                this.m_ContentCompiler,
                this.m_AssetContentManager,
                name,
                "",
                null);
        }
    }
}

