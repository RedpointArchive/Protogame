using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class Render3DPlaneComponent : IRenderableComponent, IEnabledComponent
    {
        private readonly INode _node;

        private readonly I3DRenderUtilities _renderUtilities;

        private readonly IAssetReference<UberEffectAsset> _defaultSurfaceEffect;

        public Render3DPlaneComponent(INode node, I3DRenderUtilities renderUtilities, IAssetManager assetManager)
        {
            _node = node;
            _renderUtilities = renderUtilities;
            _defaultSurfaceEffect = assetManager.Get<UberEffectAsset>("effect.BuiltinSurface");

            Enabled = true;
        }

        public Color Color { get; set; }

        public IEffect Effect { get; set; }

        public bool Enabled { get; set; }

        public void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext)
        {
            if (!Enabled)
            {
                return;
            }

            if (renderContext.IsCurrentRenderPass<I3DRenderPass>())
            {
                IEffect effect;
                if (Effect != null)
                {
                    effect = Effect;
                }
                else if (_defaultSurfaceEffect.IsReady)
                {
                    effect = _defaultSurfaceEffect.Asset.Effects?["Color"];
                }
                else
                {
                    return;
                }

                if (effect == null)
                {
                    return;
                }

                var matrix = Matrix.Identity;
                var matrixComponent = _node.Parent?.UntypedValue as IHasTransform;
                if (matrixComponent != null)
                {
                    matrix *= matrixComponent.FinalTransform.AbsoluteMatrix;
                }
                _renderUtilities.RenderPlane(renderContext, effect, effect.CreateParameterSet(), matrix, Color);
            }
        }
    }
}