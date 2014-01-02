using System;

namespace Protogame
{
    public class NullProfiler : IProfiler
    {
        public IDisposable Measure(string name, params string[] parameters)
        {
            return new NullProfilerHandle();
        }

        public void AttachNetworkDispatcher(MxDispatcher dispatcher)
        {
        }

        public void DetachNetworkDispatcher(MxDispatcher dispatcher)
        {
        }
    }
}

