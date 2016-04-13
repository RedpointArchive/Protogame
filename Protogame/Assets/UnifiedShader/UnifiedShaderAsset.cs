using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class UnifiedShaderAsset : MarshalByRefObject, IAsset
    {
        private readonly IAssetContentManager _assetContentManager;

        public UnifiedShaderAsset(IAssetContentManager assetContentManager, string name, string code, PlatformData platformData, bool sourcedFromRaw)
        {
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

                // Load the effect for the first time.
                var effect = new Effect(graphicsDevice, compiledUnifiedShaderReader);

                // Determine what kind of effect class we should use.
                var hasSeperatedMatrixes = effect.Parameters["World"] != null && effect.Parameters["View"] != null && effect.Parameters["Projection"] != null;
                var hasMatrix = effect.Parameters["WorldViewProj"] != null || hasSeperatedMatrixes;
                var hasTexture = effect.Parameters["Texture"] != null;
                var hasBones = effect.Parameters["Bones"] != null;
                if (hasMatrix && hasTexture && hasBones)
                {
                    this.Effect = new EffectWithMatricesAndTextureAndBones(graphicsDevice, this.PlatformData.Data, hasSeperatedMatrixes);
                }
                else if (hasMatrix && hasTexture)
                {
                    this.Effect = new EffectWithMatricesAndTexture(graphicsDevice, this.PlatformData.Data, hasSeperatedMatrixes);
                }
                else if (hasMatrix)
                {
                    this.Effect = new EffectWithMatrices(graphicsDevice, this.PlatformData.Data, hasSeperatedMatrixes);
                }
                else if (hasTexture)
                {
                    this.Effect = new EffectWithTexture(graphicsDevice, this.PlatformData.Data);
                }
                else
                {
                    this.Effect = effect;
                }

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
