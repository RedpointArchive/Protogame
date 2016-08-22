using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicSingleContainerSkinRenderer : ISkinRenderer<SingleContainer>
    {
        public void Render(IRenderContext renderContext, Rectangle layout, SingleContainer singleContainer)
        {
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, SingleContainer container)
        {
            throw new NotSupportedException();
        }
    }
}
