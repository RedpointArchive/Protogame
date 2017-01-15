using System;

#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
#endif

namespace Protogame
{
    public static class EffectCompilerHelper
    {
#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
        public static CompiledEffectContent Compile(EffectContent output, string tempOutputPath, TargetPlatform platform, bool isDebug, string defines)
        {
            var processor = new EffectProcessor();
            var context = new DummyContentProcessorContext(TargetPlatformCast.ToMonoGamePlatform(platform));
            context.ActualOutputFilename = tempOutputPath;
            processor.DebugMode = isDebug
                ? EffectProcessorDebugMode.Debug
                : EffectProcessorDebugMode.Optimize;
            processor.Defines = defines + ";" + GetPlatformDefineForEffect(platform) + ";" +
                                (isDebug ? "CONFIGURATION_DEBUG" : "CONFIGURATION_RELEASE");
            return processor.Process(output, context);
        }
#endif

        private static string GetPlatformDefineForEffect(TargetPlatform platform)
        {
            switch (platform)
            {
                case TargetPlatform.Windows:
                    return "PLATFORM_WINDOWS";
                case TargetPlatform.Xbox360:
                    return "PLATFORM_XBOX360";
                case TargetPlatform.iOS:
                    return "PLATFORM_IOS";
                case TargetPlatform.Android:
                    return "PLATFORM_ANDROID";
                case TargetPlatform.Linux:
                    return "PLATFORM_LINUX";
                case TargetPlatform.MacOSX:
                    return "PLATFORM_MACOSX";
                case TargetPlatform.WindowsStoreApp:
                    return "PLATFORM_WINDOWSSTOREAPP";
                case TargetPlatform.NativeClient:
                    return "PLATFORM_NATIVECLIENT";
                case TargetPlatform.Ouya:
                    return "PLATFORM_OUYA";
                case TargetPlatform.PlayStationMobile:
                    return "PLATFORM_PSMOBILE";
                case TargetPlatform.WindowsPhone8:
                    return "PLATFORM_WINDOWSPHONE8";
                case TargetPlatform.RaspberryPi:
                    return "PLATFORM_RASPBERRYPI";
                case TargetPlatform.Web:
                    return "PLATFORM_WEB";
                default:
                    return String.Empty;
            }
        }
    }
}
