namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a packed texture as the result from the texture packer.
    /// </summary>
    /// <typeparam name="T">The type of object.</typeparam>
    public class PackedTexture<T>
    {
        /// <summary>
        /// Gets or sets the position of the texture.
        /// </summary>
        /// <value>The position of the texture.</value>
        public Vector2 Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the texture.
        /// </summary>
        /// <value>The size of the texture.</value>
        public Vector2 Size
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>The texture.</value>
        public T Texture 
        { 
            get;
            set;
        }
    }
}
