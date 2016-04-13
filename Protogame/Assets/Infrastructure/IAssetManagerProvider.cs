namespace Protogame
{
    /// <summary>
    /// The AssetManagerProvider interface.
    /// </summary>
    public interface IAssetManagerProvider
    {
        /// <summary>
        /// Gets a value indicating whether is ready.
        /// </summary>
        /// <value>
        /// The is ready.
        /// </value>
        bool IsReady { get; }

        /// <summary>
        /// The get asset manager.
        /// </summary>
        /// <param name="permitCreate">
        /// The permit create.
        /// </param>
        /// <returns>
        /// The <see cref="IAssetManager"/>.
        /// </returns>
        IAssetManager GetAssetManager(bool permitCreate = false);
    }
}