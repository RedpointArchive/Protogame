using System;
using Protogame;
using Microsoft.Xna.Framework;

namespace ProtogameEditor
{
    public class CubeEntity : Entity, ISerializableEntity
    {
        private readonly I3DRenderUtilities _renderUtilities;

        public CubeEntity(I3DRenderUtilities renderUtilities)
        {
            _renderUtilities = renderUtilities;
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                _renderUtilities.RenderCube(
                    renderContext,
                    FinalTransform.AbsoluteMatrix,
                    new Color(194, 194, 194));
            }
        }

        public string Text
        {
            get
            {
                return "cube entity";
            }
            set
            {
            }
        }
    }
}

