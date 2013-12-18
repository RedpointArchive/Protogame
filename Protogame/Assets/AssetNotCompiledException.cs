using System;

namespace Protogame
{
    public class AssetNotCompiledException : ApplicationException
    {
        public const string POST_MESSAGE =
            "has not been compiled for this platform, and no runtime compiler could be " +
            "found for this asset type.  If the game is executing on a non-desktop platform, " +
            "you will need to precompile the asset with the asset manager.";

        public AssetNotCompiledException(string assetName)
            : base("The requested asset '" + assetName + "' " + POST_MESSAGE)
        {
        }

        public AssetNotCompiledException(string assetName, Exception innerException)
            : base("The requested asset '" + assetName + "' " + POST_MESSAGE, innerException)
        {
        }
    }
}

