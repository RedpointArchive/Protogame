using System;
using System.Collections.Generic;
using System.Linq;

namespace Protogame
{
    public class EffectAssetLoader : IAssetLoader
    {
        private IAssetContentManager m_AssetContentManager;

        public EffectAssetLoader(
            IAssetContentManager assetContentManager)
        {
            this.m_AssetContentManager = assetContentManager;
        }
    
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(EffectAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            if (data is CompiledAsset)
            {
                return new EffectAsset(
                    this.m_AssetContentManager,
                    name,
                    null,
                    data.PlatformData,
                    data.SourcedFromRaw != null && (bool)data.SourcedFromRaw);
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

            var effect = new EffectAsset(
                this.m_AssetContentManager,
                name,
                (string)data.Code,
                platformData,
                data.SourcedFromRaw != null && (bool)data.SourcedFromRaw);

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
            return new EffectAsset(
                this.m_AssetContentManager,
                name,
                "",
                null,
                false);
        }
    }
}

