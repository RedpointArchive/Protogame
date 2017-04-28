#if PLATFORM_WINDOWS || PLATFORM_LINUX

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Protogame
{
    public class ModelAssetCompiler : IAssetCompiler
    {
        private readonly IModelRenderConfiguration[] _modelRenderConfigurations;
        private readonly IRenderBatcher _renderBatcher;
        private readonly IModelSerializer _modelSerializer;

        public ModelAssetCompiler(IModelRenderConfiguration[] modelRenderConfigurations, IRenderBatcher renderBatcher, IModelSerializer modelSerializer)
        {
            _modelRenderConfigurations = modelRenderConfigurations;
            _renderBatcher = renderBatcher;
            _modelSerializer = modelSerializer;
        }

        public string[] Extensions => new[] { "fbx", "x", "dae" };

        public async Task CompileAsync(IAssetFsFile assetFile, IAssetDependencies assetDependencies, TargetPlatform platform, ISerializedAsset output)
        {
            var otherAnimations = new Dictionary<string, byte[]>();
            if (assetFile.Extension != "x")
            {
                var otherFiles = (await assetDependencies.GetAvailableCompileTimeFiles())
                    .Where(x => x.Name.StartsWith(assetFile.Name + "-"))
                    .ToArray();
                foreach (var otherAnim in otherFiles)
                {
                    using (var otherStream = await otherAnim.GetContentStream().ConfigureAwait(false))
                    {
                        var b = new byte[otherStream.Length];
                        await otherStream.ReadAsync(b, 0, b.Length).ConfigureAwait(false);
                        otherAnimations[otherAnim.Name.Substring((assetFile.Name + "-").Length)] = b;
                    }
                }
            }

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

            byte[] modelData;
            using (var stream = await assetFile.GetContentStream().ConfigureAwait(false))
            {
                modelData = new byte[stream.Length];
                await stream.ReadAsync(modelData, 0, modelData.Length).ConfigureAwait(false);
            }
            
            var reader = new AssimpReader(_modelRenderConfigurations, _renderBatcher);
            var model = reader.Load(modelData, assetFile.Name, assetFile.Extension, otherAnimations, importOptions);
            var data = _modelSerializer.Serialize(model);

            output.SetLoader<IAssetLoader<ModelAsset>>();
            output.SetByteArray("Data", data);
        }
    }
}

#endif