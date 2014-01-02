using System;
using System.Linq;
using System.IO;

namespace Protogame
{
    internal class DefaultProfiler : IProfiler
    {
        private string m_Indent = "";
        private StreamWriter m_Writer;
        private DateTime m_Start;
        
        public DefaultProfiler()
        {
            this.m_Writer = new StreamWriter("game.prf");
            this.m_Start = DateTime.Now;
        }
        
        ~DefaultProfiler()
        {
            this.m_Writer.Flush();
            this.m_Writer.Close();
        }
    
        public IDisposable Measure(string name, params string[] parameters)
        {
            this.BeginEvent(name, parameters);
            return new DefaultProfilerHandle(this, name, parameters);
        }

        public void AttachNetworkDispatcher(MxDispatcher dispatcher)
        {
        }

        public void DetachNetworkDispatcher(MxDispatcher dispatcher)
        {
        }

        internal void BeginEvent(string name, params string[] parameters)
        {
            if (parameters.Length > 0)
                this.m_Writer.WriteLine(
                    "[" + (DateTime.Now - this.m_Start) + "]                    | " + this.m_Indent + name + ": " +
                    parameters.Aggregate((a, b) => a + ", " + b));
            else
                this.m_Writer.WriteLine(
                    "[" + (DateTime.Now - this.m_Start) + "]                    | " + this.m_Indent + name);
            this.m_Indent += "  ";
        }
        
        internal void EndEvent(DefaultProfilerHandle handle, string name, params string[] parameters)
        {
            this.m_Indent = this.m_Indent.Substring(0, Math.Max(this.m_Indent.Length - 2, 0));
            var m = (DateTime.Now - handle.Start).ToString();
            if (m.Length == 8)
                m += ".0000000";
            this.m_Writer.WriteLine(
                "(" + (DateTime.Now - this.m_Start) + ") (" + m + ") | " + this.m_Indent + name);
        }
    }
}

