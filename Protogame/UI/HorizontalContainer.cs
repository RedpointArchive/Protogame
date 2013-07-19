using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Protogame
{
    public class HorizontalContainer : FlowContainer
    {
        protected override int GetMaximumContainerSize(Rectangle layout)
        {
            return layout.Width;
        }

        protected override Rectangle CreateChildLayout(Rectangle layout, int accumulated, int size)
        {
            return new Rectangle(
                layout.X + accumulated,
                layout.Y,
                size,
                layout.Height);
        }

        public override void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawHorizontalContainer(context, layout, this);
            foreach (var kv in this.ChildrenWithLayouts(layout).OrderByDescending(x => x.Key.Order))
                kv.Key.Draw(context, skin, kv.Value);
        }
    }
}

