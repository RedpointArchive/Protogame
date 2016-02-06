namespace Protogame
{
    /// <summary>
    /// Represents an image source for image processing.
    /// </summary>
    /// <module>Image Processing</module>
    public interface IImageSource
    {
        /// <summary>
        /// Returns the image as an array of RGBA bytes.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <returns>The image as an array of RGBA bytes.</returns>
        byte[] GetSourceAsBytes(out int width, out int height);
    }
}