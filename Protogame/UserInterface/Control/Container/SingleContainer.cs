using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class SingleContainer : IContainer, IHasDesiredSize
    {
        private IContainer _child;

        public int? DesiredWidth { get; set; }

        public int? DesiredHeight { get; set; }
        
        public IContainer[] Children => new[] { _child };
        
        public bool Focused { get; set; }
        
        public int Order { get; set; }
        
        public IContainer Parent { get; set; }

        public object Userdata { get; set; }

        public int? GetDesiredWidth(ISkinLayout skinLayout)
        {
            return DesiredWidth;
        }

        public int? GetDesiredHeight(ISkinLayout skinLayout)
        {
            return DesiredHeight;
        }
        
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
        
        public virtual void Update(ISkinLayout skinLayout, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            _child?.Update(skinLayout, layout, gameTime, ref stealFocus);
        }
        
        public bool HandleEvent(ISkinLayout skinLayout, Rectangle layout, IGameContext context, Event @event)
        {
            return _child != null && _child.HandleEvent(skinLayout, layout, context, @event);
        }
    }
}