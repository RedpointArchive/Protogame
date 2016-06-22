using System;

namespace Protogame
{
    public class DefaultSynchronisationApi : ISynchronisationApi
    {
        public void Synchronise<T>(string name, int frameInterval, T currentValue, Action<T> setValue)
        {
            
        }
    }
}
