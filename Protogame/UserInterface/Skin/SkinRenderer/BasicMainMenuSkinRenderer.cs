using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicMainMenuSkinRenderer : ISkinRenderer<MainMenu>
    {
        private readonly IBasicSkinHelper _basicSkinHelper;

        public BasicMainMenuSkinRenderer(IBasicSkinHelper basicSkinHelper)
        {
            _basicSkinHelper = basicSkinHelper;
        }

        public void Render(IRenderContext renderContext, Rectangle layout, MainMenu mainMenu)
        {
            _basicSkinHelper.DrawFlat(renderContext, layout);
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, MainMenu container)
        {
            throw new NotSupportedException();
        }
    }
}
