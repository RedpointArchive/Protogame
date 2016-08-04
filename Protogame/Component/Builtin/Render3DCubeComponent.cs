using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class Render3DCubeComponent : IRenderableComponent, IEnabledComponent
    {
        private readonly INode _node;

        private readonly I3DRenderUtilities _renderUtilities;

        public Render3DCubeComponent(INode node, I3DRenderUtilities renderUtilities, IAssetManagerProvider assetManagerProvider)
        {
            _node = node;
            _renderUtilities = renderUtilities;

            Enabled = true;
            Effect = assetManagerProvider.GetAssetManager().Get<EffectAsset>("effect.Color");
        }

        public Color Color { get; set; }

        public EffectAsset Effect { get; set; }

        public bool Enabled { get; set; }

        public void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext)
        {
            if (!Enabled)
            {
                return;
            }

            if (renderContext.IsCurrentRenderPass<I3DRenderPass>())
            {
                var matrix = Matrix.Identity;
                var matrixComponent = _node.Parent?.UntypedValue as IHasTransform;
                if (matrixComponent != null)
                {
                    matrix *= matrixComponent.FinalTransform.AbsoluteMatrix;
                }
                _renderUtilities.RenderCube(
                    renderContext,
                    Effect.Effect,
                    Effect.Effect.CreateParameterSet(),
                    Matrix.CreateTranslation(-0.5f, -0.5f, -0.5f) *
                    matrix, 
                    Color);
            }
        }
    }
}