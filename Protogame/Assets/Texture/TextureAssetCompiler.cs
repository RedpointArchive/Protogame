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

            var tempPath = System.IO.Path.GetTempFileName();
            try
            {
                using (var stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    stream.Write(asset.RawData, 0, asset.RawData.Length);
                }

                var importer = new TextureImporter();
                var output = importer.Import(tempPath, new DummyContentImporterContext());

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
            finally
            {
                try
                {
                    File.Delete(tempPath);
                }
                catch
                {
                }
            }
        }
    }
}

#endif