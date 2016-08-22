using System;
using Microsoft.Xna.Framework;

namespace Protogame
{    
    public class Canvas : IContainer
    {
        private IContainer _child;
        
        public IContainer[] Children => new[] { _child };
        
        public bool Focused { get; set; }
        
        public int Order { get; set; }
        
        public IContainer Parent
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public object Userdata { get; set; }
        
        public virtual void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
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
        
        public void Update(ISkinLayout skinLayout, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            _child?.Update(skinLayout, layout, gameTime, ref stealFocus);
        }
        
        public bool HandleEvent(ISkinLayout skinLayout, Rectangle layout, IGameContext context, Event @event)
        {
            return _child != null && _child.HandleEvent(skinLayout, layout, context, @event);
        }
    }
}