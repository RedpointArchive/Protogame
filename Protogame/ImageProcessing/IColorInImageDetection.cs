using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// An interface that represents an implementation which detects the color
    /// in images (from a given image source) and returns locations in the
    /// image where the color appears in high concentrations.
    /// <para>
    /// This interface must be constructed using <see cref="IImageProcessingFactory"/>.
    /// Because it processes data in a background thread, you must call
    /// <see cref="Start"/> to begin the actual image analysis.
    /// </para>
    /// </summary>
    /// <module>Image Processing</module>
    public interface IColorInImageDetection : IDisposable
    {
        /// <summary>
        /// Registers a color for detection.  The implementation will continuosly
        /// scan the image source on a background thread to detect this color in
        /// the image.
        /// <para>
        /// You can register multiple colors for detection by calling this method
        /// multiple times.
        /// </para>
        /// <para>
        /// This method returns a handle which you can then use in other methods
        /// on this interface to get the results of scanning for this color.
        /// </para>
        /// </summary>
        /// <param name="color">The color to detect in the image.</param>
        /// <param name="name">The name of the color for diagnostic purposes.</param>
        /// <returns></returns>
        ISelectedColorHandle RegisterColorForAnalysis(Color color, string name);

        /// <summary>
        /// Returns a two-dimensional integer array representing the strength of
        /// color detected in the image.
        /// <para>
        /// When the image is analysed, it is split up into a grid of chunks, where
        /// each chunk is <see cref="ChunkSize"/>x<see cref="ChunkSize"/> pixels in
        /// size.  Each chunk is represented by a single integer in the array.  Thus
        /// if you map the array returned by this method onto the screen, using a
        /// <see cref="ChunkSize"/>x<see cref="ChunkSize"/> rectangle to represent
        /// each value going across and then down, you will see the analysis of the
        /// original image with the desired color represented as high integer values.
        /// </para>
        /// <para>
        /// In the returned array, negative values represent a lack of the
        /// requested color, while positive values represent a presence of the
        /// requested color.  Neutral values (close to 0) represent parts of the
        /// image which are neither strongly correlated or strongly against the
        /// desired color.
        /// </para>
        /// <para>
        /// For example, if you were detecting the color red, and you had a picture
        /// of a blue object in the image source, this array would be filled with a
        /// large amount of negative values representing the blue object in the
        /// image.  If you had a very red object in the image, you would see very
        /// high positive integers representing the presence of that object.
        /// </para>
        /// <para>
        /// In testing, and with the default global sensitivity, we have found that
        /// a threshold of positive <c>100</c> (after dividing the value by the
        /// color's local sensivity; see <see cref="GetSensitivityForColor"/>)
        /// represents the precense of a color
        /// in an image, while lower values represent neutral or opposite of colors.
        /// However, you should be aware that you might need to offer calibration of
        /// global sensitivity in your game if your image source is from an external
        /// camera, as different lighting conditions may require a different sensitivity
        /// level.
        /// </para>
        /// </summary>
        /// <param name="handle">The color handle returned by <see cref="RegisterColorForAnalysis"/>.</param>
        /// <returns>The two-dimensional integer array representing the strength of color in the image.</returns>
        int[,] GetUnlockedResultsForColor(ISelectedColorHandle handle);

        /// <summary>
        /// Gets the localised sensitivity value for the specified color.
        /// <para>
        /// Because images may contain varying amounts of a specific color in them,
        /// the color detection algorithm attempts to normalize the sensitivity on a
        /// per-color basis, depending on how much of the image contains that color.
        /// </para>
        /// <para>
        /// You should divide the integers inside <see cref="GetUnlockedResultsForColor"/>
        /// by this value to determine the actual score.
        /// </para>
        /// </summary>
        /// <param name="handle">The color handle returned by <see cref="RegisterColorForAnalysis"/>.</param>
        /// <returns>The sensitivity calculated for the color.</returns>
        float GetSensitivityForColor(ISelectedColorHandle handle);

        /// <summary>
        /// Gets the total amount of color detected in the image.
        /// <para>
        /// This value is primarily useful for diagnositic purposes.
        /// </para>
        /// </summary>
        /// <param name="handle">The color handle returned by <see cref="RegisterColorForAnalysis"/>.</param>
        /// <returns>The total amount of color detected in the image.</returns>
        int GetTotalDetectedForColor(ISelectedColorHandle handle);

        /// <summary>
        /// Gets the name of the color when it was originally registered.
        /// </summary>
        /// <param name="handle">The color handle returned by <see cref="RegisterColorForAnalysis"/>.</param>
        /// <returns>The name of the color when it was originally registered.</returns>
        string GetNameForColor(ISelectedColorHandle handle);

        /// <summary>
        /// Gets the color value of the color when it was originally registered.
        /// </summary>
        /// <param name="handle">The color handle returned by <see cref="RegisterColorForAnalysis"/>.</param>
        /// <returns>The color value of the color when it was originally registered.</returns>
        Color GetValueForColor(ISelectedColorHandle handle);

        /// <summary>
        /// The global sensitivity used to detect color in the image.  This is
        /// preset at a sensible default, so you should not really need to change 
        /// this value.
        /// </summary>
        float GlobalSensitivity { get; set; }

        /// <summary>
        /// The size of chunks to perform the analysis with.  The smaller this value,
        /// the exponentially larger number of sections the algorithm has to scan over.
        /// <para>
        /// In testing, we have found that for a <c>640</c>x<c>480</c> image source, a chunk size of
        /// <c>5</c> is a reasonable value.  This is the default value.  If scanning larger
        /// images, or if you're finding the results are being processed too slowly on the
        /// background thread, you may need to set this property to a higher value.
        /// </para>
        /// <para>
        /// Once <see cref="Start"/> has been called, you can not change this value.  If you
        /// need to change the chunk size after image analysis has been started, you need
        /// to <see cref="IDisposable.Dispose"/> of this instance and create a new one using
        /// the <see cref="IImageProcessingFactory"/> factory.
        /// </para>
        /// </summary>
        int ChunkSize { get; set; }

        /// <summary>
        /// Starts the background image analysis.  Until you call this function, no image
        /// analysis is performed.
        /// </summary>
        void Start();
    }
}