using Microsoft.Xna.Framework;

namespace Protogame
{    
    public class Label : IContainer
    {
        public IContainer[] Children => IContainerConstant.EmptyContainers;
        
        public bool Focused { get; set; }
        
        public int Order { get; set; }
        
        public IContainer Parent { get; set; }

        public object Userdata { get; set; }

        public string Text { get; set; }
        
        public Color? TextColor { get; set; }

        public HorizontalAlignment? HorizontalAlignment { get; set; }

        public VerticalAlignment? VerticalAlignment { get; set; }

        public IAssetReference<FontAsset> Font { get; set; }

        public bool? RenderShadow { get; set; }

        public Color? ShadowColor { get; set; }

        public IContainer AttachTo { get; set; }

        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
        }
        
        public void Update(ISkinLayout skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
        }
        
        public bool HandleEvent(ISkinLayout skin, Rectangle layout, IGameContext context, Event @event)
        {
            if (AttachTo != null)
            {
                return AttachTo.HandleEvent(skin, layout, context, @event);
            }

            return false;
        }
    }
}