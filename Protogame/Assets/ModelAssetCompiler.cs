#if PLATFORM_WINDOWS

namespace Protogame
{
    public class ModelAssetCompiler : IAssetCompiler<ModelAsset>
    {
        public void Compile(ModelAsset asset, TargetPlatform platform)
        {
            if (asset.SourceData == null)
            {
                return;
            }

            var reader = new FbxReader();
            var model = reader.Load(asset.SourceData);
            var serializer = new ModelSerializer();
            var data = serializer.Serialize(model);

            asset.CompiledData = data;

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
