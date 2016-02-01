using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// A sensor which represents all available cameras on the current device.
    /// </summary>
    /// <module>Sensor</module>
    public interface ICameraSensor : ISensor
    {
        /// <summary>
        /// The texture representing what the currently active camera is
        /// capturing.  This value may be <c>null</c> if no active camera
        /// is set, or if the image from the active camera is not ready.
        /// </summary>
        Texture2D VideoCaptureFrame { get; }

        /// <summary>
        /// The raw RGBA data from the camera.  This data is written to by
        /// another thread, which means that it is not atomically correct.
        /// The data in this array may be written to as you are reading
        /// from it.
        /// </summary>
        byte[] VideoCaptureUnlockedRGBA { get; }

        /// <summary>
        /// The width of the current texture, or <c>null</c> if no texture
        /// is currently available.
        /// </summary>
        int? VideoCaptureWidth { get; }

        /// <summary>
        /// The height of the current texture, or <c>null</c> if no texture
        /// is currently available.
        /// </summary>
        int? VideoCaptureHeight { get; }

        /// <summary>
        /// The currently active camera.  This is initially <c>null</c>, and
        /// you need to set this value to one of the cameras returned by
        /// <see cref="GetAvailableCameras"/>.
        /// </summary>
        ICamera ActiveCamera { get; set; }

        /// <summary>
        /// Gets all of the available cameras on this device.
        /// </summary>
        /// <returns></returns>
        List<ICamera> GetAvailableCameras();
    }
}
