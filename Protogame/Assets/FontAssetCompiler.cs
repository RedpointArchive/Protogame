#if PLATFORM_WINDOWS || PLATFORM_LINUX

namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Content.Pipeline;
    using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
    using MonoGame.Framework.Content.Pipeline.Builder;

    /// <summary>
    /// The font asset compiler.
    /// </summary>
    public class FontAssetCompiler : MonoGameContentCompiler, IAssetCompiler<FontAsset>
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
        public void Compile(FontAsset asset, TargetPlatform platform)
        {
            if (IntPtr.Size == 4)
            {
                throw new NotSupportedException("Font compilation is only supported under a 64-bit process.");
            }

            this.CompileFont(asset, platform);
        }

        /// <summary>
        /// The compile font.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <param name="platform">
        /// The platform.
        /// </param>
        private void CompileFont(FontAsset asset, TargetPlatform platform)
        {
            foreach (var fontDesc in this.GetDescriptionsForAsset(asset))
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

                    asset.PlatformData = new PlatformData
                    {
                        Platform = platform,
                        Data = this.CompileAndGetBytes(content)
                    };

                    try
                    {
                        asset.ReloadFont();
                    }
                    catch (NoAssetContentManagerException)
                    {
                        // We might be running under a server where we can't load
                        // the actual texture (because we have no game).
                    }

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
        }

        /// <summary>
        /// The get descriptions for asset.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable" />.
        /// </returns>
        private IEnumerable<FontDescription> GetDescriptionsForAsset(FontAsset asset)
        {
            var fontNames = string.IsNullOrEmpty(asset.FontName) ? "Arial" : asset.FontName;

            foreach (var fontName in fontNames.Split(','))
            {
                var fontDesc = new FontDescription(
                    fontName,
                    asset.FontSize,
                    asset.Spacing,
                    FontDescriptionStyle.Regular,
                    asset.UseKerning);

                // Add the entire ASCII range of characters to the sprite font.
                for (var c = 0; c < 256; c++)
                {
                    fontDesc.Characters.Add((char)c);
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