using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Protogame
{
    public interface ICompiledAssetFs : IAssetUpdateNotifier
    {
        Task<IAssetFsFile[]> List();

        Task<List<Task<IAssetFsFile>>> ListTasks();

        Task<IAssetFsFile> Get(string name);
    }
}
