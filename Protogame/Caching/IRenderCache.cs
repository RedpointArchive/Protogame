using System;

namespace Protogame
{
    public interface IRenderCache
    {
        T Get<T>(string name) where T : class;
        T Set<T>(string name, T value) where T : class;
        void Free<T>(string name) where T : class;
        bool Has<T>(string name) where T : class;
        
        T GetOrSet<T>(string name, Func<T> setter) where T : class;
    }
}

