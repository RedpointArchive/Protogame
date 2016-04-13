namespace Protogame
{
    using System.IO;

    /// <summary>
    /// The AssetContentManager interface.
    /// </summary>
    public interface IAssetContentManager
    {
        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T Load<T>(string assetName);

        /// <summary>
        /// The purge.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        void Purge(string assetName);

        /// <summary>
        /// The set stream.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        void SetStream(string assetName, Stream stream);

        /// <summary>
        /// The unset stream.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        void UnsetStream(string assetName);
    }
}