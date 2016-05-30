using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Protoinject;

namespace Protogame
{
    public class UnifiedShaderAsset : MarshalByRefObject, IAsset
    {
        private readonly IKernel _kernel;
        private readonly IAssetContentManager _assetContentManager;

        public UnifiedShaderAsset(
            IKernel kernel,
            IAssetContentManager assetContentManager, 
            string name,
            string code,
            PlatformData platformData, 
            bool sourcedFromRaw)
        {
            _kernel = kernel;
            _assetContentManager = assetContentManager;
            Name = name;
            Code = code;
            PlatformData = platformData;
            SourcedFromRaw = sourcedFromRaw;

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

        private void ReloadEffect()
        {
            // FIXME: We shouldn't be casting IAssetContentManager like this!
            var assetContentManager = _assetContentManager as AssetContentManager;
            if (assetContentManager == null)
            {
                throw new NoAssetContentManagerException();
            }

            var serviceProvider = assetContentManager.ServiceProvider;
            var graphicsDeviceProvider =
                (IGraphicsDeviceService)serviceProvider.GetService(typeof(IGraphicsDeviceService));
            if (graphicsDeviceProvider != null && graphicsDeviceProvider.GraphicsDevice != null)
            {
                var graphicsDevice = graphicsDeviceProvider.GraphicsDevice;

                var compiledUnifiedShaderReader = new CompiledUnifiedShaderReader(this.PlatformData.Data);

                // Use the new EffectWithSemantics class that allows for extensible semantics.
                var availableSemantics = _kernel.GetAll<IEffectSemantic>();
                this.Effect = new EffectWithSemantics(graphicsDevice, compiledUnifiedShaderReader, availableSemantics);

                // Assign the asset name so we can trace it back.
                this.Effect.Name = this.Name;
            }
        }

        public PlatformData PlatformData { get; set; }

        public string Code { get; set; }

        public bool SourcedFromRaw { get; set; }

        public bool CompiledOnly => Code == null;

        public Effect Effect { get; set; }

        public string Name { get; }

        public bool SourceOnly => PlatformData == null;

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(UnifiedShaderAsset)))
            {
                return this as T;
            }

            throw new InvalidOperationException("Asset already resolved to UnifiedShaderAsset.");
        }
    }
}
