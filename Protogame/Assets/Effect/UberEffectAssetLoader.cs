using Protoinject;

namespace Protogame
{
    public class UberEffectAssetLoader : IAssetLoader
    {
        private readonly IKernel _kernel;

        private readonly IAssetContentManager _assetContentManager;

        public UberEffectAssetLoader(IKernel kernel, IAssetContentManager assetContentManager)
        {
            _kernel = kernel;
            this._assetContentManager = assetContentManager;
        }

        public bool CanHandle(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(UberEffectAssetLoader).FullName;
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
            return new UberEffectAsset(_kernel, this._assetContentManager, name, string.Empty, null, false);
        }
        
        public IAsset Handle(IAssetManager assetManager, string name, IRawAsset data)
        {
            if (data is CompiledAsset)
            {
                return new UberEffectAsset(_kernel, this._assetContentManager, name, null, data.GetProperty<PlatformData>("PlatformData"), false);
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

            var effect = new UberEffectAsset(
                _kernel,
                this._assetContentManager, 
                name,
                data.GetProperty<string>("Code"), 
                platformData, 
                data.GetProperty<bool>("SourcedFromRaw"));

            return effect;
        }
    }
}