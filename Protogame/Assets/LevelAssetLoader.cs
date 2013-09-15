using System;

namespace Protogame
{
    public class LevelAssetLoader : IAssetLoader
    {
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(LevelAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            return new LevelAsset(name, (string)data.Value, (string)data.SourcePath);
        }

        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            throw new InvalidOperationException();
        }
        
        public bool CanNew()
        {
            return true;
        }
        
        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return new LevelAsset(name, null, "");
        }
    }
}

