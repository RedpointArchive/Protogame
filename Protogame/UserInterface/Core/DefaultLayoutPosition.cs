using Microsoft.Xna.Framework;

namespace Protogame
{
    public class DefaultLayoutPosition : ILayoutPosition
    {
        public Vector2 GetPositionInLayout(Rectangle layout, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            var x = 0f;
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    x = layout.X;
                    break;
                case HorizontalAlignment.Center:
                    x = layout.X + layout.Width / 2;
                    break;
                case HorizontalAlignment.Right:
                    x = layout.X + layout.Width;
                    break;
            }

            var y = 0f;
            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    y = layout.Y;
                    break;
                case VerticalAlignment.Center:
                    y = layout.Y + layout.Height / 2;
                    break;
                case VerticalAlignment.Bottom:
                    y = layout.Y + layout.Height;
                    break;
            }

            return new Vector2(x, y);
        }
    }
}
