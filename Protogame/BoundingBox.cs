namespace Protogame
{
    /// <summary>
    /// The bounding box.
    /// </summary>
    public class BoundingBox : IBoundingBox
    {
        /// <summary>
        /// Gets or sets the depth.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        public float Depth { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public float Height { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public float Width { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the x speed.
        /// </summary>
        /// <value>
        /// The x speed.
        /// </value>
        public float XSpeed { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the y speed.
        /// </summary>
        /// <value>
        /// The y speed.
        /// </value>
        public float YSpeed { get; set; }

        /// <summary>
        /// Gets or sets the z.
        /// </summary>
        /// <value>
        /// The z.
        /// </value>
        public float Z { get; set; }

        /// <summary>
        /// Gets or sets the z speed.
        /// </summary>
        /// <value>
        /// The z speed.
        /// </value>
        public float ZSpeed { get; set; }
    }
}