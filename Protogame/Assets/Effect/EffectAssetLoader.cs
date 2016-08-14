using Protoinject;

namespace Protogame
{
    public class EffectAssetLoader : IAssetLoader
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
        
        public bool CanHandle(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(EffectAssetLoader).FullName;
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
            return new EffectAsset(_kernel, _assetContentManager, _rawLaunchArguments, name, string.Empty, null, false);
        }
        
        public IAsset Handle(IAssetManager assetManager, string name, IRawAsset data)
        {
            if (data is CompiledAsset)
            {
                return new EffectAsset(_kernel, _assetContentManager, _rawLaunchArguments, name, null, data.GetProperty<PlatformData>("PlatformData"), false);
            }

            PlatformData platformData = null;
            if (data.GetProperty<PlatformData>("PlatformData") != null)
            {
                platformData = new PlatformData
                {
                    Platform = data.GetProperty<PlatformData>("PlatformData").Platform,
                    Data = data.GetProperty<PlatformData>("PlatformData").Data
                };
            }

            var effect = new EffectAsset(
                _kernel,
                _assetContentManager, 
                _rawLaunchArguments,
                name,
                data.GetProperty<string>("Code"), 
                platformData, 
                data.GetProperty<bool>("SourcedFromRaw"));

            return effect;
        }
    }
}