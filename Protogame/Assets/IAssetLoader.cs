//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Protogame
{
    public interface IAssetLoader
    {
        bool CanHandle(dynamic data);
        IAsset Handle(string name, dynamic data);
        IAsset GetDefault(string name);
        
        bool CanNew();
        IAsset GetNew(string name);
    }
}

