using System;
using System.Collections.Generic;
using System.Linq;

namespace Protogame
{
    public class EffectAssetLoader : IAssetLoader
    {
        private IAssetContentManager m_AssetContentManager;
        private IContentCompiler m_ContentCompiler;
        
        public EffectAssetLoader(
            IAssetContentManager assetContentManager,
            IContentCompiler contentCompiler)
        {
            this.m_AssetContentManager = assetContentManager;
            this.m_ContentCompiler = contentCompiler;
        }
    
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(EffectAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            byte[] glXnaData = null;
            byte[] dxXnaData = null;
            if (data.GLEffectData != null)
                glXnaData = ((List<object>)data.GLEffectData)
                    .Cast<int>().Select(x => (byte)x).ToArray();
            if (data.DXEffectData != null)
                glXnaData = ((List<object>)data.DXEffectData)
                    .Cast<int>().Select(x => (byte)x).ToArray();
            return new EffectAsset(
                name,
                data.SourcePath,
                glXnaData,
                dxXnaData);
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
            return new EffectAsset(
                name,
                "",
                null,
                null);
        }
    }
}

