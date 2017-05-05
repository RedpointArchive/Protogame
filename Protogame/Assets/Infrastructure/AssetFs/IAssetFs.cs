using System;
using System.Threading.Tasks;

namespace Protogame
{
    public interface IAssetFs : IAssetUpdateNotifier
    {
        Task<IAssetFsFile[]> List();

        Task<IAssetFsFile> Get(string name);
    }
}
