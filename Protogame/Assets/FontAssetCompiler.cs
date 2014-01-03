#if PLATFORM_WINDOWS || PLATFORM_MAC || PLATFORM_LINUX

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace Protogame
{
    public class FontAssetCompiler : MonoGameContentCompiler, IAssetCompiler<FontAsset>
    {
        public void Compile(FontAsset asset, TargetPlatform platform)
        {
            var chars = new List<CharacterRegion>();
            chars.Add(new CharacterRegion(' ', '~'));
            var fontName = string.IsNullOrEmpty(asset.FontName) ? "Arial" : asset.FontName;
            var description = new FontDescription(
                fontName,
                asset.FontSize,
                asset.Spacing,
                FontDescriptionStyle.Regular,
                asset.UseKerning,
                chars);
                
#if PLATFORM_WINDOWS
            if (IntPtr.Size != 4)
            {
                // Compilation of SpriteFonts requires that the process
                // is executing under 32-bit due to native dependencies.
                // We just return here and hope that the user has compiled
                // a version of the font.
                return;
            }
#endif

            try
            {
                var manager = new PipelineManager(Environment.CurrentDirectory, Environment.CurrentDirectory, Environment.CurrentDirectory);
                var dictionary = new OpaqueDataDictionary();
                var processor = manager.CreateProcessor("FontDescriptionProcessor", dictionary);
                var context = new DummyContentProcessorContext(TargetPlatformCast.ToMonoGamePlatform(platform));
                var content = processor.Process(description, context);

                asset.PlatformData = new PlatformData
                {
                    Platform = platform,
                    Data = this.CompileAndGetBytes(content)
                };

                try
                {
                    asset.ReloadFont();
                }
                catch (NotImplementedException)
                {
                    // We might be running under a server where we can't load
                    // the actual texture (because we have no game).
                }
            }
            catch (NullReferenceException)
            {
                // The user might not have the font installed...
            }
        }
    }
}

#endif