namespace Protogame
{
    /// <summary>
    /// A callback interface that allows the game configuration
    /// to specify the asset manager provider to use at runtime.
    /// </summary>
    public interface IAssetManagerProviderInitializer
    {
        /// <summary>
        /// Initialize the game using the specified asset manager provider.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Initialize<T>() where T : IAssetManagerProvider;
    }
}