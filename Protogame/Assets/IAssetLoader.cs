namespace Protogame
{
    public interface IAssetLoader
    {
        bool CanHandle(dynamic data);
        IAsset Handle(IAssetManager assetManager, string name, dynamic data);
        IAsset GetDefault(IAssetManager assetManager, string name);
        
        bool CanNew();
        IAsset GetNew(IAssetManager assetManager, string name);
    }
}

