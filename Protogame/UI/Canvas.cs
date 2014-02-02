namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The canvas.
    /// </summary>
    public class Canvas : IContainer
    {
        /// <summary>
        /// The m_ child.
        /// </summary>
        private IContainer m_Child;

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public IContainer[] Children
        {
            get
            {
                return new[] { this.m_Child };
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether focused.
        /// </summary>
        /// <value>
        /// The focused.
        /// </value>
        public bool Focused { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public IContainer Parent
        {
            get
            {
                return null;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

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
        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawCanvas(context, layout, this);
            if (this.m_Child != null)
            {
                this.m_Child.Draw(context, skin, layout);
            }
        }

        /// <summary>
        /// The set child.
        /// </summary>
        /// <param name="child">
        /// The child.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public void SetChild(IContainer child)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            if (child.Parent != null)
            {
                throw new InvalidOperationException();
            }

            this.m_Child = child;
            this.m_Child.Parent = this;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        /// <param name="stealFocus">
        /// The steal focus.
        /// </param>
        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            if (this.m_Child != null)
            {
                this.m_Child.Update(skin, layout, gameTime, ref stealFocus);
            }
        }
    }
}