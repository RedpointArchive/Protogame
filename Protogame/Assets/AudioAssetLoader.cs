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

        public IAsset Handle(string name, dynamic data)
        {
            byte[] xnaData = null;
            if (data.AudioData != null)
                xnaData = ((List<object>)data.AudioData)
                    .Cast<int>().Select(x => (byte)x).ToArray();
            return new AudioAsset(
                //this.m_ContentCompiler,
                //this.m_AssetContentManager,
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
            return new AudioAsset(
                //this.m_ContentCompiler,
                //this.m_AssetContentManager,
                name,
                "",
                null);
        }
    }
}

