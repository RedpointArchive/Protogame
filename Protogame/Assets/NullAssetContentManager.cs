namespace Protogame
{
    using System.IO;

    /// <summary>
    /// The null asset content manager.
    /// </summary>
    public class NullAssetContentManager : IAssetContentManager
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
        /// <exception cref="NoAssetContentManagerException">
        /// </exception>
        public T Load<T>(string assetName)
        {
            throw new NoAssetContentManagerException();
        }

        /// <summary>
        /// The purge.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        /// <exception cref="NoAssetContentManagerException">
        /// </exception>
        public void Purge(string assetName)
        {
            throw new NoAssetContentManagerException();
        }

        /// <summary>
        /// The set stream.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <exception cref="NoAssetContentManagerException">
        /// </exception>
        public void SetStream(string assetName, Stream stream)
        {
            throw new NoAssetContentManagerException();
        }

        /// <summary>
        /// The unset stream.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        /// <exception cref="NoAssetContentManagerException">
        /// </exception>
        public void UnsetStream(string assetName)
        {
            throw new NoAssetContentManagerException();
        }
    }
}