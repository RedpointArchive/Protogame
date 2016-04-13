namespace Protogame
{
    /// <summary>
    /// An interface which allows raw assets to be loaded into the running game or application.
    /// </summary>
    public interface IAssetLoader
    {
        /// <summary>
        /// Indicates whether the current loader can handle the specified raw asset.
        /// </summary>
        /// <param name="data">
        /// The raw asset data to check.
        /// </param>
        /// <returns>
        /// <em>true</em>, if the loader can handle the asset, <em>false</em> otherwise.
        /// </returns>
        bool CanHandle(IRawAsset data);

        /// <summary>
        /// Indicates whether this loader can construct new assets.
        /// </summary>
        /// <returns>
        /// <em>true</em>, if the loader can construct a new asset, <em>false</em> otherwise.
        /// </returns>
        bool CanNew();

        /// <summary>
        /// Returns the default representation of an asset from this loader.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name of the asset.
        /// </param>
        /// <returns>
        /// A default representation for an asset loaded by this loader.
        /// </returns>
        IAsset GetDefault(IAssetManager assetManager, string name);

        /// <summary>
        /// Returns the new representation of an asset created by this loader.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name of the new asset.
        /// </param>
        /// <returns>
        /// A new asset created by this loader.
        /// </returns>
        IAsset GetNew(IAssetManager assetManager, string name);

        /// <summary>
        /// Loads the specified raw asset and returns the runtime representation.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name of the asset that is being loaded.
        /// </param>
        /// <param name="data">
        /// The associated raw asset data to load.
        /// </param>
        /// <returns>
        /// The loaded asset.
        /// </returns>
        IAsset Handle(IAssetManager assetManager, string name, IRawAsset data);
    }
}