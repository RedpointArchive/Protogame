using Microsoft.Xna.Framework;

namespace Protogame
{    
    public class FontViewer : IContainer
    {
        public FontViewer()
        {
            Font = null;
        }

        public IContainer[] Children => IContainerConstant.EmptyContainers;
        
        public bool Focused { get; set; }
        
        public IAssetReference<FontAsset> Font { get; set; }
        
        public int Order { get; set; }
        
        public IContainer Parent { get; set; }

        public object Userdata { get; set; }
        
        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
        }
        
        public void Update(ISkinLayout skinLayout, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
        }
        
        public bool HandleEvent(ISkinLayout skinLayout, Rectangle layout, IGameContext context, Event @event)
        {
            return false;
        }
    }
}