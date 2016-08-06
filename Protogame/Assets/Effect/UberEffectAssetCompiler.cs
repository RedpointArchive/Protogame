#if PLATFORM_WINDOWS

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace Protogame
{
    public class UberEffectAssetCompiler : IAssetCompiler<UberEffectAsset>
    {
        public void Compile(UberEffectAsset asset, TargetPlatform platform)
        {
            if (string.IsNullOrEmpty(asset.Code))
            {
                return;
            }

            var allPassed = true;
            var effectCodes = new Dictionary<string, Tuple<string, byte[]>>();
            foreach (var rawLine in asset.Code.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
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

                var output = new EffectContent();
                output.EffectCode = this.GetEffectPrefixCode() + asset.Code;

                string tempPath = null, tempOutputPath = null;
                try
                {
                    tempPath = Path.GetTempFileName();
                    tempOutputPath = Path.GetTempFileName();

                    using (var writer = new StreamWriter(tempPath))
                    {
                        writer.Write(output.EffectCode);
                    }

                    output.Identity = new ContentIdentity(tempPath);

                    var processor = new EffectProcessor();
                    processor.Defines = defines;
                    var context = new DummyContentProcessorContext(TargetPlatformCast.ToMonoGamePlatform(platform));
                    context.ActualOutputFilename = tempOutputPath;
                    var content = processor.Process(output, context);
                    effectCodes[name] = new Tuple<string, byte[]>(defines, content.GetEffectCode());
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
                    writer.Write((uint)1);
                    writer.Write((uint)effectCodes.Count);
                    foreach (var kv in effectCodes)
                    {
                        writer.Write(kv.Key);
                        writer.Write(kv.Value.Item1);
                        writer.Write(kv.Value.Item2.Length);
                        writer.Write(kv.Value.Item2);
                    }

                    var len = memory.Position;
                    var data = new byte[len];
                    memory.Seek(0, SeekOrigin.Begin);
                    memory.Read(data, 0, data.Length);
                    asset.PlatformData = new PlatformData { Platform = platform, Data = data };
                }
            }

            try
            {
                asset.ReloadEffects();
            }
            catch (NoAssetContentManagerException)
            {
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