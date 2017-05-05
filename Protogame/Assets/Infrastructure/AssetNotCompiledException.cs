using System;

namespace Protogame
{
    public class AssetNotCompiledException : Exception
    {
        public const string PostMessage =
            "has not been compiled for this platform, and no runtime compiler could be "
            + "found for this asset type.  If the game is executing on a non-desktop platform, "
            + "you will need to precompile the asset with the asset manager.";
        
        public AssetNotCompiledException(string assetName)
            : base("The requested asset '" + assetName + "' " + PostMessage)
        {
        }
        
        public AssetNotCompiledException(string assetName, Exception innerException)
            : base("The requested asset '" + assetName + "' " + PostMessage, innerException)
        {
        }
    }
}