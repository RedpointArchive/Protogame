namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The button.
    /// </summary>
    public class Button : IContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        public Button()
        {
            this.State = ButtonUIState.None;
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
        public ButtonUIState State { get; private set; }

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
        public virtual void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawButton(context, layout, this);
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
        }

        /// <summary>
        /// Requests that the UI container handle the specified event or return false.
        /// </summary>
        /// <param name="skin">
        /// The UI skin.
        /// </param>
        /// <param name="layout">
        /// The layout for the element.
        /// </param>
        /// <param name="context">
        /// The current game context.
        /// </param>
        /// <param name="event">
        /// The event that was raised.
        /// </param>
        /// <returns>
        /// Whether or not this UI element handled the event.
        /// </returns>
        public bool HandleEvent(ISkin skin, Rectangle layout, IGameContext context, Event @event)
        {
            var mouseEvent = @event as MouseEvent;
            var mousePressEvent = @event as MousePressEvent;
            var mouseReleaseEvent = @event as MouseReleaseEvent;
            var mouseMoveEvent = @event as MouseMoveEvent;
            var touchEvent = @event as TouchEvent;
            var touchPressEvent = @event as TouchPressEvent;
            var touchHeldEvent = @event as TouchHeldEvent;
            var touchReleaseEvent = @event as TouchReleaseEvent;

            if (mouseEvent == null && touchEvent == null)
            {
                return false;
            }

            var x = 0;
            var y = 0;

            if (mouseEvent != null)
            {
                x = mouseEvent.MouseState.X;
                y = mouseEvent.MouseState.Y;
            }

            if (touchPressEvent != null)
            {
                x = (int)touchPressEvent.X;
                y = (int)touchPressEvent.Y;
            }

            if (touchReleaseEvent != null)
            {
                x = (int)touchReleaseEvent.X;
                y = (int)touchReleaseEvent.Y;
            }

            if (touchHeldEvent != null)
            {
                x = (int)touchHeldEvent.X;
                y = (int)touchHeldEvent.Y;
            }

            if (layout.Contains(x, y))
            {
                if (mouseMoveEvent != null)
                {
                    this.State = ButtonUIState.Hover;
                }
                else if ((mousePressEvent != null && mousePressEvent.Button == MouseButton.Left) || touchPressEvent != null)
                {
                    if (this.Click != null && this.State != ButtonUIState.Clicked)
                    {
                        this.Click(this, new EventArgs());
                    }

                    this.State = ButtonUIState.Clicked;
                    this.Focus();

                    return true;
                }
            }
            else if (mouseMoveEvent != null)
            {
                this.State = ButtonUIState.None;
            }

            if (mouseReleaseEvent != null && mouseReleaseEvent.Button == MouseButton.Left)
            {
                if (touchHeldEvent == null || !layout.Contains(x, y))
                {
                    this.State = ButtonUIState.None;
                }
            }

            if (touchReleaseEvent != null && layout.Contains(x, y))
            {
                this.State = ButtonUIState.None;
            }

            return false;
        }
    }
}