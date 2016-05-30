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

        public bool CanHandle(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(UnifiedShaderAssetLoader).FullName;
        }

        public bool CanNew()
        {
            return true;
        }

        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            return null;
        }

        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return new UnifiedShaderAsset(_kernel, _assetContentManager, name, string.Empty, null, false);
        }

        public IAsset Handle(IAssetManager assetManager, string name, IRawAsset data)
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
