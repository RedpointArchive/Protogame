namespace Protogame
{
    /// <summary>
    /// The AssetCompiler interface.
    /// </summary>
    /// <typeparam name="TAsset">
    /// </typeparam>
    public interface IAssetCompiler<TAsset>
        where TAsset : IAsset
    {
        /// <summary>
        /// The compile.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <param name="platform">
        /// The platform.
        /// </param>
        void Compile(TAsset asset, TargetPlatform platform);
    }
}