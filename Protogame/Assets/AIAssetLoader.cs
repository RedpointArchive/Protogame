using System;

namespace Protogame
{
    public class AIAssetLoader : IAssetLoader
    {
        public bool CanHandle(dynamic data)
        {
            return typeof(AIAsset).IsAssignableFrom(data.type);
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            return (IAsset)Activator.CreateInstance(data.type);
        }

        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            throw new NotSupportedException();
        }
        
        public bool CanNew()
        {
            return false;
        }
        
        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            throw new NotSupportedException();
        }
    }
}
