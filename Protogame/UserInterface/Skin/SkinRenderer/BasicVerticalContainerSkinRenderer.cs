using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicVerticalContainerSkinRenderer : ISkinRenderer<VerticalContainer>
    {
        public void Render(IRenderContext renderContext, Rectangle layout, VerticalContainer verticalContainer)
        {
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, VerticalContainer container)
        {
            throw new NotSupportedException();
        }
    }
}
