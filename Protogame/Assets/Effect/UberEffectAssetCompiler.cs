#if PLATFORM_WINDOWS

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Threading.Tasks;

namespace Protogame
{
    public class UberEffectAssetCompiler : BaseEffectAssetLoader, IAssetCompiler
    {
        public string[] Extensions => new[] { "fx" };

        public async Task CompileAsync(IAssetFsFile assetFile, IAssetDependencies assetDependencies, TargetPlatform platform, IWritableSerializedAsset output)
        {
            var code = string.Empty;
            using (var reader = new StreamReader(await assetFile.GetContentStream().ConfigureAwait(false)))
            {
                code = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            if (!code.Contains("// uber"))
            {
                // Do nothing with this file.
                return;
            }

            var dirName = Path.GetDirectoryName(assetFile.Name.Replace(".", "/"));
            code = await ResolveIncludes(assetDependencies, dirName.Replace(Path.DirectorySeparatorChar, '.'), code).ConfigureAwait(false);

            var allPassed = true;
            var effectCodes = new Dictionary<string, Tuple<string, byte[], byte[]>>();
            foreach (var rawLine in code.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var line = rawLine.Trim();
                if (line.StartsWith("#line"))
                {
                    continue;
                }
                if (!line.StartsWith("// uber "))
                {
                    break;
                }

                var components = line.Substring("// uber ".Length).Split(':');
                var name = components[0].Trim();
                var defines = components[1].Trim();

                Console.WriteLine();
                Console.Write("Compiling uber shader variant " + name + "... ");

                var effectOutput = new EffectContent();
                effectOutput.EffectCode = this.GetEffectPrefixCode() + code;

                string tempPath = null, tempOutputPath = null;
                try
                {
                    tempPath = Path.GetTempFileName();
                    tempOutputPath = Path.GetTempFileName();

                    using (var writer = new StreamWriter(tempPath))
                    {
                        writer.Write(effectOutput.EffectCode);
                    }

                    effectOutput.Identity = new ContentIdentity(tempPath);

                    var debugContent = EffectCompilerHelper.Compile(
                        effectOutput,
                        tempOutputPath,
                        platform,
                        true,
                        defines);
                    var releaseContent = EffectCompilerHelper.Compile(
                        effectOutput,
                        tempOutputPath,
                        platform,
                        false,
                        defines);
                    
                    effectCodes[name] = new Tuple<string, byte[], byte[]>(defines, debugContent.GetEffectCode(), releaseContent.GetEffectCode());
                    Console.Write("done.");
                }
                catch (InvalidContentException ex)
                {
                    Console.WriteLine("failed.");
                    Console.Write(ex.Message.Trim());
                    allPassed = false;
                }
                finally
                {
                    if (tempOutputPath != null)
                    {
                        File.Delete(tempOutputPath);
                    }

                    if (tempOutputPath != null)
                    {
                        File.Delete(tempPath);
                    }
                }
            }
            
            Console.WriteLine();
            Console.Write("Finalizing uber shader compilation... ");

            if (!allPassed)
            {
                throw new Exception("One or more uber shader variants failed to compile (see above!)");
            }

            using (var memory = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memory))
                {
                    writer.Write((uint)2);
                    writer.Write((uint)effectCodes.Count);
                    foreach (var kv in effectCodes)
                    {
                        writer.Write(kv.Key);
                        writer.Write(kv.Value.Item1);
                        writer.Write(kv.Value.Item2.Length);
                        writer.Write(kv.Value.Item2);
                        writer.Write(kv.Value.Item3.Length);
                        writer.Write(kv.Value.Item3);
                    }

                    var len = memory.Position;
                    var data = new byte[len];
                    memory.Seek(0, SeekOrigin.Begin);
                    memory.Read(data, 0, data.Length);

                    output.SetLoader<IAssetLoader<UberEffectAsset>>();
                    output.SetPlatform(platform);
                    output.SetByteArray("Data", data);
                }
            }
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