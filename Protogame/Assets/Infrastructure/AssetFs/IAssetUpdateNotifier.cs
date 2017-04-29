using System;
using System.Threading.Tasks;

namespace Protogame
{
    public interface IAssetUpdateNotifier
    {
        void RegisterUpdateNotifier(Func<string, Task> onAssetUpdated);

        void UnregisterUpdateNotifier(Func<string, Task> onAssetUpdated);   
    }
}
