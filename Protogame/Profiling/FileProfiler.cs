namespace Protogame
{
    using System;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The default profiler.
    /// </summary>
    internal class FileProfiler : IProfiler
    {
        /// <summary>
        /// The m_ start.
        /// </summary>
        private readonly DateTime m_Start;

        /// <summary>
        /// The m_ writer.
        /// </summary>
        private readonly StreamWriter m_Writer;

        /// <summary>
        /// The m_ indent.
        /// </summary>
        private string m_Indent = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProfiler"/> class.
        /// </summary>
        public FileProfiler()
        {
            this.m_Writer = new StreamWriter("game.prf");
            this.m_Start = DateTime.Now;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FileProfiler"/> class. 
        /// </summary>
        ~FileProfiler()
        {
            try
            {
                this.m_Writer.Flush();
                this.m_Writer.Close();
            }
            catch (ObjectDisposedException)
            {
            }
        }
        
        /// <summary>
        /// The measure.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="IDisposable"/>.
        /// </returns>
        public IDisposable Measure(string name, params string[] parameters)
        {
            this.BeginEvent(name, parameters);
            return new DefaultProfilerHandle(this, name, parameters);
        }

        /// <summary>
        /// The begin event.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        internal void BeginEvent(string name, params string[] parameters)
        {
            if (parameters.Length > 0)
            {
                this.m_Writer.WriteLine(
                    "[" + (DateTime.Now - this.m_Start) + "]                    | " + this.m_Indent + name + ": "
                    + parameters.Aggregate((a, b) => a + ", " + b));
            }
            else
            {
                this.m_Writer.WriteLine(
                    "[" + (DateTime.Now - this.m_Start) + "]                    | " + this.m_Indent + name);
            }

            this.m_Indent += "  ";
        }

        /// <summary>
        /// The end event.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        internal void EndEvent(DefaultProfilerHandle handle, string name, params string[] parameters)
        {
            this.m_Indent = this.m_Indent.Substring(0, Math.Max(this.m_Indent.Length - 2, 0));
            var m = (DateTime.Now - handle.Start).ToString();
            if (m.Length == 8)
            {
                m += ".0000000";
            }

            this.m_Writer.WriteLine("(" + (DateTime.Now - this.m_Start) + ") (" + m + ") | " + this.m_Indent + name);
        }
    }
}