namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a rectangle area of UV mappings.  Used in a texture atlas.
    /// </summary>
    public struct UVMapping
    {
        /// <summary>
        /// Gets or sets the top left UV.
        /// </summary>
        /// <value>The top left UV.</value>
        public Vector2 TopLeft { get; set; }

        /// <summary>
        /// Gets or sets the bottom right UV.
        /// </summary>
        /// <value>The bottom right UV.</value>
        public Vector2 BottomRight { get; set; }

        public float X
        {
            get
            {
                return this.TopLeft.X;
            }
        }

        public float Y
        {
            get
            {
                return this.TopLeft.Y;
            }
        }

        public float Width
        {
            get
            {
                return this.BottomRight.X - this.TopLeft.X;
            }
        }

        public float Height
        {
            get
            {
                return this.BottomRight.Y - this.TopLeft.Y;
            }
        }

        public Vector2 WidthHeight
        {
            get
            {
                return new Vector2(this.Width, this.Height);
            }
        }
    }
}
