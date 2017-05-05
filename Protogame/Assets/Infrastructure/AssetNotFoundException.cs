using System;

namespace Protogame
{
    public class AssetNotFoundException : Exception
    {
        public AssetNotFoundException(string assetName)
            : base("The requested asset '" + assetName + "' could not be found.")
        {
        }
        
        public AssetNotFoundException(string assetName, Exception innerException)
            : base("The requested asset '" + assetName + "' could not be found.", innerException)
        {
        }
    }
}