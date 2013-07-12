//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Protogame
{
    public class TextureAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is FontAsset;
        }

        public dynamic Handle(IAsset asset)
        {
            var textureAsset = asset as TextureAsset;
            
            return new
            {
                Loader = typeof(TextureAssetLoader).FullName,
                TextureData = textureAsset.Data
            };
        }
    }
}

