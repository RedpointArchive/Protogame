using System;

namespace Protogame
{
    internal class DefaultProfilerHandle : IDisposable
    {
        private DefaultProfiler m_Profiler;
        private string m_Name;
        private string[] m_Parameters;
        
        internal DefaultProfilerHandle(DefaultProfiler profiler, string name, string[] parameters)
        {
            this.m_Profiler = profiler;
            this.m_Name = name;
            this.m_Parameters = parameters;
            this.Start = DateTime.Now;
        }
        
        internal DateTime Start { get; private set; }

        public void Dispose()
        {
            this.m_Profiler.EndEvent(this, this.m_Name, this.m_Parameters);
        }
    }
}

