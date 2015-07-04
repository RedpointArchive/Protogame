#if PLATFORM_WINDOWS || PLATFORM_LINUX

using System;
using System.Drawing;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace Protogame
{
    using System.Drawing.Imaging;
    using System.Reflection;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The texture asset compiler.
    /// </summary>
    public class TextureAssetCompiler : MonoGameContentCompiler, IAssetCompiler<TextureAsset>
    {
        /// <summary>
        /// The compile.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <param name="platform">
        /// The platform.
        /// </param>
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

            if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                throw new InvalidDataException(
                    "8-bit indexed PNGs do not convert correctly (at least under Mono) to 32-bit ARGB.  Save " +
                    "your PNG files in a non-indexed format, such as 24-bit or 32-bit ARGB.");
            }

            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                var newBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                using (var graphics = Graphics.FromImage(newBitmap)) graphics.DrawImage(bitmap, 0, 0, width, height);
                bitmap = newBitmap;
            }

            output.Faces[0] = new MipmapChain(bitmap.ToXnaBitmap(true));
            bitmap.Dispose();

            var manager = new PipelineManager(
                Environment.CurrentDirectory, 
                Environment.CurrentDirectory, 
                Environment.CurrentDirectory);
            var dictionary = new OpaqueDataDictionary();
            var processor = manager.CreateProcessor("TextureProcessor", dictionary);
            var context = new DummyContentProcessorContext(TargetPlatformCast.ToMonoGamePlatform(platform));
            var content = processor.Process(output, context);

            asset.PlatformData = new PlatformData { Platform = platform, Data = this.CompileAndGetBytes(content) };

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