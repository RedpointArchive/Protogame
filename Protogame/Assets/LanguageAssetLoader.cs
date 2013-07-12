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

        public IAsset Handle(string name, dynamic data)
        {
            // The text key is the asset name.
            return new LanguageAsset(name, data.Value);
        }

        public IAsset GetDefault(string name)
        {
            return new LanguageAsset(name, "Default Text");
        }
    }
}

