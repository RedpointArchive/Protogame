namespace Protogame
{
    /// <summary>
    /// The RawAssetSaver interface.
    /// </summary>
    public interface IRawAssetSaver
    {
        /// <summary>
        /// The save raw asset.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        void SaveRawAsset(string name, object data);
    }
}