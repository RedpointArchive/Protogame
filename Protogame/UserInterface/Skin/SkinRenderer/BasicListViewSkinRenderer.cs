using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicListViewSkinRenderer : ISkinRenderer<ListView>
    {
        private readonly IBasicSkinHelper _basicSkinHelper;

        public BasicListViewSkinRenderer(IBasicSkinHelper basicSkinHelper)
        {
            _basicSkinHelper = basicSkinHelper;
        }

        public void Render(IRenderContext renderContext, Rectangle layout, ListView listView)
        {
            _basicSkinHelper.DrawSunken(renderContext, layout);
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, ListView container)
        {
            throw new NotSupportedException();
        }
    }
}
