namespace Protogame
{
    public interface ITransparentAssetCompiler
    {
        IAsset Handle(IAsset asset, bool force = false);
    }
}
