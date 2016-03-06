using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class Render3DPlaneComponent : IRenderableComponent
    {
        private readonly INode _node;

        private readonly I3DRenderUtilities _renderUtilities;

        public Render3DPlaneComponent(INode node, I3DRenderUtilities renderUtilities)
        {
            _node = node;
            _renderUtilities = renderUtilities;
        }

        public Color Color { get; set; }

        public void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext)
        {
            var matrix = Matrix.Identity;
            var matrixComponent = _node.Parent?.UntypedValue as IHasMatrix;
            if (matrixComponent != null)
            {
                matrix *= matrixComponent.GetFinalMatrix();
            }
            _renderUtilities.RenderPlane(renderContext, matrix, Color);
        }
    }
}