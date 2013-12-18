using System;
using System.Collections.Generic;
using System.Linq;

namespace Protogame
{
    public class AudioAssetLoader : IAssetLoader
    {
        private IAssetContentManager m_AssetContentManager;
        
        public AudioAssetLoader(
            IAssetContentManager assetContentManager)
        {
            this.m_AssetContentManager = assetContentManager;
        }
    
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(AudioAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            PlatformData platformData = null;
            if (data.PlatformData != null)
            {
                platformData = new PlatformData
                {
                    Platform = data.PlatformData.Platform,
                    Data = data.PlatformData.Data
                };
            }

            var texture = new AudioAsset(
                name,
                (string)data.SourcePath,
                platformData);

            return texture;
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

