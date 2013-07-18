using System;

namespace Protogame
{
    public class LevelAssetLoader : IAssetLoader
    {
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(LevelAssetLoader).FullName;
        }

        public IAsset Handle(string name, dynamic data)
        {
            return new LevelAsset(name, data.Value);
        }

        public IAsset GetDefault(string name)
        {
            throw new InvalidOperationException();
        }
    }
}

