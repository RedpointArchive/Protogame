namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// These extension methods assist with converting between bounding boxes
    /// and rectangles.
    /// <para>
    /// Protogame's bounding box includes speed attributes, which the built-in
    /// MonoGame bounding box does not have.
    /// </para>
    /// </summary>
    /// <module>Extensions</module>
    public static class BoundingBoxExtensions
    {
        /// <summary>
        /// Copies the specified Protogame bounding box to the XNA rectangle.
        /// </summary>
        /// <param name="boundingBox">
        /// The Protogame bounding box to copy from.
        /// </param>
        /// <param name="rectangle">
        /// The XNA rectangle to copy to.
        /// </param>
        public static void CopyTo(this IBoundingBox boundingBox, Rectangle rectangle)
        {
            rectangle.X = (int)boundingBox.Transform.LocalPosition.X;
            rectangle.Y = (int)boundingBox.Transform.LocalPosition.Y;
            rectangle.Width = (int)boundingBox.Width;
            rectangle.Height = (int)boundingBox.Height;
        }

        /// <summary>
        /// Copies the specified XNA rectangle to the Protogame bounding box.
        /// </summary>
        /// <param name="rectangle">
        /// The XNA rectangle to copy from.
        /// </param>
        /// <param name="boundingBox">
        /// The Protogame bounding box to copy to.
        /// </param>
        public static void CopyTo(this Rectangle rectangle, IBoundingBox boundingBox)
        {
            boundingBox.Transform.LocalPosition *= 
                new Vector3(rectangle.X, rectangle.Y, 0) - boundingBox.Transform.LocalPosition;
            boundingBox.Width = rectangle.Width;
            boundingBox.Height = rectangle.Height;
        }

        /// <summary>
        /// Converts the specified XNA rectangle to a Protogame bounding box.
        /// </summary>
        /// <param name="rectangle">
        /// The XNA rectangle to convert.
        /// </param>
        /// <returns>
        /// The <see cref="IBoundingBox"/>.
        /// </returns>
        public static IBoundingBox ToBoundingBox(this Rectangle rectangle)
        {
            return new BoundingBox
            {
                Transform = new DefaultTransform { LocalPosition = new Vector3(rectangle.X, rectangle.Y, 0) },
                Width = rectangle.Width, 
                Height = rectangle.Height
            };
        }

        /// <summary>
        /// Converts the specified XNA bounding box to a Protogame bounding box.
        /// </summary>
        /// <param name="boundingBox">
        /// The XNA bounding box to convert.
        /// </param>
        /// <returns>
        /// The <see cref="IBoundingBox"/>.
        /// </returns>
        public static IBoundingBox ToProtogame(this Microsoft.Xna.Framework.BoundingBox boundingBox)
        {
            return new BoundingBox
            {
                Transform = new DefaultTransform { LocalPosition = new Vector3(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Min.Z) },
                Width = (boundingBox.Max - boundingBox.Min).X, 
                Height = (boundingBox.Max - boundingBox.Min).Y, 
                Depth = (boundingBox.Max - boundingBox.Min).Z
            };
        }

        /// <summary>
        /// Converts the specified Protogame bounding box to an XNA rectangle.
        /// </summary>
        /// <param name="boundingBox">
        /// The Protogame bounding box to convert.
        /// </param>
        /// <returns>
        /// The <see cref="Rectangle"/>.
        /// </returns>
        public static Rectangle ToRectangle(this IBoundingBox boundingBox)
        {
            return new Rectangle(
                (int)boundingBox.Transform.LocalPosition.X, 
                (int)boundingBox.Transform.LocalPosition.Y, 
                (int)boundingBox.Width, 
                (int)boundingBox.Height);
        }

        /// <summary>
        /// Converts the specified Protogame bounding box to an XNA bounding box.
        /// </summary>
        /// <param name="boundingBox">
        /// The Protogame bounding box to convert.
        /// </param>
        /// <returns>
        /// The <see cref="Microsoft.Xna.Framework.BoundingBox"/>.
        /// </returns>
        public static Microsoft.Xna.Framework.BoundingBox ToXna(this IBoundingBox boundingBox)
        {
            return new Microsoft.Xna.Framework.BoundingBox(
                boundingBox.Transform.LocalPosition,
                boundingBox.Transform.LocalPosition + new Vector3(
                    boundingBox.Width, 
                    boundingBox.Height, 
                    boundingBox.Depth));
        }
    }
}