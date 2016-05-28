namespace Protogame
{
    public interface ITextureFromHintPath
    {
        TextureAsset GetTextureFromHintPath(string hintPath);
        TextureAsset GetTextureFromHintPath(IMaterialTexture materialTexture);
    }
}
