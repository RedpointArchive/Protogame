namespace Protogame
{
    using System;

    /// <summary>
    /// The AssetEditor interface.
    /// </summary>
    public interface IAssetEditor
    {
        /// <summary>
        /// The bake.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        void Bake(IAssetManager assetManager);

        /// <summary>
        /// The build layout.
        /// </summary>
        /// <param name="editorContainer">
        /// The editor container.
        /// </param>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager);

        /// <summary>
        /// The finish layout.
        /// </summary>
        /// <param name="editorContainer">
        /// The editor container.
        /// </param>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        void FinishLayout(SingleContainer editorContainer, IAssetManager assetManager);

        /// <summary>
        /// The get asset type.
        /// </summary>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        Type GetAssetType();

        /// <summary>
        /// The set asset.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        void SetAsset(IAsset asset);
    }
}