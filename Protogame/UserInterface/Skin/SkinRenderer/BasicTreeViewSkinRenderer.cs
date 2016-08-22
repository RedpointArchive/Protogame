using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicTreeViewSkinRenderer : ISkinRenderer<TreeView>
    {
        private readonly IBasicSkinHelper _basicSkinHelper;

        public BasicTreeViewSkinRenderer(IBasicSkinHelper basicSkinHelper)
        {
            _basicSkinHelper = basicSkinHelper;
        }

        public void Render(IRenderContext renderContext, Rectangle layout, TreeView treeView)
        {
            _basicSkinHelper.DrawSunken(renderContext, layout);
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, TreeView container)
        {
            throw new NotSupportedException();
        }
    }
}
