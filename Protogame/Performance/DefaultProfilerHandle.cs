namespace Protogame
{
    using System;

    /// <summary>
    /// The default profiler handle.
    /// </summary>
    internal class DefaultProfilerHandle : IDisposable
    {
        /// <summary>
        /// The m_ name.
        /// </summary>
        private readonly string m_Name;

        /// <summary>
        /// The m_ parameters.
        /// </summary>
        private readonly string[] m_Parameters;

        /// <summary>
        /// The m_ profiler.
        /// </summary>
        private readonly DefaultProfiler m_Profiler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultProfilerHandle"/> class.
        /// </summary>
        /// <param name="profiler">
        /// The profiler.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        internal DefaultProfilerHandle(DefaultProfiler profiler, string name, string[] parameters)
        {
            this.m_Profiler = profiler;
            this.m_Name = name;
            this.m_Parameters = parameters;
            this.Start = DateTime.Now;
        }

        /// <summary>
        /// Gets the start.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        internal DateTime Start { get; private set; }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.m_Profiler.EndEvent(this, this.m_Name, this.m_Parameters);
        }
    }
}