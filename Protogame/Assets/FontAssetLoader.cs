using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Protogame
{
    public class FontAssetLoader : IAssetLoader
    {
        private IAssetContentManager m_AssetContentManager;
        private IContentCompiler m_ContentCompiler;
        
        public FontAssetLoader(
            IAssetContentManager assetContentManager,
            IContentCompiler contentCompiler)
        {
            this.m_AssetContentManager = assetContentManager;
            this.m_ContentCompiler = contentCompiler;
        }
    
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(FontAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            byte[] xnaData = null;
            if (data.FontData != null)
                xnaData = ((JArray)data.FontData).Select(x => (byte)x).ToArray();
            return new FontAsset(
                this.m_ContentCompiler,
                this.m_AssetContentManager,
                name,
                (string)data.FontName,
                (int)data.FontSize,
                xnaData);
        }

        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            return null;
        }
        
        public bool CanNew()
        {
            return true;
        }
        
        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return new FontAsset(
                this.m_ContentCompiler,
                this.m_AssetContentManager,
                name,
                "",
                0,
                null);
        }
    }
}

