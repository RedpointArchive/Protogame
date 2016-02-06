using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// An interface which represents an implementation that accepted
    /// analysed image data (such as that from <see cref="IColorInImageDetection"/>)
    /// and groups the detected highlights in the image into individual
    /// points.
    /// <para>
    /// When analysing an image for highlighted color points, you will often
    /// need to convert these segments of color into individual points so they
    /// can be mapped back into virtual space.  This API allows you to take
    /// multiple highlighted areas of an image and map them into individual 
    /// recognised points within an image.
    /// </para>
    /// </summary>
    public interface IPointInAnalysedImageDetection : IDisposable
    {
        /// <summary>
        /// The points detected in the image.
        /// </summary>
        List<Vector2> DetectedPoints { get; }

        /// <summary>
        /// The minimum number of analysed chunks in the image that
        /// need to meet the threshold in order for a point to be
        /// defined at this location.
        /// </summary>
        int MinNumberOfPoints { get; set; }

        /// <summary>
        /// The maximum number of analysed chunks in the image that
        /// need to meet the threshold in order for a point to be
        /// defined at this location.
        /// </summary>
        int MaxNumberOfPoints { get; set; }

        /// <summary>
        /// The score threshold for point detection.
        /// </summary>
        float PointThreshold { get; set; }

        /// <summary>
        /// The width of the chunk data.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The height of the chunk data.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Starts the point detection.
        /// </summary>
        void Start();
    }
}
