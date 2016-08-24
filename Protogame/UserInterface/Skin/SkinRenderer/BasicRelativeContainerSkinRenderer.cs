using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicRelativeContainerSkinRenderer : ISkinRenderer<RelativeContainer>
    {
        public void Render(IRenderContext renderContext, Rectangle layout, RelativeContainer container)
        {
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, RelativeContainer container)
        {
            throw new NotSupportedException();
        }
    }
}
