using System;

namespace Protogame
{
    public class NullProfiler : IProfiler
    {
        public IDisposable Measure(string name, params string[] parameters)
        {
            return new NullProfilerHandle();
        }
    }
}

