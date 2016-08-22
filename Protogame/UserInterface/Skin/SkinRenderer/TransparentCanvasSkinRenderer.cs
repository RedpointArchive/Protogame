using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class TransparentCanvasSkinRenderer : ISkinRenderer<Canvas>
    {
        public void Render(IRenderContext renderContext, Rectangle layout, Canvas canvas)
        {
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, Canvas container)
        {
            throw new NotSupportedException();
        }
    }
}
