
#if PLATFORM_WINDOWS || PLATFORM_LINUX

namespace Protogame
{
    /// <summary>
    /// The model asset compiler.
    /// </summary>
    public class ModelAssetCompiler : IAssetCompiler<ModelAsset>
    {
        private readonly IModelRenderConfiguration[] _modelRenderConfigurations;
        private readonly IModelSerializer _modelSerializer;

        public ModelAssetCompiler(IModelRenderConfiguration[] modelRenderConfigurations, IModelSerializer modelSerializer)
        {
            _modelRenderConfigurations = modelRenderConfigurations;
            _modelSerializer = modelSerializer;
        }

        /// <summary>
        /// The compile.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <param name="platform">
        /// The platform.
        /// </param>
        public void Compile(ModelAsset asset, TargetPlatform platform)
        {
            if (asset.RawData == null)
            {
                return;
            }

            var reader = new FbxReader(_modelRenderConfigurations);
            var model = reader.Load(asset.RawData, asset.Name, asset.Extension, asset.RawAdditionalAnimations);
            var data = _modelSerializer.Serialize(model);

            asset.PlatformData = new PlatformData { Data = data, Platform = platform };

            try
            {
                asset.ReloadModel();
            }
            catch (NoAssetContentManagerException)
            {
            }
        }
    }
}

#endif