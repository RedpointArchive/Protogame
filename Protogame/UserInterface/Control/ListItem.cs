using Microsoft.Xna.Framework;

namespace Protogame
{
    public class ListItem : IContainer
    {
        public IContainer[] Children => IContainerConstant.EmptyContainers;

        public bool Focused { get; set; }

        public int Indent { get; set; }
        
        public int Order { get; set; }
        
        public IContainer Parent { get; set; }

        public object Userdata { get; set; }
        
        public virtual string Text { get; set; }

        public virtual IAssetReference<TextureAsset> Icon { get; set; }
        
        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
        }
        
        public void Update(ISkinLayout skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
        }
        
        public bool HandleEvent(ISkinLayout skin, Rectangle layout, IGameContext context, Event @event)
        {
            var mouseEvent = @event as MousePressEvent;
            if (mouseEvent == null)
            {
                return false;
            }
            if (!layout.Contains(mouseEvent.X, mouseEvent.Y))
            {
                return false;
            }
            var listView = Parent as ListView;

            if (listView == null)
            {
                return false;
            }

            listView.SelectedItem = this;

            return true;
        }
    }
}