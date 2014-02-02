namespace Protogame
{
    /// <summary>
    /// The BoundingBoxUtilities interface.
    /// </summary>
    public interface IBoundingBoxUtilities
    {
        /// <summary>
        /// The overlaps.
        /// </summary>
        /// <param name="boundingBoxes">
        /// The bounding boxes.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Overlaps(params IBoundingBox[] boundingBoxes);
    }
}