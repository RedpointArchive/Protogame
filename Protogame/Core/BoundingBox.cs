// ReSharper disable CheckNamespace

namespace Protogame
{
    /// <summary>
    /// The bounding box.
    /// </summary>
    public class BoundingBox : IBoundingBox
    {
        public BoundingBox()
        {
            Transform = new DefaultTransform();
        }

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
        /// Gets or sets the x speed.
        /// </summary>
        /// <value>
        /// The x speed.
        /// </value>
        public float XSpeed { get; set; }

        /// <summary>
        /// Gets or sets the y speed.
        /// </summary>
        /// <value>
        /// The y speed.
        /// </value>
        public float YSpeed { get; set; }

        /// <summary>
        /// Gets or sets the z speed.
        /// </summary>
        /// <value>
        /// The z speed.
        /// </value>
        public float ZSpeed { get; set; }

        /// <summary>
        /// Gets the local transform.  For bounding boxes, this currently is only used for translation.
        /// </summary>
        public ITransform Transform { get; }

        public IFinalTransform FinalTransform
        {
            get { return this.GetDetachedFinalTransformImplementation(); }
        }
    }
}