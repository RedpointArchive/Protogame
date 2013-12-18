#if PLATFORM_WINDOWS || PLATFORM_MAC || PLATFORM_LINUX

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace Protogame
{
    public class EffectAssetCompiler : MonoGameContentCompiler, IAssetCompiler<EffectAsset>
    {
        public void Compile(EffectAsset asset, TargetPlatform platform)
        {
            var output = new EffectContent();
            using (var reader = new StreamReader(asset.SourcePath))
            {
                output.EffectCode = reader.ReadToEnd();
            }

            var manager = new PipelineManager(Environment.CurrentDirectory, Environment.CurrentDirectory, Environment.CurrentDirectory);
            var dictionary = new OpaqueDataDictionary();
            var processor = manager.CreateProcessor("EffectProcessor", dictionary);
            var context = new DummyContentProcessorContext(TargetPlatformCast.ToMonoGamePlatform(platform));
            var content = processor.Process(output, context);

            asset.PlatformData = new PlatformData
            {
                Platform = platform,
                Data = this.CompileAndGetBytes(content)
            };
        }
    }
}

#endif
