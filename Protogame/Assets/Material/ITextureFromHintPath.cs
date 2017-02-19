namespace Protogame
{
    public interface ITextureFromHintPath
    {
        IAssetReference<TextureAsset> GetTextureFromHintPath(string hintPath);
        IAssetReference<TextureAsset> GetTextureFromHintPath(IMaterialTexture materialTexture);
    }
}
