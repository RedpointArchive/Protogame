namespace Protogame
{
    /// <summary>
    /// The AssetLoader interface.
    /// </summary>
    public interface IAssetLoader
    {
        /// <summary>
        /// The can handle.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool CanHandle(dynamic data);

        /// <summary>
        /// The can new.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool CanNew();

        /// <summary>
        /// The get default.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        IAsset GetDefault(IAssetManager assetManager, string name);

        /// <summary>
        /// The get new.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        IAsset GetNew(IAssetManager assetManager, string name);

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        IAsset Handle(IAssetManager assetManager, string name, dynamic data);
    }
}