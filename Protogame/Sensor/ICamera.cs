namespace Protogame
{
    /// <summary>
    /// A camera sensor on the current device.
    /// </summary>
    /// <module>Sensor</module>
    public interface ICamera
    {
        /// <summary>
        /// The user-friendly name of the camera hardware.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The width of the images captured by this camera.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The height of images captured by this camera.
        /// </summary>
        int Height { get; } 
    }
}