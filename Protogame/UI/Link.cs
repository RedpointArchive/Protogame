using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class Link : IContainer
    {
        public IContainer[] Children { get { return new IContainer[0]; } }
        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public LinkState State { get; private set; }
        public string Text { get; set; }
        public bool Focused { get; set; }
        public event EventHandler Click;

        public Link()
        {
            this.State = LinkState.None;
        }

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            var mouse = Mouse.GetState();
            if (layout.Contains(mouse.X, mouse.Y))
            {
                if (mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    if (this.Click != null && this.State != LinkState.Clicked)
                        this.Click(this, new EventArgs());
                    this.State = LinkState.Clicked;
                }
            }
            else
                this.State = LinkState.None;
        }

        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawLink(context, layout, this);
        }
    }
}

