using Microsoft.Xna.Framework;

namespace Protogame
{
    public class FixedContainer : BaseContainer, IContainer
    {
        public FixedContainer(Rectangle absolute)
        {
            AbsoluteRectangle = absolute;
        }
        
        public Rectangle AbsoluteRectangle { get; set; }

        public object Userdata { get; set; }
        
        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
            Child?.Render(context, skinLayout, skinDelegator, AbsoluteRectangle);
        }
        
        public void Update(ISkinLayout skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            Child?.Update(skin, AbsoluteRectangle, gameTime, ref stealFocus);
        }

        public bool HandleEvent(ISkinLayout skin, Rectangle layout, IGameContext context, Event @event)
        {
            return Child != null && Child.HandleEvent(skin, AbsoluteRectangle, context, @event);
        }
    }
}