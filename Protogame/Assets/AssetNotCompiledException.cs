namespace Protogame
{
    using System;

    /// <summary>
    /// The asset not compiled exception.
    /// </summary>
    public class AssetNotCompiledException : ApplicationException
    {
        /// <summary>
        /// The pos t_ message.
        /// </summary>
        public const string POST_MESSAGE =
            "has not been compiled for this platform, and no runtime compiler could be "
            + "found for this asset type.  If the game is executing on a non-desktop platform, "
            + "you will need to precompile the asset with the asset manager.";

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetNotCompiledException"/> class.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        public AssetNotCompiledException(string assetName)
            : base("The requested asset '" + assetName + "' " + POST_MESSAGE)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetNotCompiledException"/> class.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public AssetNotCompiledException(string assetName, Exception innerException)
            : base("The requested asset '" + assetName + "' " + POST_MESSAGE, innerException)
        {
        }
    }
}