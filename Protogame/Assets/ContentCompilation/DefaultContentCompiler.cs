#if PLATFORM_WINDOWS || PLATFORM_MAC || PLATFORM_LINUX

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Protogame
{
    public class DefaultContentCompiler : IContentCompiler
    {
        public byte[] BuildSpriteFont(string fontName, float size, float spacing, bool useKerning)
        {
            var chars = new List<char>();
            for (int i = 0; i < 256; i++)
                chars.Add((char)i);
            var description = new FontDescription(
                fontName,
                size,
                spacing,
                FontDescriptionStyle.Regular,
                useKerning,
                chars);
            var manager = new PipelineManager(null, null, null);
            var dictionary = new OpaqueDataDictionary();
            var processor = manager.CreateProcessor("FontDescriptionProcessor", dictionary);
            var context = new DummyContentProcessorContext(TargetPlatform.Linux);
            var content = processor.Process(description, context);
            var compiler = new Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler.ContentCompiler();
            var temp = Path.GetTempFileName();
            try
            {
                using (var stream = new FileStream(temp, FileMode.Open, FileAccess.Write))
                {
                    compiler.Compile(stream, content, TargetPlatform.Windows, GraphicsProfile.Reach, false, Environment.CurrentDirectory, Environment.CurrentDirectory);
                }
                using (var stream = new FileStream(temp, FileMode.Open, FileAccess.Read))
                {
                    stream.Position = 0;
                    using (var reader = new BinaryReader(stream))
                    {
                        var result = reader.ReadBytes((int)stream.Length);
                        File.Delete(temp);
                        return result;
                    }
                }
            }
            finally
            {
                File.Delete(temp);
            }
        }
    }
}

#endif
