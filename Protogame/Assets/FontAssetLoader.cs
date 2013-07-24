using System;
using System.Collections.Generic;
using System.Linq;

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

        public IAsset Handle(string name, dynamic data)
        {
            var xnaData = ((List<object>)data.FontData)
                .Cast<int>().Select(x => (byte)x).ToArray();
            return new FontAsset(
                this.m_ContentCompiler,
                this.m_AssetContentManager,
                name,
                data.FontName,
                data.FontSize,
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
            return new FontAsset(
                this.m_ContentCompiler,
                this.m_AssetContentManager,
                name,
                "Arial",
                12,
                null);
        }
    }
}

