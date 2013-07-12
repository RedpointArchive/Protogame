//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;

namespace Protogame
{
    public class AssetNotFoundException : ApplicationException
    {
        public AssetNotFoundException(string assetName, Exception innerException)
            : base("The requested asset '" + assetName + "' could not be found.", innerException)
        {
        }
    }
}

