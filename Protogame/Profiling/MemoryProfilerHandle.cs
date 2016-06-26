using System.Diagnostics;

namespace Protogame
{
    using System;
    
    internal class MemoryProfilerHandle : IDisposable
    {
        private readonly string _name;
        private readonly string[] _parameters;
        private readonly MemoryProfiler _profiler;
        private Stopwatch _stopwatch;

        internal MemoryProfilerHandle(MemoryProfiler profiler, string name, string[] parameters)
        {
            _profiler = profiler;
            _name = name;
            _parameters = parameters;
            _stopwatch = Stopwatch.StartNew();
        }
        
        public void Dispose()
        {
            _stopwatch.Stop();
            
            _profiler.EndEvent(_name, _parameters, _stopwatch);
        }
    }
}