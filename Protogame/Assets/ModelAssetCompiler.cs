#if PLATFORM_WINDOWS

namespace Protogame
{
    public class ModelAssetCompiler : IAssetCompiler<ModelAsset>
    {
        public void Compile(ModelAsset asset, TargetPlatform platform)
        {
            if (asset.RawData == null)
            {
                return;
            }

            var reader = new FbxReader();
            var model = reader.Load(asset.RawData, asset.RawAdditionalAnimations);
            var serializer = new ModelSerializer();
            var data = serializer.Serialize(model);

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
