namespace Protogame
{
    using Microsoft.Xna.Framework;
    
    public interface IContainer
    {
        IContainer[] Children { get; }
        
        bool Focused { get; set; }

        int Order { get; set; }

        IContainer Parent { get; set; }

        object Userdata { get; set; }

        void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout);

        void Update(ISkinLayout skinLayout, Rectangle layout, GameTime gameTime, ref bool stealFocus);

        bool HandleEvent(ISkinLayout skinLayout, Rectangle layout, IGameContext context, Event @event);
    }
}