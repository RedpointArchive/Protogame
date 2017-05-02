using Protoinject;
using System;
using System.Threading.Tasks;

namespace Protogame
{
    public class EffectAssetLoader : IAssetLoader<EffectAsset>
    {
        private readonly IKernel _kernel;
        private readonly IAssetContentManager _assetContentManager;
        private readonly IRawLaunchArguments _rawLaunchArguments;

        public EffectAssetLoader(IKernel kernel, IAssetContentManager assetContentManager, IRawLaunchArguments rawLaunchArguments)
        {
            _kernel = kernel;
            _assetContentManager = assetContentManager;
            _rawLaunchArguments = rawLaunchArguments;
        }

        public async Task<IAsset> Load(string name, IReadableSerializedAsset input, IAssetManager assetManager)
        {
            return new EffectAsset(
                _kernel,
                _assetContentManager,
                _rawLaunchArguments,
                name,
                input.GetByteArray("Data"));
        }
    }
}
