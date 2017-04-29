using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Protogame
{
    public interface IAssetFsLayer
    {
        Task<IAssetFsFile> Get(string name);

        Task<IAssetFsFile[]> List();

        void RegisterUpdateNotifier(Action<string> onAssetUpdated);

        void UnregisterUpdateNotifier(Action<string> onAssetUpdated);
    }
}
