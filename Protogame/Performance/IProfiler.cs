using System;

namespace Protogame
{
    public interface IProfiler
    {
        IDisposable Measure(string name, params string[] parameters);
    }
}

