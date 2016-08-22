using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicFormSkinRenderer : ISkinRenderer<Form>
    {
        private readonly IBasicSkinHelper _basicSkinHelper;

        public BasicFormSkinRenderer(IBasicSkinHelper basicSkinHelper)
        {
            _basicSkinHelper = basicSkinHelper;
        }

        public void Render(IRenderContext renderContext, Rectangle layout, Form form)
        {
            _basicSkinHelper.DrawFlat(renderContext, layout);
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, Form container)
        {
            throw new NotSupportedException();
        }
    }
}
