namespace Protogame
{
    using System;
    using System.Linq;
    using Microsoft.Xna.Framework;

    public class HorizontalContainer : FlowContainer, IHasDesiredSize
    {
        public int? GetDesiredHeight(ISkinLayout skin)
        {
            return null;
        }

        public int? GetDesiredWidth(ISkinLayout skin)
        {
            return GetPureChildrenSize();
        }

        public override void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
            foreach (var kv in this.ChildrenWithLayouts(layout).OrderByDescending(x => x.Key.Order))
            {
                kv.Key.Render(context, skinLayout, skinDelegator, kv.Value);
            }
        }
        
        protected override Rectangle CreateChildLayout(Rectangle layout, int accumulated, int size)
        {
            return new Rectangle(layout.X + accumulated, layout.Y, size, layout.Height);
        }
        
        protected override int GetMaximumContainerSize(Rectangle layout)
        {
            return layout.Width;
        }
    }
}