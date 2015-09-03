namespace Protogame
{
    /// <summary>
    /// A static utility class which returns the platform that
    /// code is currently executing under.
    /// <para>
    /// This method allows you to access platform information at
    /// runtime.  To change what code is built at compile-time based
    /// on the platform, use one of the following constants:
    /// </para>
    /// <para>
    ///   - ``PLATFORM_ANDROID``
    ///   - ``PLATFORM_IOS``
    ///   - ``PLATFORM_LINUX``
    ///   - ``PLATFORM_MACOS``
    ///   - ``PLATFORM_OUYA``
    ///   - ``PLATFORM_PSMOBILE``
    ///   - ``PLATFORM_WINDOWS``
    ///   - ``PLATFORM_WINDOWS8``
    ///   - ``PLATFORM_WINDOWSGL``
    ///   - ``PLATFORM_WINDOWSPHONE``
    ///   - ``PLATFORM_WEB``
    /// </para>
    /// <para>
    /// You can use these constants with the ``#if`` construct in C#.
    /// For example:
    /// </para>
    /// <code>
    /// public void MyMethod()
    /// {
    /// <i/>#if PLATFORM_WINDOWS
    /// <i/>SomeWindowsSpecificCall()
    /// <i/>#elif PLATFORM_LINUX
    /// <i/>SomeLinuxSpecificCall()
    /// <i/>#endif
    /// }
    /// </code>
    /// </summary>
    /// <module>Assets</module>
    public static class TargetPlatformUtility
    {
        /// <summary>
        /// Returns the platform that the game is currently executing on.
        /// </summary>
        /// <returns>
        /// The <see cref="Protogame.TargetPlatform"/> value representing
        /// the platform that the game is currently executing on.
        /// </returns>
        public static TargetPlatform GetExecutingPlatform()
        {
#if PLATFORM_ANDROID
            return TargetPlatform.Android;
#elif PLATFORM_IOS
            return TargetPlatform.iOS;
#elif PLATFORM_LINUX
            return TargetPlatform.Linux;
#elif PLATFORM_MACOS
            return TargetPlatform.MacOSX;
#elif PLATFORM_OUYA
            return TargetPlatform.Ouya;
#elif PLATFORM_PSMOBILE
            return TargetPlatform.PlayStationMobile;
#elif PLATFORM_WINDOWS
            return TargetPlatform.Windows;
#elif PLATFORM_WINDOWS8
            return TargetPlatform.Windows8;
#elif PLATFORM_WINDOWSGL
            return TargetPlatform.WindowsGL;
#elif PLATFORM_WINDOWSPHONE
            return TargetPlatform.WindowsPhone;
#elif PLATFORM_WEB
            return TargetPlatform.Web;
#else
            throw new NotSupportedException();
#endif
        }
    }
}