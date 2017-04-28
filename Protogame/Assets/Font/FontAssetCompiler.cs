#if PLATFORM_WINDOWS || PLATFORM_LINUX

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Protogame
{    
    public class FontAssetCompiler : MonoGameContentCompiler, IAssetCompiler
    {
        public string[] Extensions => new[] { "font" };

        private class FontDefinition
        {
            [JsonProperty("FontSize")]
            public int FontSize { get; set; }

            [JsonProperty("FontName")]
            public string FontName { get; set; }

            [JsonProperty("UseKerning")]
            public bool UseKerning { get; set; }

            [JsonProperty("Spacing")]
            public int Spacing { get; set; }

            [JsonProperty("CharacterRanges")]
            public CharacterRange[] CharacterRanges { get; set; }
        }

        private class CharacterRange
        {
            [JsonProperty("Start")]
            public int Start { get; set; }

            [JsonProperty("End")]
            public int End { get; set; }
        }

        public async Task CompileAsync(IAssetFsFile assetFile, IAssetDependencies assetDependencies, TargetPlatform platform, ISerializedAsset output)
        {
            if (IntPtr.Size == 4)
            {
                throw new NotSupportedException("Font compilation is only supported under a 64-bit process.");
            }

            var fileContent = await assetFile.GetContentStream().ConfigureAwait(false);
            var json = string.Empty;
            using (var reader = new StreamReader(fileContent))
            {
                json = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            var fontDefinition = JsonConvert.DeserializeObject<FontDefinition>(json);

            foreach (var fontDesc in this.GetDescriptionsForAsset(fontDefinition))
            {
                try
                {
                    var manager = new PipelineManager(
                        Environment.CurrentDirectory,
                        Environment.CurrentDirectory,
                        Environment.CurrentDirectory);
                    var dictionary = new OpaqueDataDictionary();
                    var processor = manager.CreateProcessor("FontDescriptionProcessor", dictionary);
                    var context = new DummyContentProcessorContext(TargetPlatformCast.ToMonoGamePlatform(platform));
                    var content = processor.Process(fontDesc, context);

                    output.SetLoader<IAssetLoader<FontAsset>>();
                    output.SetPlatform(platform);
                    output.SetByteArray("Data", CompileAndGetBytes(content));
                    output.SetString("FontName", fontDesc.FontName);
                    output.SetFloat("FontSize", fontDesc.Size);
                    output.SetBoolean("UseKerning", fontDesc.UseKerning);
                    output.SetFloat("Spacing", fontDesc.Spacing);

                    // Font compilation was successful.
                    return;
                }
                catch (ArgumentOutOfRangeException)
                {
                    // The user might not have the font installed...
                }
                catch (NullReferenceException)
                {
                    // The user might not have the font installed...
                }
            }
            
            // TODO: Throw maybe?
        }

        private IEnumerable<FontDescription> GetDescriptionsForAsset(FontDefinition definition)
        {
            var fontNames = string.IsNullOrEmpty(definition.FontName) ? "Arial" : definition.FontName;

            foreach (var fontName in fontNames.Split(','))
            {
                var fontDesc = new FontDescription(
                    fontName,
                    definition.FontSize,
                    definition.Spacing,
                    FontDescriptionStyle.Regular,
                    definition.UseKerning);

                if (definition.CharacterRanges == null || definition.CharacterRanges.Length == 0)
                {
                    // Add the entire ASCII range of characters to the sprite font.
                    for (var c = 0; c < 256; c++)
                    {
                        fontDesc.Characters.Add((char)c);
                    }
                }
                else
                {
                    foreach (var range in definition.CharacterRanges)
                    {
                        for (var c = range.Start; c <= range.End; c++)
                        {
                            fontDesc.Characters.Add((char)c);
                        }
                    }
                }

#if PLATFORM_LINUX
                fontDesc.Identity = new ContentIdentity
                {
                    SourceFilename = "/usr/share/fonts/truetype/dummy.spritefont"
                };
#endif
                yield return fontDesc;
            }
        }
    }
}

#endif