using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicAdjustedContainerSkinRenderer : ISkinRenderer<AdjustedContainer>
    {
        public void Render(IRenderContext renderContext, Rectangle layout, AdjustedContainer container)
        {
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, AdjustedContainer container)
        {
            throw new NotSupportedException();
        }
    }
}
