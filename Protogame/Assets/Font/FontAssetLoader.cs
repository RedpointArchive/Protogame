using System.Threading.Tasks;

namespace Protogame
{
    public class FontAssetLoader : IAssetLoader<FontAsset>
    {
        private readonly IAssetContentManager _assetContentManager;
        
        public FontAssetLoader(IAssetContentManager assetContentManager)
        {
            _assetContentManager = assetContentManager;
        }

        public async Task<IAsset> Load(string name, SerializedAsset input, IAssetManager assetManager)
        {
            return new FontAsset(
                _assetContentManager,
                name,
                input.GetString("FontName"),
                input.GetFloat("FontSize"),
                input.GetBoolean("UseKerning"),
                input.GetFloat("Spacing"),
                input.GetByteArray("Data"));
        }
    }
}