using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Protogame
{
    public class FontAssetLoader : IAssetLoader
    {
        private IAssetContentManager m_AssetContentManager;
        
        public FontAssetLoader(
            IAssetContentManager assetContentManager)
        {
            this.m_AssetContentManager = assetContentManager;
        }
    
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(FontAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            if (data is CompiledAsset)
            {
                return new FontAsset(
                    this.m_AssetContentManager,
                    name,
                    null,
                    0,
                    false,
                    0,
                    data.PlatformData);
            }

            PlatformData platformData = null;
            if (data.PlatformData != null)
            {
                platformData = new PlatformData
                {
                    Platform = data.PlatformData.Platform,
                    Data = ByteReader.ReadAsByteArray(data.PlatformData.Data)
                };
            }

            var effect = new FontAsset(
                this.m_AssetContentManager,
                name,
                (string)data.FontName,
                (int)data.FontSize,
                (bool)data.UseKerning,
                (int)data.Spacing,
                platformData);

            return effect;
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
                this.m_AssetContentManager,
                name,
                "",
                0,
                true,
                0,
                null);
        }
    }
}

