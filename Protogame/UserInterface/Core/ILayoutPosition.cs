using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface ILayoutPosition
    {
        Vector2 GetPositionInLayout(Rectangle layout, HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment);
    }
}