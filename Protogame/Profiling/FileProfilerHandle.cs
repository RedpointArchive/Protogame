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
        private readonly string _name;

        /// <summary>
        /// The m_ parameters.
        /// </summary>
        private readonly string[] _parameters;

        /// <summary>
        /// The m_ profiler.
        /// </summary>
        private readonly FileProfiler _profiler;

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
        internal DefaultProfilerHandle(FileProfiler profiler, string name, string[] parameters)
        {
            this._profiler = profiler;
            this._name = name;
            this._parameters = parameters;
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
            this._profiler.EndEvent(this, this._name, this._parameters);
        }
    }
}