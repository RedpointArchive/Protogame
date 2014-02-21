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
            var mouse = Mouse.GetState();
            var leftPressed = mouse.LeftChanged(this) == ButtonState.Pressed;
            if (layout.Contains(mouse.X, mouse.Y))
            {
                if (leftPressed)
                {
                    if (this.State != ButtonUIState.Clicked)
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
                    }
                }
                else
                {
                    this.State = ButtonUIState.Hover;
                }
            }
            else
            {
                this.State = ButtonUIState.None;
            }
        }
    }
}