using Microsoft.Xna.Framework;

namespace Protogame
{
    public static class BoundingBoxExtensions
    {
        public static Rectangle ToRectangle(this IBoundingBox boundingBox)
        {
            return new Rectangle(
                (int)boundingBox.X,
                (int)boundingBox.Y,
                (int)boundingBox.Width,
                (int)boundingBox.Height);
        }
        
        public static IBoundingBox ToBoundingBox(this Rectangle rectangle)
        {
            return new BoundingBox
            {
                X = rectangle.X,
                Y = rectangle.Y,
                Width = rectangle.Width,
                Height = rectangle.Height
            };
        }
        
        public static void CopyTo(this IBoundingBox boundingBox, Rectangle rectangle)
        {
            rectangle.X = (int)boundingBox.X;
            rectangle.Y = (int)boundingBox.Y;
            rectangle.Width = (int)boundingBox.Width;
            rectangle.Height = (int)boundingBox.Height;
        }
        
        public static void CopyTo(this Rectangle rectangle, IBoundingBox boundingBox)
        {
            boundingBox.X = rectangle.X;
            boundingBox.Y = rectangle.Y;
            boundingBox.Width = rectangle.Width;
            boundingBox.Height = rectangle.Height;
        }
    }
}
