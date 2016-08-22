using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicHorizontalContainerSkinRenderer : ISkinRenderer<HorizontalContainer>
    {
        public void Render(IRenderContext renderContext, Rectangle layout, HorizontalContainer horizontalContainer)
        {
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, HorizontalContainer container)
        {
            throw new NotSupportedException();
        }
    }
}
