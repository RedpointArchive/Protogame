#if PLATFORM_WINDOWS || PLATFORM_MAC || PLATFORM_LINUX

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace Protogame
{
    public class DefaultContentCompiler : IContentCompiler
    {
        private byte[] CompileAndGetBytes(object content)
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
                        TargetPlatform.Windows, 
                        GraphicsProfile.Reach, 
                        false, 
                        Environment.CurrentDirectory, 
                        Environment.CurrentDirectory);
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
        
        public byte[] BuildSpriteFont(string fontName, float size, float spacing, bool useKerning)
        {
            var chars = new List<CharacterRegion>();
            chars.Add(new CharacterRegion((char)0, (char)255));
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
            return this.CompileAndGetBytes(content);
        }
        
        public byte[] BuildTexture2D(Stream source)
        {
            var output = new Texture2DContent();
            var bitmap = new Bitmap(source);
            var width = bitmap.Width;
            var height = bitmap.Height;
            if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                var newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (var graphics = Graphics.FromImage(bitmap))
                    graphics.DrawImage(bitmap, 0,0, width, height);
                bitmap = newBitmap;
            }
            var imageData = bitmap.GetData();
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            output.GetType().GetField("_bitmap", flags).SetValue(output, bitmap);
            var bitmapContent = new PixelBitmapContent<Microsoft.Xna.Framework.Color>(width, height);
            bitmapContent.SetPixelData(imageData);
            output.Faces.Add(new MipmapChain(bitmapContent));
            
            var manager = new PipelineManager(null, null, null);
            var dictionary = new OpaqueDataDictionary();
            var processor = manager.CreateProcessor("TextureProcessor", dictionary);
            var context = new DummyContentProcessorContext(TargetPlatform.Linux);
            var content = processor.Process(output, context);
            return this.CompileAndGetBytes(content);
        }
        
        public byte[] BuildSoundEffect(Stream source)
        {
            throw new InvalidOperationException();
        }
    }
}

#endif
