namespace Protogame
{
    public interface IAssetManagerProvider
    {
        bool IsReady { get; }
        IAssetManager GetAssetManager(bool permitCreate = false);
    }
}

