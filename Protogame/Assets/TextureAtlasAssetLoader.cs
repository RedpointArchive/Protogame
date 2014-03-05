namespace Protogame
{
    using System.Linq;
    using Newtonsoft.Json.Linq;

    public class TextureAtlasAssetLoader : IAssetLoader
    {
        private readonly IAssetContentManager m_AssetContentManager;

        public TextureAtlasAssetLoader(IAssetContentManager assetContentManager)
        {
            this.m_AssetContentManager = assetContentManager;
        }

        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(TextureAtlasAssetLoader).FullName;
        }

        public bool CanNew()
        {
            return false;
        }

        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            return null;
        }

        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return null;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            if (data is CompiledAsset)
            {
                return new TextureAtlasAsset(
                    this.m_AssetContentManager,
                    name,
                    data.PlatformData);
            }

            var sourceTextureNames = ((JArray)data.SourceTextures).Select(x => x.Value<string>()).ToArray();

            return new TextureAtlasAsset(
                assetManager,
                name,
                sourceTextureNames);
        }
    }
}

