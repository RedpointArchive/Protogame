using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Protogame
{
    public class MemoryProfiler : IMemoryProfiler
    {
        private Dictionary<string, double> _measuredCosts;

        public MemoryProfiler()
        {
            _measuredCosts = new Dictionary<string, double>();
        }

        public IDisposable Measure(string name, params string[] parameters)
        {
            return new MemoryProfilerHandle(this, name, parameters);
        }

        public Dictionary<string, double> GetMeasuredCosts()
        {
            return _measuredCosts;
        }

        public void ResetMeasuredCosts()
        {
            _measuredCosts.Clear();
        }

        internal void EndEvent(string name, string[] parameters, Stopwatch stopwatch)
        {
            if (!_measuredCosts.ContainsKey(name))
            {
                _measuredCosts[name] = 0;
            }

            _measuredCosts[name] += stopwatch.Elapsed.TotalMilliseconds*1000;
        }
    }
}
