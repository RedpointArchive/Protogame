namespace Protogame
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// MojoShader under Windows is hard-coded to reference the 32-bit version, but model
    /// compilation via AssImp requires that it's loaded DLL match the processor architecture.
    /// We copy the appropriate MojoShader DLL into the correct location based on the platform.
    /// </summary>
    public static class MojoShaderDeploy
    {
        /// <summary>
        /// Copies the appropriate MojoShader DLL into place.
        /// </summary>
        public static void Deploy()
        {
#if !PLATFORM_WINDOWS
            return;
#endif

            var location = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.FullName;
            var current = Environment.CurrentDirectory;
            Environment.CurrentDirectory = location;

#if PLATFORM_WINDOWS
            if (File.Exists("libmojoshader_32.dll"))
            {
                File.Delete("libmojoshader_32.dll");
            }

#else
            if (File.Exists("libmojoshader.dll"))
            {
                File.Delete("libmojoshader.dll");
            }
#endif

            if (Environment.Is64BitProcess)
            {
#if PLATFORM_WINDOWS
                File.Copy("libmojoshader_src_64.dll", "libmojoshader_32.dll");
#else
                File.Copy("libmojoshader_src_64.dll", "libmojoshader.dll");
#endif
            }
            else
            {
#if PLATFORM_WINDOWS
                File.Copy("libmojoshader_src_32.dll", "libmojoshader_32.dll");
#else
                File.Copy("libmojoshader_src_32.dll", "libmojoshader.dll");
#endif
            }

            Environment.CurrentDirectory = current;
        }
    }
}