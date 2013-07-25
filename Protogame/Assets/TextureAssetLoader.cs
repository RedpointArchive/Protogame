using System;
using System.Collections.Generic;
using System.Linq;

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

        public IAsset Handle(string name, dynamic data)
        {
            byte[] xnaData = null;
            if (data.TextureData != null)
                xnaData = ((List<object>)data.TextureData)
                    .Cast<int>().Select(x => (byte)x).ToArray();
            return new TextureAsset(
                this.m_ContentCompiler,
                this.m_AssetContentManager,
                name,
                data.SourcePath,
                xnaData);
        }

        public IAsset GetDefault(string name)
        {
            throw new InvalidOperationException();
        }
        
        public bool CanNew()
        {
            return true;
        }
        
        public IAsset GetNew(string name)
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

