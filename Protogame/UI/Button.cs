using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class Button : IContainer
    {
        public IContainer[] Children { get { return new IContainer[0]; } }
        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public ButtonUIState State { get; private set; }
        public string Text { get; set; }
        public bool Focused { get; set; }
        public event EventHandler Click;

        public Button()
        {
            this.State = ButtonUIState.None;
        }

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            var mouse = Mouse.GetState();
            var leftPressed = mouse.LeftPressed(this);
            if (layout.Contains(mouse.X, mouse.Y))
            {
                if (leftPressed)
                {
                    if (this.Click != null && this.State != ButtonUIState.Clicked)
                        this.Click(this, new EventArgs());
                    this.State = ButtonUIState.Clicked;
                    this.Focus();
                }
                else
                    this.State = ButtonUIState.Hover;
            }
            else
                this.State = ButtonUIState.None;
        }

        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawButton(context, layout, this);
        }
    }
}

