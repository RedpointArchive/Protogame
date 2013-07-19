using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protogame;
using Wedge.XNA.Input;

namespace Protogame
{
    public class TextBox : IContainer
    {
        public IContainer[] Children { get { return new IContainer[0]; } }
        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public bool Focused { get; set; }
        public string Hint { get; set; }
        public int UpdateCounter { get; set; }
        private StringBuilder m_TextBuilder = new StringBuilder();
        private string m_PreviousValue = "";
        private KeyboardStringReader m_KeyboardReader = new KeyboardStringReader();
        public event EventHandler TextChanged;

        public string Text
        {
            get { return this.m_TextBuilder.ToString(); }
            set
            {
                var oldValue = this.m_TextBuilder.ToString();
                this.m_TextBuilder = new StringBuilder(value);
                this.m_PreviousValue = value;
                if (oldValue != value)
                {
                    if (this.TextChanged != null)
                        this.TextChanged(this, new EventArgs());
                }
            }
        }

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            this.UpdateCounter++;
            var mouse = Mouse.GetState();
            var leftPressed = mouse.LeftPressed(this);
            if (layout.Contains(mouse.X, mouse.Y) && leftPressed)
                this.Focus();

            var keyboard = Keyboard.GetState();
            if (this.Focused)
            {
                this.m_KeyboardReader.Process(keyboard, gameTime, this.m_TextBuilder);
                if (this.m_TextBuilder.ToString() != this.m_PreviousValue)
                {
                    if (this.TextChanged != null)
                        this.TextChanged(this, new EventArgs());
                }
                this.m_PreviousValue = this.m_TextBuilder.ToString();
            }
        }

        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawTextBox(context, layout, this);
        }
    }
}

