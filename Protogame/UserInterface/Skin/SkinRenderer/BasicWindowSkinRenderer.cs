using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicWindowSkinRenderer : ISkinRenderer<Window>
    {
        private readonly IBasicSkinHelper _basicSkinHelper;

        public BasicWindowSkinRenderer(IBasicSkinHelper basicSkinHelper)
        {
            _basicSkinHelper = basicSkinHelper;
        }

        public void Render(IRenderContext renderContext, Rectangle layout, Window window)
        {
            _basicSkinHelper.DrawRaised(renderContext, layout);
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, Window container)
        {
            throw new NotSupportedException();
        }
    }
}
