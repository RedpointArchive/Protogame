//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Protogame
{
    public interface IAsset
    {
        string Name { get; }

        T Resolve<T>() where T : class, IAsset;
    }
}
