namespace Protogame
{
    using System.Linq;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The horizontal container.
    /// </summary>
    public class HorizontalContainer : FlowContainer
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
            skin.DrawHorizontalContainer(context, layout, this);
            foreach (var kv in this.ChildrenWithLayouts(layout).OrderByDescending(x => x.Key.Order))
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
            return new Rectangle(layout.X + accumulated, layout.Y, size, layout.Height);
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
            return layout.Width;
        }
    }
}