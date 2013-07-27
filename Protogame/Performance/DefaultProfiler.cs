using System;
using System.Linq;

namespace Protogame
{
    public class DefaultProfiler : IProfiler
    {
        private string m_Indent = "";
    
        public IDisposable Measure(string name, params string[] parameters)
        {
            this.BeginEvent(name, parameters);
            return new DefaultProfilerHandle(this, name, parameters);
        }
        
        internal void BeginEvent(string name, params string[] parameters)
        {
            if (parameters.Length > 0)
                Console.WriteLine(
                    "                   " + this.m_Indent + name + ": " +
                    parameters.Aggregate((a, b) => a + ", " + b));
            else
                Console.WriteLine(
                    "                   " + this.m_Indent + name);
            this.m_Indent += "  ";
        }
        
        internal void EndEvent(DefaultProfilerHandle handle, string name, params string[] parameters)
        {
            this.m_Indent = this.m_Indent.Substring(0, Math.Max(this.m_Indent.Length - 2, 0));
            Console.WriteLine(
                "(" + (DateTime.Now - handle.Start) + ") " + this.m_Indent + name);
        }
    }
}

