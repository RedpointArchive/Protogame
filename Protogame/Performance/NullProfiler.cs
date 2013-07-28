using System;

namespace Protogame
{
    internal class NullProfiler : IProfiler
    {
        public IDisposable Measure(string name, params string[] parameters)
        {
            return new NullProfilerHandle();
        }
    }
}

