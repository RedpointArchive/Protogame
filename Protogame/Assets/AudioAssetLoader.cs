using System;
using System.Collections.Generic;
using System.Linq;

namespace Protogame
{
    public class AudioAssetLoader : IAssetLoader
    {
        private IAssetContentManager m_AssetContentManager;
        private IContentCompiler m_ContentCompiler;
        
        public AudioAssetLoader(
            IAssetContentManager assetContentManager,
            IContentCompiler contentCompiler)
        {
            this.m_AssetContentManager = assetContentManager;
            this.m_ContentCompiler = contentCompiler;
        }
    
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(AudioAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            byte[] xnaData = null;
            if (data.AudioData != null)
                xnaData = ((List<object>)data.AudioData)
                    .Cast<int>().Select(x => (byte)x).ToArray();
            return new AudioAsset(
                name,
                data.SourcePath,
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
            return new AudioAsset(
                name,
                "",
                null);
        }
    }
}

