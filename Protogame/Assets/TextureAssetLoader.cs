using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Protogame
{
    public class TextureAssetLoader : IAssetLoader
    {
        private IAssetContentManager m_AssetContentManager;
        
        public TextureAssetLoader(
            IAssetContentManager assetContentManager)
        {
            this.m_AssetContentManager = assetContentManager;
        }
    
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(TextureAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            if (data is CompiledAsset)
            {
                return new TextureAsset(
                    this.m_AssetContentManager,
                    name,
                    null,
                    data.PlatformData);
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

            var texture = new TextureAsset(
                this.m_AssetContentManager,
                name,
                ByteReader.ReadAsByteArray(data.RawData),
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
            return new TextureAsset(
                this.m_AssetContentManager,
                name,
                null,
                null);
        }
    }
}

