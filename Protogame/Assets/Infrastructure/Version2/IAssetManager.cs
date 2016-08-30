namespace Protogame
{
    public interface IAssetManager
    {
        IAssetReference<T> Get<T>(string name) where T : class, IAsset;

        IAssetReference<T> GetPreferred<T>(string[] namePreferenceList) where T : class, IAsset;
    }
}
