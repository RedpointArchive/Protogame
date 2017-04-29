using System;
using System.Threading.Tasks;

namespace Protogame
{
    public interface IAssetFs
    {
        Task<IAssetFsFile[]> List();

        Task<IAssetFsFile> Get(string name);

        void RegisterUpdateNotifier(Action<string> onAssetUpdated);

        void UnregisterUpdateNotifier(Action<string> onAssetUpdated);
    }
}
