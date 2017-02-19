namespace Protogame
{
    public interface IAssetLoader
    {
        bool CanLoad(IRawAsset data);

        IAsset Load(string name, IRawAsset data);
    }
}
