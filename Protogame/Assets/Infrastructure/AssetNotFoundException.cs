namespace Protogame
{
    using System;

    /// <summary>
    /// The asset not found exception.
    /// </summary>
    public class AssetNotFoundException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetNotFoundException"/> class.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        public AssetNotFoundException(string assetName)
            : base("The requested asset '" + assetName + "' could not be found.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetNotFoundException"/> class.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public AssetNotFoundException(string assetName, Exception innerException)
            : base("The requested asset '" + assetName + "' could not be found.", innerException)
        {
        }
    }
}