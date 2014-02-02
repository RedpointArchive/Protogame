namespace Protogame
{
    /// <summary>
    /// The TransparentAssetCompiler interface.
    /// </summary>
    public interface ITransparentAssetCompiler
    {
        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <param name="force">
        /// The force.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        IAsset Handle(IAsset asset, bool force = false);

        /// <summary>
        /// The handle platform.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <param name="platform">
        /// The platform.
        /// </param>
        /// <param name="force">
        /// The force.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        IAsset HandlePlatform(IAsset asset, TargetPlatform platform, bool force = false);
    }
}