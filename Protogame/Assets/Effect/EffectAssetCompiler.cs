#if PLATFORM_WINDOWS

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Protogame
{
    public class EffectAssetCompiler : BaseEffectAssetLoader, IAssetCompiler
    {
        public string[] Extensions => new[] { "fx" };

        public async Task CompileAsync(IAssetFsFile assetFile, IAssetDependencies assetDependencies, TargetPlatform platform, ISerializedAsset output)
        {
            var content = await assetFile.GetContentStream().ConfigureAwait(false);
            var code = string.Empty;
            using (var reader = new StreamReader(content))
            {
                code = await reader.ReadToEndAsync();
            }

            if (code.Contains("// uber"))
            {
                // Do nothing with this file.
                return;
            }

            var dirName = Path.GetDirectoryName(assetFile.Name.Replace(".", "/"));
            code = await ResolveIncludes(assetDependencies, dirName.Replace(Path.DirectorySeparatorChar, '.'), code).ConfigureAwait(false);

            var effectContent = new EffectContent();
            effectContent.EffectCode = this.GetEffectPrefixCode() + code;

            var tempPath = Path.GetTempFileName();
            using (var writer = new StreamWriter(tempPath))
            {
                writer.Write(effectContent.EffectCode);
            }

            effectContent.Identity = new ContentIdentity(tempPath);

            var tempOutputPath = Path.GetTempFileName();

            var debugContent = EffectCompilerHelper.Compile(
                effectContent,
                tempOutputPath,
                platform,
                true,
                string.Empty);
            var releaseContent = EffectCompilerHelper.Compile(
                effectContent,
                tempOutputPath,
                platform,
                false,
                string.Empty);

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    // Magic flag that indicates new compiled effect format.
                    writer.Write((uint)0x12345678);

                    // Version 1 of new effect format.
                    writer.Write((uint)1);

                    var debugCode = debugContent.GetEffectCode();
                    var releaseCode = releaseContent.GetEffectCode();

                    writer.Write(debugCode.Length);
                    writer.Write(debugCode);
                    writer.Write(releaseCode.Length);
                    writer.Write(releaseCode);

                    var p = stream.Position;
                    var b = new byte[p];
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(b, 0, b.Length);

                    output.SetLoader<IAssetLoader<EffectAsset>>();
                    output.SetPlatform(platform);
                    output.SetByteArray("Data", b);
                }
            }

            File.Delete(tempPath);
            File.Delete(tempOutputPath);
        }

        /// <summary>
        /// Gets the prefix code for effects compiled under Protogame.
        /// </summary>
        /// <returns>The effect prefix code.</returns>
        private string GetEffectPrefixCode()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("Protogame.Assets.Effect.Macros.fx");

            if (stream == null)
            {
                throw new InvalidOperationException("Prefix code for effects could not be found");
            }

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}

#endif
