using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicFixedContainerSkinRenderer : ISkinRenderer<FixedContainer>
    {
        public void Render(IRenderContext renderContext, Rectangle layout, FixedContainer fixedContainer)
        {
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, FixedContainer container)
        {
            throw new NotSupportedException();
        }
    }
}
