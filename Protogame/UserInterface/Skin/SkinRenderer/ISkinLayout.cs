using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface ISkinLayout
    {
        int AdditionalMenuItemWidth { get; }
        
        int HeightForTreeItem { get; }
        
        int ListHorizontalPadding { get; }
        
        int ListVerticalPadding { get; }
        
        int MainMenuHorizontalPadding { get; }
        
        int MenuItemHeight { get; }

        int HorizontalScrollBarHeight { get; }

        int VerticalScrollBarWidth { get; }

        int GetLeftPadding(IContainer container, object context);

        int GetRightPadding(IContainer container, object context);

        int GetTopPadding(IContainer container, object context);

        int GetBottomPadding(IContainer container, object context);
    }
}
