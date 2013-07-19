using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Protogame
{
    public class VerticalContainer : FlowContainer
    {
        protected override int GetMaximumContainerSize(Rectangle layout)
        {
            return layout.Height;
        }

        protected override Rectangle CreateChildLayout(Rectangle layout, int accumulated, int size)
        {
            return new Rectangle(
                layout.X,
                layout.Y + accumulated,
                layout.Width,
                size);
        }

        public override void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawVerticalContainer(context, layout, this);
            foreach (var kv in this.ChildrenWithLayouts(layout).OrderBy(x => x.Key.Order))
                kv.Key.Draw(context, skin, kv.Value);
        }
    }
}

