#if PLATFORM_WINDOWS || PLATFORM_LINUX

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Framework.Content.Pipeline.Builder;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace Protogame
{
    public class TextureAssetCompiler : MonoGameContentCompiler, IAssetCompiler
    {
        public string[] Extensions => new[] { "png" };

        public async Task CompileAsync(IAssetFsFile assetFile, IAssetDependencies assetDependencies, TargetPlatform platform, IWritableSerializedAsset output)
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

                var nameComponents = assetFile.Name.Split('.');
                nameComponents[nameComponents.Length - 1] = "_FolderOptions";
                var folderOptionsFile = await assetDependencies.GetOptionalCompileTimeFileDependency(string.Join(".", nameComponents)).ConfigureAwait(false);
                string[] importFolderOptions = null;
                if (folderOptionsFile != null)
                {
                    using (var optionsReader = new StreamReader(await folderOptionsFile.GetContentStream().ConfigureAwait(false)))
                    {
                        importFolderOptions = optionsReader.ReadToEnd()
                            .Trim()
                            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .Where(x => !x.StartsWith("#"))
                            .ToArray();
                    }
                }

                var optionsFile = await assetDependencies.GetOptionalCompileTimeFileDependency(assetFile.Name + ".Options").ConfigureAwait(false);
                string[] importOptions = null;
                if (optionsFile != null)
                {
                    using (var optionsReader = new StreamReader(await optionsFile.GetContentStream().ConfigureAwait(false)))
                    {
                        importOptions = optionsReader.ReadToEnd()
                            .Trim()
                            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .Where(x => !x.StartsWith("#"))
                            .ToArray();
                    }
                }

                if (importOptions == null)
                {
                    importOptions = importFolderOptions;
                }
                if (importOptions == null)
                {
                    importOptions = new string[0];
                }

                var allowResizeToPowerOfTwo = !IsPowerOfTwo((ulong)originalWidth) || !IsPowerOfTwo((ulong)originalHeight);
                var allowMakeSquare = originalWidth != originalHeight;

                var manager = new PipelineManager(
                    Environment.CurrentDirectory,
                    Environment.CurrentDirectory,
                    Environment.CurrentDirectory);
                TextureContent content = CompileTexture(assetFile, platform, monogameOutput, importOptions, allowResizeToPowerOfTwo, allowMakeSquare, manager);

                Console.WriteLine("Texture " + assetFile.Name + " resized " + originalWidth + "x" + originalHeight + " -> " + content.Faces[0][0].Width + "x" + content.Faces[0][0].Height);

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
        
        [HandleProcessCorruptedStateExceptions]
        private static TextureContent CompileTexture(IAssetFsFile assetFile, TargetPlatform platform, TextureContent monogameOutput, string[] importOptions, bool allowResizeToPowerOfTwo, bool allowMakeSquare, PipelineManager manager)
        {
            var dictionary = new OpaqueDataDictionary();
            dictionary["GenerateMipmaps"] = importOptions.Contains("GenerateMipmaps");
            dictionary["ResizeToPowerOfTwo"] = allowResizeToPowerOfTwo && !importOptions.Contains("NoResizeToPowerOfTwo");
            dictionary["MakeSquare"] = allowMakeSquare && !importOptions.Contains("MakeSquare");
            dictionary["TextureFormat"] = importOptions.Contains("NoCompress") ? TextureProcessorOutputFormat.Color : TextureProcessorOutputFormat.Compressed;
            var processor = manager.CreateProcessor("TextureProcessor", dictionary);
            var context = new DummyContentProcessorContext(TargetPlatformCast.ToMonoGamePlatform(platform));
            TextureContent content;

            try
            {
                content = (TextureContent)processor.Process(monogameOutput, context);
            }
            catch (AccessViolationException)
            {
                // Sometimes the nvtt compressor crashes on some textures.  Workaround by turning off compression
                // in this case.
                Console.WriteLine("WARNING: Disabling texture compression on " + assetFile.Name + " due to nvtt bugs");
                dictionary["TextureFormat"] = TextureProcessorOutputFormat.Color;
                processor = manager.CreateProcessor("TextureProcessor", dictionary);
                context = new DummyContentProcessorContext(TargetPlatformCast.ToMonoGamePlatform(platform));
                content = (TextureContent)processor.Process(monogameOutput, context);
            }

            return content;
        }

        private bool IsPowerOfTwo(ulong x)
        {
            return (x & (x - 1)) == 0;
        }
    }
}

#endif