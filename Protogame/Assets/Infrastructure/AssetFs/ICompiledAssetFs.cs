using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Protogame
{
    public interface ICompiledAssetFs
    {
        Task<IAssetFsFile[]> List();

        Task<List<Task<IAssetFsFile>>> ListTasks();

        Task<IAssetFsFile> Get(string name);

        void RegisterUpdateNotifier(Action<string> onAssetUpdated);

        void UnregisterUpdateNotifier(Action<string> onAssetUpdated);
    }
}
