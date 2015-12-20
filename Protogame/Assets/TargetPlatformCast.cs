#if PLATFORM_WINDOWS || PLATFORM_MAC || PLATFORM_LINUX

using System;
using MonoGamePlatform = Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform;

namespace Protogame
{
    /// <summary>
    /// The target platform cast.
    /// </summary>
    public class TargetPlatformCast
    {
        /// <summary>
        /// The from mono game platform.
        /// </summary>
        /// <param name="platform">
        /// The platform.
        /// </param>
        /// <returns>
        /// The <see cref="TargetPlatform"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public static TargetPlatform FromMonoGamePlatform(MonoGamePlatform platform)
        {
            switch (platform)
            {
                case MonoGamePlatform.Windows:
                    return TargetPlatform.Windows;
                case MonoGamePlatform.Xbox360:
                    return TargetPlatform.Xbox360;
                case MonoGamePlatform.WindowsPhone:
                    return TargetPlatform.WindowsPhone;
                case MonoGamePlatform.iOS:
                    return TargetPlatform.iOS;
                case MonoGamePlatform.Android:
                    return TargetPlatform.Android;
                case MonoGamePlatform.Linux:
                    return TargetPlatform.Linux;
                case MonoGamePlatform.MacOSX:
                    return TargetPlatform.MacOSX;
                case MonoGamePlatform.WindowsStoreApp:
                    return TargetPlatform.WindowsStoreApp;
                case MonoGamePlatform.NativeClient:
                    return TargetPlatform.NativeClient;
                case MonoGamePlatform.WindowsPhone8:
                    return TargetPlatform.WindowsPhone8;
                case MonoGamePlatform.RaspberryPi:
                    return TargetPlatform.RaspberryPi;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// The to mono game platform.
        /// </summary>
        /// <param name="platform">
        /// The platform.
        /// </param>
        /// <returns>
        /// The <see cref="TargetPlatform"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public static MonoGamePlatform ToMonoGamePlatform(TargetPlatform platform)
        {
            switch (platform)
            {
                case TargetPlatform.Windows:
                    return MonoGamePlatform.Windows;
                case TargetPlatform.Xbox360:
                    return MonoGamePlatform.Xbox360;
                case TargetPlatform.WindowsPhone:
                    return MonoGamePlatform.WindowsPhone;
                case TargetPlatform.iOS:
                    return MonoGamePlatform.iOS;
                case TargetPlatform.Android:
                    return MonoGamePlatform.Android;
                case TargetPlatform.Linux:
                    return MonoGamePlatform.Linux;
                case TargetPlatform.MacOSX:
                    return MonoGamePlatform.MacOSX;
                case TargetPlatform.WindowsStoreApp:
                    return MonoGamePlatform.WindowsStoreApp;
                case TargetPlatform.NativeClient:
                    return MonoGamePlatform.NativeClient;
                case TargetPlatform.Ouya:
                    return MonoGamePlatform.Android;
                case TargetPlatform.WindowsPhone8:
                    return MonoGamePlatform.WindowsPhone8;
                case TargetPlatform.RaspberryPi:
                    return MonoGamePlatform.RaspberryPi;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

#endif