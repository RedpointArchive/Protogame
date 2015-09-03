namespace Protogame
{
    /// <summary>
    /// Represents a platform that Protogame can either execute on, or output
    /// asset content for.
    /// </summary>
    /// <module>Assets</module>
    public enum TargetPlatform
    {
        /// <summary>
        /// The Windows desktop.  This represents running on the Windows desktop
        /// using the Win32 API (not Metro).
        /// </summary>
        Windows, 

        /// <summary>
        /// XBox 360.  This platform is only supported by the original XNA toolkit,
        /// and Protogame can not run on it.
        /// </summary>
        Xbox360, 

        /// <summary>
        /// Windows Phone 7.
        /// </summary>
        WindowsPhone, 

        /// <summary>
        /// Apple iOS devices (including iPhone and iPad).
        /// </summary>
        iOS, 

        /// <summary>
        /// Android devices (including phones and tablets).
        /// </summary>
        Android, 

        /// <summary>
        /// The Linux desktop.
        /// </summary>
        Linux, 

        /// <summary>
        /// The Mac OS X desktop.
        /// </summary>
        MacOSX, 

        /// <summary>
        /// Windows Store applications.  This represents running under the new
        /// Metro API offered by Windows 8 and later.
        /// </summary>
        WindowsStoreApp, 

        /// <summary>
        /// Google NativeClient.  This platform is not supported by MonoGame, and as
        /// such, Protogame can not run on it.
        /// </summary>
        NativeClient, 

        /// <summary>
        /// The OUYA console, which is a superset of the Android functionality.
        /// </summary>
        Ouya, 

        /// <summary>
        /// PlayStation Mobile.  This platform has since been deprecated by Sony.
        /// </summary>
        PlayStationMobile, 

        /// <summary>
        /// Windows Phone 8.
        /// </summary>
        WindowsPhone8, 

        /// <summary>
        /// Raspberry Pi.  This platform is not supported by MonoGame, and as such,
        /// Protogame can not run on it.
        /// </summary>
        RaspberryPi, 
        
        /// <summary>
        /// A web application, where the C# code has been converted to Javascript with
        /// JSIL.  This platform is not yet supported by MonoGame, and as such,
        /// Protogame can not run on it.
        /// </summary>
        Web,
    }
}