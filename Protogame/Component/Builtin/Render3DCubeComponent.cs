using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class Render3DCubeComponent : IRenderableComponent, IEnabledComponent
    {
        private readonly INode _node;

        private readonly I3DRenderUtilities _renderUtilities;

        public Render3DCubeComponent(INode node, I3DRenderUtilities renderUtilities)
        {
            _node = node;
            _renderUtilities = renderUtilities;

            Enabled = true;
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
                if (Effect != null)
                {
                    renderContext.PushEffect(Effect.Effect);
                }

                var matrix = Matrix.Identity;
                var matrixComponent = _node.Parent?.UntypedValue as IHasTransform;
                if (matrixComponent != null)
                {
                    matrix *= matrixComponent.FinalTransform.AbsoluteMatrix;
                }
                _renderUtilities.RenderCube(
                    renderContext, 
                    Matrix.CreateTranslation(-0.5f, -0.5f, -0.5f) *
                    matrix, 
                    Color);

                if (Effect != null)
                {
                    renderContext.PopEffect();
                }
            }
        }
    }
}