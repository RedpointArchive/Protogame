using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class Render3DCubeComponent : IRenderableComponent, IEnabledComponent, IHasTransform
    {
        private readonly INode _node;

        private readonly IFinalTransform _finalTransform;

        private readonly I3DRenderUtilities _renderUtilities;

        private IAssetReference<UberEffectAsset> _defaultSurfaceEffect;

        public Render3DCubeComponent(INode node, I3DRenderUtilities renderUtilities, IAssetManager assetManager)
        {
            _node = node;
            _finalTransform = new DefaultFinalTransform(this, node);
            _renderUtilities = renderUtilities;
            _defaultSurfaceEffect = assetManager.Get<UberEffectAsset>("effect.BuiltinSurface");

            Enabled = true;
            Transform = new DefaultTransform();
        }

        public Color Color { get; set; }

        public IEffect Effect { get; set; }

        public bool Enabled { get; set; }

        public ITransform Transform { get; }

        public IFinalTransform FinalTransform => _finalTransform;

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

                _renderUtilities.RenderCube(
                    renderContext,
                    effect,
                    effect.CreateParameterSet(),
                    Matrix.CreateTranslation(-0.5f, -0.5f, -0.5f) *
                    FinalTransform.AbsoluteMatrix, 
                    Color);
            }
        }
    }
}