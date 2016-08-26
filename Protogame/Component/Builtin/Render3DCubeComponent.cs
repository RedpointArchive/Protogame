using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class Render3DCubeComponent : IRenderableComponent, IEnabledComponent, IHasTransform
    {
        private readonly INode _node;

        private readonly I3DRenderUtilities _renderUtilities;

        public Render3DCubeComponent(INode node, I3DRenderUtilities renderUtilities, IAssetManagerProvider assetManagerProvider)
        {
            _node = node;
            _renderUtilities = renderUtilities;

            Enabled = true;
            Effect = assetManagerProvider.GetAssetManager().Get<UberEffectAsset>("effect.BuiltinSurface").Effects?["Color"];
            Transform = new DefaultTransform();
        }

        public Color Color { get; set; }

        public IEffect Effect { get; set; }

        public bool Enabled { get; set; }

        public ITransform Transform { get; }

        public IFinalTransform FinalTransform => this.GetAttachedFinalTransformImplementation(_node);

        public void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext)
        {
            if (!Enabled)
            {
                return;
            }

            if (renderContext.IsCurrentRenderPass<I3DRenderPass>())
            {
                _renderUtilities.RenderCube(
                    renderContext,
                    Effect,
                    Effect.CreateParameterSet(),
                    Matrix.CreateTranslation(-0.5f, -0.5f, -0.5f) *
                    FinalTransform.AbsoluteMatrix, 
                    Color);
            }
        }
    }
}