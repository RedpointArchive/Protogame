using System;

namespace Protogame
{
    /// <summary>
    /// Represents the backbuffer size returned from <see cref="IBackBufferDimensions"/>.
    /// </summary>
    [Serializable]
    public struct BackBufferSize
    {
        /// <summary>
        /// Construct a new <see cref="BackBufferSize"/> with the given width and height.
        /// </summary>
        /// <param name="width">The backbuffer width.</param>
        /// <param name="height">The backbuffer height.</param>
        public BackBufferSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// The width of the backbuffer.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the backbuffer.
        /// </summary>
        public int Height { get; set; }
    }
}
