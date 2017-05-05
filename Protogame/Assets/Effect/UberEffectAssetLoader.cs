using Protoinject;
using System.Threading.Tasks;

namespace Protogame
{
    public class UberEffectAssetLoader : IAssetLoader<UberEffectAsset>
    {
        private readonly IKernel _kernel;
        private readonly IAssetContentManager _assetContentManager;
        private readonly IRawLaunchArguments _rawLaunchArguments;

        public UberEffectAssetLoader(IKernel kernel, IAssetContentManager assetContentManager, IRawLaunchArguments rawLaunchArguments)
        {
            _kernel = kernel;
            _assetContentManager = assetContentManager;
            _rawLaunchArguments = rawLaunchArguments;
        }

        public async Task<IAsset> Load(string name, IReadableSerializedAsset input, IAssetManager assetManager)
        {
            return new UberEffectAsset(
                _kernel,
                _assetContentManager,
                _rawLaunchArguments,
                name,
                input.GetByteArray("Data"));
        }
    }
}