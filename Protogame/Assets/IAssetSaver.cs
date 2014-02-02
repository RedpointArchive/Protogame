namespace Protogame
{
    /// <summary>
    /// An interface for saving assets.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface are used to convert runtime assets into
    /// dynamic objects suitable for conversion into JSON, or instances of
    /// <see cref="CompiledAsset"/> for efficient storage of compiled assets that
    /// contain binary data.
    /// </remarks>
    public interface IAssetSaver
    {
        /// <summary>
        /// Returns whether the implementation can save the specified asset.
        /// </summary>
        /// <param name="asset">
        /// The asset to check.
        /// </param>
        /// <returns>
        /// Whether the implementation can save the specified asset.
        /// </returns>
        bool CanHandle(IAsset asset);

        /// <summary>
        /// Handles the specified asset and returns either a dynamic object suitable
        /// for conversion into JSON or an instance of <see cref="CompiledAsset"/>.
        /// </summary>
        /// <remarks>
        /// The target format is used to indicate the most suitable format for
        /// the asset.  Most importantly, the result of the <see cref="Handle"/>
        /// call must be such that:
        /// <ul>
        /// <li>
        /// If the <see cref="target"/> is <see cref="AssetTarget.Runtime"/>, 
        /// then the resulting asset should contain both source and compiled information.
        /// </li>
        /// <li>
        /// If the <see cref="target"/> is <see cref="AssetTarget.SourceFile"/>, then 
        /// the resulting asset should contain only source information.
        /// </li>
        /// <li>
        /// If the <see cref="target"/> is <see cref="AssetTarget.CompiledFile"/>, then 
        /// the resulting asset should contain only compiled information.
        /// </li>
        /// </ul>
        /// </remarks>
        /// <param name="asset">
        /// The asset to save.
        /// </param>
        /// <param name="target">
        /// The target format for the asset.
        /// </param>
        /// <returns>
        /// The dynamic object or instance of <see cref="CompiledAsset"/> for storage.
        /// </returns>
        dynamic Handle(IAsset asset, AssetTarget target);
    }
}