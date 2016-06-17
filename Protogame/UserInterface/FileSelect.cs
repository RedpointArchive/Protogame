namespace Protogame
{
	using System;
#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
	using System.Windows.Forms;
#endif
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
	using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

    /// <summary>
    /// The file select.
    /// </summary>
    public class FileSelect : IContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSelect"/> class.
        /// </summary>
        public FileSelect()
        {
            this.State = ButtonUIState.None;
        }

        /// <summary>
        /// The changed.
        /// </summary>
        public event EventHandler Changed;

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
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public ButtonUIState State { get; private set; }

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
            skin.DrawFileSelect(context, layout, this);
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

            if (mouseEvent == null)
            {
                return false;
            }

            if (layout.Contains(mouseEvent.MouseState.X, mouseEvent.MouseState.Y))
            {
                if (mouseMoveEvent != null)
                {
                    this.State = ButtonUIState.Hover;
                }
                else if (mousePressEvent != null && mousePressEvent.Button == MouseButton.Left)
                {
                    this.State = ButtonUIState.Clicked;
                    this.Focus();

#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
                    using (var openFileDialog = new OpenFileDialog())
                    {
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            this.Path = openFileDialog.FileName;
                            if (this.Changed != null)
                            {
                                this.Changed(this, new EventArgs());
                            }
                        }

                        Application.DoEvents();
                    }
#endif

                    return true;
                }
            }
            else
            {
                this.State = ButtonUIState.None;
            }

            if (mouseReleaseEvent != null && mouseReleaseEvent.Button == MouseButton.Left)
            {
                this.State = ButtonUIState.None;
            }

            return false;
        }
    }
}