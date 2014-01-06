#if PLATFORM_WINDOWS || PLATFORM_LINUX

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace Protogame
{
    public class TextureAssetCompiler : MonoGameContentCompiler, IAssetCompiler<TextureAsset>
    {
        public void Compile(TextureAsset asset, TargetPlatform platform)
        {
            if (asset.RawData == null)
            {
                return;
            }

            var output = new Texture2DContent();
            var bitmap = new Bitmap(new MemoryStream(asset.RawData));
            var width = bitmap.Width;
            var height = bitmap.Height;
            if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                var newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (var graphics = Graphics.FromImage(newBitmap))
                    graphics.DrawImage(bitmap, 0, 0, width, height);
                bitmap = newBitmap;
            }
            var imageData = bitmap.GetData();
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            output.GetType().GetField("_bitmap", flags).SetValue(output, bitmap);
            var bitmapContent = new PixelBitmapContent<Microsoft.Xna.Framework.Color>(width, height);
            bitmapContent.SetPixelData(imageData);
            output.Faces.Add(new MipmapChain(bitmapContent));

            var manager = new PipelineManager(Environment.CurrentDirectory, Environment.CurrentDirectory, Environment.CurrentDirectory);
            var dictionary = new OpaqueDataDictionary();
            var processor = manager.CreateProcessor("TextureProcessor", dictionary);
            var context = new DummyContentProcessorContext(TargetPlatformCast.ToMonoGamePlatform(platform));
            var content = processor.Process(output, context);

            asset.PlatformData = new PlatformData
            {
                Platform = platform,
                Data = this.CompileAndGetBytes(content)
            };

            try
            {
                asset.ReloadTexture();
            }
            catch (NoAssetContentManagerException)
            {
            }
        }
    }
}

#endif
