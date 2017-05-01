#if PLATFORM_WINDOWS || PLATFORM_LINUX

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Framework.Content.Pipeline.Builder;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace Protogame
{
    public class TextureAssetCompiler : MonoGameContentCompiler, IAssetCompiler
    {
        public string[] Extensions => new[] { "png" };

        public async Task CompileAsync(IAssetFsFile assetFile, IAssetDependencies assetDependencies, TargetPlatform platform, ISerializedAsset output)
        {
            var tempPath = System.IO.Path.GetTempFileName();
            try
            {
                using (var stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (var sourceStream = await assetFile.GetContentStream().ConfigureAwait(false))
                    {
                        await sourceStream.CopyToAsync(stream).ConfigureAwait(false);
                    }
                }

                var importer = new TextureImporter();
                var monogameOutput = importer.Import(tempPath, new DummyContentImporterContext());

                var originalWidth = monogameOutput.Faces[0][0].Width;
                var originalHeight = monogameOutput.Faces[0][0].Height;

                var manager = new PipelineManager(
                    Environment.CurrentDirectory,
                    Environment.CurrentDirectory,
                    Environment.CurrentDirectory);
                var dictionary = new OpaqueDataDictionary();
                dictionary["GenerateMipmaps"] = true;
                dictionary["ResizeToPowerOfTwo"] = true;
                dictionary["MakeSquare"] = true;
                dictionary["TextureFormat"] = TextureProcessorOutputFormat.Compressed;
                var processor = manager.CreateProcessor("TextureProcessor", dictionary);
                var context = new DummyContentProcessorContext(TargetPlatformCast.ToMonoGamePlatform(platform));
                var content = processor.Process(monogameOutput, context);

                output.SetLoader<IAssetLoader<TextureAsset>>();
                output.SetPlatform(platform);
                output.SetByteArray("Data", CompileAndGetBytes(content));
                output.SetInt32("OriginalWidth", originalWidth);
                output.SetInt32("OriginalHeight", originalHeight);
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