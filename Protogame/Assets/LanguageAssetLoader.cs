//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Protogame
{
    public class LanguageAssetLoader : IAssetLoader
    {
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(LanguageAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            // The text key is the asset name.
            return new LanguageAsset(name, (string)data.Value);
        }

        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            return new LanguageAsset(name, "Default Text");
        }
        
        public bool CanNew()
        {
            return true;
        }
        
        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return new LanguageAsset(name, "");
        }
    }
}

