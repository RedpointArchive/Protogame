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

        public bool CanHandle(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(TextureAtlasAssetLoader).FullName;
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

        public IAsset Handle(IAssetManager assetManager, string name, IRawAsset data)
        {
            if (data is CompiledAsset)
            {
                return new TextureAtlasAsset(
                    this.m_AssetContentManager,
                    name,
                    data.GetProperty<PlatformData>("PlatformData"));
            }

            var sourceTextureNames = (data.GetProperty<JArray>("SourceTextures")).Select(x => x.Value<string>()).ToArray();

            return new TextureAtlasAsset(
                assetManager,
                name,
                sourceTextureNames);
        }
    }
}

