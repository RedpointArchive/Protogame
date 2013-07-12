//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Protogame
{
    public class FontAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is FontAsset;
        }

        public dynamic Handle(IAsset asset)
        {
            var fontAsset = asset as FontAsset;
            
            return new
            {
                Loader = typeof(FontAssetLoader).FullName,
                FontSize = fontAsset.FontSize,
                FontName = fontAsset.FontName,
                FontData = fontAsset.FontData
            };
        }
    }
}

