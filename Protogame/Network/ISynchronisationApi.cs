using System;

namespace Protogame
{
    public interface ISynchronisationApi
    {
        void Synchronise<T>(string name, int frameInterval, T currentValue, Action<T> setValue, int? timeMachineHistory);

        bool IsRunningOnClient();
    }
}
