using Protoinject;

namespace Protogame
{
    public class AssetManagerProviderInitializer : IAssetManagerProviderInitializer
    {
        private readonly IKernel _kernel;
        private readonly string[] _args;

        public AssetManagerProviderInitializer(IKernel kernel, string[] args)
        {
            _kernel = kernel;
            _args = args;
        }

        public void Initialize<T>() where T : IAssetManagerProvider
        {
            AssetManagerClient.AcceptArgumentsAndSetup<T>(_kernel, _args);
        }
    }
}