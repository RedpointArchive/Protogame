using System;
using System.Threading.Tasks;

namespace Protogame
{
    public class TextureAssetLoader : IAssetLoader<TextureAsset>
    {
        private readonly IAssetContentManager _assetContentManager;
        
        public TextureAssetLoader(IAssetContentManager assetContentManager)
        {
            _assetContentManager = assetContentManager;
        }
        
        public async Task<IAsset> Load(string name, SerializedAsset input, IAssetManager assetManager)
        {
            return new TextureAsset(
                _assetContentManager,
                name,
                input.GetByteArray("Data"),
                input.GetInt32("OriginalWidth"),
                input.GetInt32("OriginalHeight"));
        }
    }
}