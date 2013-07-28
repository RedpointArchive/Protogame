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
        IAsset Handle(IAssetManager assetManager, string name, dynamic data);
        IAsset GetDefault(IAssetManager assetManager, string name);
        
        bool CanNew();
        IAsset GetNew(IAssetManager assetManager, string name);
    }
}

