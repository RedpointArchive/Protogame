#if PLATFORM_WINDOWS || PLATFORM_MAC || PLATFORM_LINUX

using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Graphics;
using MonoGamePlatform = Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform;

namespace Protogame
{
    public abstract class MonoGameContentCompiler
    {
        protected byte[] CompileAndGetBytes(object content)
        {
            var temp = Path.GetTempFileName();
            var compiler = new ContentCompiler();
            try
            {
                using (var stream = new FileStream(temp, FileMode.Open, FileAccess.Write))
                {
                    compiler.Compile(
                        stream,
                        content,
                        MonoGamePlatform.Windows,
                        GraphicsProfile.Reach,
                        false,
                        Environment.CurrentDirectory,
                        Environment.CurrentDirectory);
                }
                byte[] result;
                using (var stream = new FileStream(temp, FileMode.Open, FileAccess.Read))
                {
                    stream.Position = 0;
                    using (var reader = new BinaryReader(stream))
                    {
                        result = reader.ReadBytes((int)stream.Length);
                    }
                }
                File.Delete(temp);
                return result;
            }
            finally
            {
                File.Delete(temp);
            }
        }
    }
}

#endif
