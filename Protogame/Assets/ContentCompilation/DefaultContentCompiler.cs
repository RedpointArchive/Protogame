#if PLATFORM_WINDOWS || PLATFORM_MAC || PLATFORM_LINUX

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;

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
            using (var stream = new MemoryStream())
            {
                compiler.Compile(stream, content, context.TargetPlatform, context.TargetProfile, false, Environment.CurrentDirectory, Environment.CurrentDirectory);
                return stream.GetBuffer();
            }
        }
    }
}

#endif
