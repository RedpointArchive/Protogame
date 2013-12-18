namespace Protogame
{
    public interface IAssetSaver
    {
        bool CanHandle(IAsset asset);
        dynamic Handle(IAsset asset, AssetTarget target);
    }
}

