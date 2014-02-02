namespace Protogame
{
    using System;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The text box.
    /// </summary>
    public class TextBox : IContainer
    {
        /// <summary>
        /// The m_ keyboard reader.
        /// </summary>
        private readonly DefaultKeyboardStringReader m_KeyboardReader = new DefaultKeyboardStringReader();

        /// <summary>
        /// The m_ previous value.
        /// </summary>
        private string m_PreviousValue = string.Empty;

        /// <summary>
        /// The m_ text builder.
        /// </summary>
        private StringBuilder m_TextBuilder = new StringBuilder();

        /// <summary>
        /// The text changed.
        /// </summary>
        public event EventHandler TextChanged;

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
        /// Gets or sets the hint.
        /// </summary>
        /// <value>
        /// The hint.
        /// </value>
        public string Hint { get; set; }

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
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get
            {
                return this.m_TextBuilder.ToString();
            }

            set
            {
                var oldValue = this.m_TextBuilder.ToString();
                this.m_TextBuilder = new StringBuilder(value);
                this.m_PreviousValue = value;
                if (oldValue != value)
                {
                    if (this.TextChanged != null)
                    {
                        this.TextChanged(this, new EventArgs());
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the update counter.
        /// </summary>
        /// <value>
        /// The update counter.
        /// </value>
        public int UpdateCounter { get; set; }

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
            skin.DrawTextBox(context, layout, this);
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
            this.UpdateCounter++;
            var mouse = Mouse.GetState();
            var leftPressed = mouse.LeftPressed(this);
            if (layout.Contains(mouse.X, mouse.Y) && leftPressed)
            {
                this.Focus();
            }

            var keyboard = Keyboard.GetState();
            if (this.Focused)
            {
                this.m_KeyboardReader.Process(keyboard, gameTime, this.m_TextBuilder);
                if (this.m_TextBuilder.ToString() != this.m_PreviousValue)
                {
                    if (this.TextChanged != null)
                    {
                        this.TextChanged(this, new EventArgs());
                    }
                }

                this.m_PreviousValue = this.m_TextBuilder.ToString();
            }
        }
    }
}