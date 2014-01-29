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
            if (data is CompiledAsset)
            {
                return new AudioAsset(
                    name,
                    null,
                    data.PlatformData,
                    false);
            }

            PlatformData platformData = null;
            if (data.PlatformData != null)
            {
                platformData = new PlatformData
                {
                    Platform = data.PlatformData.Platform,
                    Data = data.PlatformData.Data
                };
            }

            var audio = new AudioAsset(
                name,
                ByteReader.ReadAsByteArray(data.RawData),
                platformData,
                data.SourcedFromRaw != null && (bool)data.SourcedFromRaw);

            return audio;
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
                null,
                null,
                false);
        }
    }
}

