namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The bounding box extensions.
    /// </summary>
    public static class BoundingBoxExtensions
    {
        /// <summary>
        /// The copy to.
        /// </summary>
        /// <param name="boundingBox">
        /// The bounding box.
        /// </param>
        /// <param name="rectangle">
        /// The rectangle.
        /// </param>
        public static void CopyTo(this IBoundingBox boundingBox, Rectangle rectangle)
        {
            rectangle.X = (int)boundingBox.X;
            rectangle.Y = (int)boundingBox.Y;
            rectangle.Width = (int)boundingBox.Width;
            rectangle.Height = (int)boundingBox.Height;
        }

        /// <summary>
        /// The copy to.
        /// </summary>
        /// <param name="rectangle">
        /// The rectangle.
        /// </param>
        /// <param name="boundingBox">
        /// The bounding box.
        /// </param>
        public static void CopyTo(this Rectangle rectangle, IBoundingBox boundingBox)
        {
            boundingBox.X = rectangle.X;
            boundingBox.Y = rectangle.Y;
            boundingBox.Width = rectangle.Width;
            boundingBox.Height = rectangle.Height;
        }

        /// <summary>
        /// The to bounding box.
        /// </summary>
        /// <param name="rectangle">
        /// The rectangle.
        /// </param>
        /// <returns>
        /// The <see cref="IBoundingBox"/>.
        /// </returns>
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

        /// <summary>
        /// The to protogame.
        /// </summary>
        /// <param name="boundingBox">
        /// The bounding box.
        /// </param>
        /// <returns>
        /// The <see cref="IBoundingBox"/>.
        /// </returns>
        public static IBoundingBox ToProtogame(this Microsoft.Xna.Framework.BoundingBox boundingBox)
        {
            return new BoundingBox
            {
                X = boundingBox.Min.X, 
                Y = boundingBox.Min.Y, 
                Z = boundingBox.Min.Z, 
                Width = (boundingBox.Max - boundingBox.Min).X, 
                Height = (boundingBox.Max - boundingBox.Min).Y, 
                Depth = (boundingBox.Max - boundingBox.Min).Z
            };
        }

        /// <summary>
        /// The to rectangle.
        /// </summary>
        /// <param name="boundingBox">
        /// The bounding box.
        /// </param>
        /// <returns>
        /// The <see cref="Rectangle"/>.
        /// </returns>
        public static Rectangle ToRectangle(this IBoundingBox boundingBox)
        {
            return new Rectangle(
                (int)boundingBox.X, 
                (int)boundingBox.Y, 
                (int)boundingBox.Width, 
                (int)boundingBox.Height);
        }

        /// <summary>
        /// The to xna.
        /// </summary>
        /// <param name="boundingBox">
        /// The bounding box.
        /// </param>
        /// <returns>
        /// The <see cref="BoundingBox"/>.
        /// </returns>
        public static Microsoft.Xna.Framework.BoundingBox ToXna(this IBoundingBox boundingBox)
        {
            return new Microsoft.Xna.Framework.BoundingBox(
                new Vector3(boundingBox.X, boundingBox.Y, boundingBox.Z), 
                new Vector3(
                    boundingBox.X + boundingBox.Width, 
                    boundingBox.Y + boundingBox.Height, 
                    boundingBox.Z + boundingBox.Depth));
        }
    }
}