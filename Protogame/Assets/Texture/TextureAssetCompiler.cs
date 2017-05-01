#if PLATFORM_WINDOWS || PLATFORM_LINUX

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Framework.Content.Pipeline.Builder;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Linq;

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
                var dictionary = new OpaqueDataDictionary();
                dictionary["GenerateMipmaps"] = importOptions.Contains("GenerateMipmaps");
                dictionary["ResizeToPowerOfTwo"] = allowResizeToPowerOfTwo && !importOptions.Contains("NoResizeToPowerOfTwo");
                dictionary["MakeSquare"] = allowMakeSquare && !importOptions.Contains("MakeSquare");
                dictionary["TextureFormat"] = importOptions.Contains("NoCompress") ? TextureProcessorOutputFormat.Color : TextureProcessorOutputFormat.Compressed;
                var processor = manager.CreateProcessor("TextureProcessor", dictionary);
                var context = new DummyContentProcessorContext(TargetPlatformCast.ToMonoGamePlatform(platform));
                var content = (TextureContent)processor.Process(monogameOutput, context);

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

        private bool IsPowerOfTwo(ulong x)
        {
            return (x & (x - 1)) == 0;
        }
    }
}

#endif