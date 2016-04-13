namespace Protogame
{
    using System;

    /// <summary>
    /// The asset editor.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public abstract class AssetEditor<T> : IAssetEditor
    {
        /// <summary>
        /// The m_ asset.
        /// </summary>
        protected T m_Asset;

        /// <summary>
        /// The bake.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        public virtual void Bake(IAssetManager assetManager)
        {
        }

        /// <summary>
        /// The build layout.
        /// </summary>
        /// <param name="editorContainer">
        /// The editor container.
        /// </param>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        public abstract void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager);

        /// <summary>
        /// The finish layout.
        /// </summary>
        /// <param name="editorContainer">
        /// The editor container.
        /// </param>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        public virtual void FinishLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
        }

        /// <summary>
        /// The get asset type.
        /// </summary>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        public Type GetAssetType()
        {
            return typeof(T);
        }

        /// <summary>
        /// The set asset.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        public void SetAsset(IAsset asset)
        {
            this.m_Asset = (T)asset;
        }
    }
}