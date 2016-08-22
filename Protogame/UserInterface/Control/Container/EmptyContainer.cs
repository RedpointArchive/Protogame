using Microsoft.Xna.Framework;

namespace Protogame
{    
    public class EmptyContainer : IContainer
    {
        public IContainer[] Children => IContainerConstant.EmptyContainers;
        
        public bool Focused { get; set; }

        public int Order { get; set; }
        
        public IContainer Parent { get; set; }

        public object Userdata { get; set; }
        
        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
        }
        
        public void Update(ISkinLayout skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
        }
        
        public bool HandleEvent(ISkinLayout skin, Rectangle layout, IGameContext context, Event @event)
        {
            return false;
        }
    }
}