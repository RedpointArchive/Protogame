//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Protogame
{
    public class LanguageAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is LanguageAsset;
        }

        public dynamic Handle(IAsset asset)
        {
            var textAsset = asset as LanguageAsset;

            return new
            {
                Loader = typeof(LanguageAssetLoader).FullName,
                Value = textAsset.Value
            };
        }
    }
}

