using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class EffectAsset : MarshalByRefObject, IAsset
    {
        private IAssetContentManager m_AssetContentManager;
        public string Name { get; private set; }
        public PlatformData PlatformData { get; set; }
        public string Code { get; set; }
        public Effect Effect { get; private set; }

        public EffectAsset(
            IAssetContentManager assetContentManager,
            string name,
            string code,
            PlatformData platformData)
        {
            this.Name = name;
            this.Code = code;
            this.PlatformData = platformData;
            this.m_AssetContentManager = assetContentManager;

            if (this.PlatformData != null)
            {
                try
                {
                    this.ReloadEffect();
                }
                catch (NoAssetContentManagerException)
                {
                }
            }
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
                return this.Code == null;
            }
        }

        public void ReloadEffect()
        {
            if (this.m_AssetContentManager == null)
                throw new InvalidOperationException("Asset content manager is not present.");
            if (this.m_AssetContentManager != null && this.PlatformData != null)
            {
                // We don't load XNB-based effects because MonoGame doesn't have a reader
                // that supports them, and the XNB format mangles the shader bytecode if we
                // try and parse it out manually.  We also can't just define our own effect reader
                // as that doesn't seem to get picked up by the MonoGame content manager.
                
                // FIXME: We shouldn't be casting IAssetContentManager like this!
                var assetContentManager = this.m_AssetContentManager as AssetContentManager;
                if (assetContentManager == null)
                {
                    throw new NoAssetContentManagerException();
                }

                var serviceProvider = assetContentManager.ServiceProvider;
                var graphicsDeviceProvider = (IGraphicsDeviceService)serviceProvider.GetService(typeof(IGraphicsDeviceService));
                if (graphicsDeviceProvider != null && graphicsDeviceProvider.GraphicsDevice != null)
                {
                    var graphicsDevice = graphicsDeviceProvider.GraphicsDevice;

                    // Load the effect for the first time.
                    var effect = new Effect(graphicsDevice, this.PlatformData.Data);

                    // Determine what kind of effect class we should use.
                    var hasMatrix = effect.Parameters["WorldViewProj"] != null;
                    var hasTexture = effect.Parameters["Texture"] != null;
                    if (hasMatrix && hasTexture)
                    {
                        this.Effect = new EffectWithMatricesAndTexture(graphicsDevice, this.PlatformData.Data);
                    }
                    else if (hasMatrix)
                    {
                        this.Effect = new EffectWithMatrices(graphicsDevice, this.PlatformData.Data);
                    }
                    else if (hasTexture)
                    {
                        this.Effect = new EffectWithTexture(graphicsDevice, this.PlatformData.Data);
                    }
                    else
                    {
                        this.Effect = effect;
                    }
                }
            }
        }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(EffectAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to EffectAsset.");
        }
    }
}

