namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The link.
    /// </summary>
    public class Link : IContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// </summary>
        public Link()
        {
            this.State = LinkState.None;
        }

        /// <summary>
        /// The click.
        /// </summary>
        public event EventHandler Click;

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
                return new IContainer[0];
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
        public IContainer Parent { get; set; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public LinkState State { get; private set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

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
            skin.DrawLink(context, layout, this);
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
            var mouse = Mouse.GetState();
            if (layout.Contains(mouse.X, mouse.Y))
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (this.Click != null && this.State != LinkState.Clicked)
                    {
                        this.Click(this, new EventArgs());
                    }

                    this.State = LinkState.Clicked;
                }
            }
            else
            {
                this.State = LinkState.None;
            }
        }
    }
}