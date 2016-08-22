namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;
    
    public class Window : IContainer
    {
        private IContainer _child;
        
        public Rectangle Bounds { get; set; }
        
        public IContainer[] Children => new[] { _child };

        public bool Focused { get; set; }
        
        public bool Modal { get; set; }
        
        public int Order { get; set; }
        
        public IContainer Parent { get; set; }

        public object Userdata { get; set; }
        
        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
            _child?.Render(context, skinLayout, skinDelegator, layout);
        }
        
        public void SetChild(IContainer child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (child.Parent != null)
            {
                throw new InvalidOperationException();
            }

            _child = child;
            _child.Parent = this;
        }
        
        public void Update(ISkinLayout skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            _child?.Update(skin, layout, gameTime, ref stealFocus);

            if (Modal)
            {
                stealFocus = true;
            }
        }
        
        public bool HandleEvent(ISkinLayout skin, Rectangle layout, IGameContext context, Event @event)
        {
            return _child != null && _child.HandleEvent(skin, layout, context, @event);
        }
    }
}