namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The camera.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// The m_ height.
        /// </summary>
        private readonly int m_Height;

        /// <summary>
        /// The m_ width.
        /// </summary>
        private readonly int m_Width;

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public Camera(int width, int height)
        {
            this.m_Width = width;
            this.m_Height = height;
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height
        {
            get
            {
                return this.m_Height;
            }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width
        {
            get
            {
                return this.m_Width;
            }
        }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public float Y { get; set; }

        /// <summary>
        /// The get transformation matrix.
        /// </summary>
        /// <returns>
        /// The <see cref="Matrix"/>.
        /// </returns>
        public Matrix GetTransformationMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-this.X, -this.Y, 0));
        }
    }
}