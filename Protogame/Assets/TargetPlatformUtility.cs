namespace Protogame
{
    /// <summary>
    /// The target platform utility.
    /// </summary>
    public static class TargetPlatformUtility
    {
        /// <summary>
        /// The get executing platform.
        /// </summary>
        /// <returns>
        /// The <see cref="TargetPlatform"/>.
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
#else
            throw new NotSupportedException();
#endif
        }
    }
}