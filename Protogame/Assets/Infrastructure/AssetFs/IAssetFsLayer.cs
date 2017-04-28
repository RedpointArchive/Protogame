using System.Collections.Generic;
using System.Threading.Tasks;

namespace Protogame
{
    public interface IAssetFsLayer
    {
        Task<IAssetFsFile> Get(string name);

        Task<IAssetFsFile[]> List();

        void GetChangedSinceLastUpdate(ref List<string> names);
    }
}
