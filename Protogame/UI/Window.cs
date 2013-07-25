using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class Window : IContainer
    {
        private IContainer m_Child;
        
        public IContainer[] Children { get { return new[] { this.m_Child }; } }
        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public bool Focused { get; set; }
        public Rectangle Bounds { get; set; }
        public bool Modal { get; set; }
        
        public void SetChild(IContainer child)
        {
            if (child == null)
                throw new ArgumentNullException("child");
            if (child.Parent != null)
                throw new InvalidOperationException();
            this.m_Child = child;
            this.m_Child.Parent = this;
        }

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            if (this.m_Child != null)
                this.m_Child.Update(skin, layout, gameTime, ref stealFocus);
            
            if (this.Modal)
                stealFocus = true;
        }

        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawWindow(context, layout, this);
            if (this.m_Child != null)
                this.m_Child.Draw(context, skin, layout);
        }
    }
}

