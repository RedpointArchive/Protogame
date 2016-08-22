using System;
using Microsoft.Xna.Framework;

namespace Protogame
{    
    public class Link : IContainer
    {
        public Link()
        {
            State = LinkState.None;
        }
        
        public event EventHandler Click;

        public IContainer[] Children => IContainerConstant.EmptyContainers;
        
        public bool Focused { get; set; }
        
        public int Order { get; set; }
        
        public IContainer Parent { get; set; }

        public object Userdata { get; set; }
        
        public LinkState State { get; private set; }
        
        public string Text { get; set; }
        
        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
        }
        
        public void Update(ISkinLayout skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
        }
        
        public bool HandleEvent(ISkinLayout skin, Rectangle layout, IGameContext context, Event @event)
        {
            var mousePressEvent = @event as MousePressEvent;
            if (mousePressEvent != null)
            {
                if (!layout.Contains(mousePressEvent.MouseState.X, mousePressEvent.MouseState.Y))
                {
                    return false;
                }

                State = LinkState.Clicked;

                Click?.Invoke(this, new EventArgs());

                return true;
            }

            var mouseReleaseEvent = @event as MouseReleaseEvent;
            if (mouseReleaseEvent != null)
            {
                if (!layout.Contains(mouseReleaseEvent.MouseState.X, mouseReleaseEvent.MouseState.Y))
                {
                    return false;
                }

                State = LinkState.None;
            }

            return false;
        }
    }
}