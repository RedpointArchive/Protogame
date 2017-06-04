namespace Protogame
{
    using System;
    using System.Linq;
    using Microsoft.Xna.Framework;

    public class VerticalContainer : FlowContainer, IHasDesiredSize
    {
        public int? GetDesiredHeight(ISkinLayout skin)
        {
            return GetPureChildrenSize();
        }

        public int? GetDesiredWidth(ISkinLayout skin)
        {
            return null;
        }

        public override void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
            foreach (var kv in this.ChildrenWithLayouts(layout).OrderBy(x => x.Key.Order))
            {
                kv.Key.Render(context, skinLayout, skinDelegator, kv.Value);
            }
        }

        protected override Rectangle CreateChildLayout(Rectangle layout, int accumulated, int size)
        {
            return new Rectangle(layout.X, layout.Y + accumulated, layout.Width, size);
        }

        protected override int GetMaximumContainerSize(Rectangle layout)
        {
            return layout.Height;
        }
    }
}