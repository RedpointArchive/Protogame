using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Protogame
{
    public interface IAssetFsLayer : IAssetUpdateNotifier
    {
        Task<IAssetFsFile> Get(string name);

        Task<IAssetFsFile[]> List();
    }
}
