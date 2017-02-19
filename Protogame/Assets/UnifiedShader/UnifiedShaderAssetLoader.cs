using Protoinject;

namespace Protogame
{
    public class UnifiedShaderAssetLoader : IAssetLoader
    {
        private readonly IKernel _kernel;
        private readonly IAssetContentManager _assetContentManager;

        public UnifiedShaderAssetLoader(IKernel kernel, IAssetContentManager assetContentManager)
        {
            _kernel = kernel;
            _assetContentManager = assetContentManager;
        }

        public bool CanLoad(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(UnifiedShaderAssetLoader).FullName;
        }

        public IAsset Load(string name, IRawAsset data)
        {
            if (data is CompiledAsset)
            {
                return new UnifiedShaderAsset(_kernel, _assetContentManager, name, null, data.GetProperty<PlatformData>("PlatformData"), false);
            }

            return new UnifiedShaderAsset(
                _kernel,
                _assetContentManager,
                name,
                data.GetProperty<string>("Code"),
                data.GetProperty<PlatformData>("PlatformData"),
                data.GetProperty<bool>("SourcedFromRaw"));
        }
    }
}
