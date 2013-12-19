using System;
using System.Collections.Generic;
using System.Linq;

namespace Protogame
{
    public class EffectAssetLoader : IAssetLoader
    {
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(EffectAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            if (data is CompiledAsset)
            {
                return new EffectAsset(
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

            var effect = new EffectAsset(
                name,
                (string)data.SourcePath,
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
            return new EffectAsset(
                name,
                "",
                null);
        }
    }
}

