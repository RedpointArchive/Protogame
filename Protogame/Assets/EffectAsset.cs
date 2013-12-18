using System;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class EffectAsset : MarshalByRefObject, IAsset
    {
        public string Name { get; private set; }
        public PlatformData PlatformData { get; set; }
        public string SourcePath { get; set; }
        public Effect Effect { get; private set; }

        public EffectAsset(
            string name,
            string sourcePath,
            PlatformData platformData)
        {
            this.Name = name;
            this.SourcePath = sourcePath;
            this.PlatformData = platformData;
        }

        public bool SourceOnly
        {
            get
            {
                return this.PlatformData == null;
            }
        }

        public bool CompiledOnly
        {
            get
            {
                return this.SourcePath == null;
            }
        }
        
        public Effect LoadEffect(GraphicsDevice device)
        {
            if (this.PlatformData == null)
            {
                throw new AssetNotCompiledException("Effect has not been compiled.");
            }

            this.Effect = new Effect(device, this.PlatformData.Data);
            if (this.Effect == null)
                throw new InvalidOperationException("Unable to load effect.");

            return this.Effect;
        }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(EffectAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to EffectAsset.");
        }
    }
}

