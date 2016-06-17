namespace Protogame
{
    using System.Linq;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The vertical container.
    /// </summary>
    public class VerticalContainer : FlowContainer
    {
        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        public override void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawVerticalContainer(context, layout, this);
            foreach (var kv in this.ChildrenWithLayouts(layout).OrderBy(x => x.Key.Order))
            {
                kv.Key.Draw(context, skin, kv.Value);
            }
        }

        /// <summary>
        /// The create child layout.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="accumulated">
        /// The accumulated.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The <see cref="Rectangle"/>.
        /// </returns>
        protected override Rectangle CreateChildLayout(Rectangle layout, int accumulated, int size)
        {
            return new Rectangle(layout.X, layout.Y + accumulated, layout.Width, size);
        }

        /// <summary>
        /// The get maximum container size.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected override int GetMaximumContainerSize(Rectangle layout)
        {
            return layout.Height;
        }
    }
}