namespace Protogame
{
    public interface ITransparentAssetCompiler
    {
        IAsset Handle(IAsset asset, bool force = false);
        IAsset HandlePlatform(IAsset asset, TargetPlatform platform, bool force = false);
    }
}
