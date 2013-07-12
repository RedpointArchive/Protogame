//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Protogame
{
    public interface IAssetManagerProvider
    {
        bool IsReady { get; }
        IAssetManager GetAssetManager(bool permitCreate = false);
    }
}

