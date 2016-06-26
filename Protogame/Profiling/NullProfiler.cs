namespace Protogame
{
    using System;

    /// <summary>
    /// The null profiler.
    /// </summary>
    public class NullProfiler : IProfiler
    {
        /// <summary>
        /// The attach network dispatcher.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher.
        /// </param>
        public void AttachNetworkDispatcher(MxDispatcher dispatcher)
        {
        }

        /// <summary>
        /// The detach network dispatcher.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher.
        /// </param>
        public void DetachNetworkDispatcher(MxDispatcher dispatcher)
        {
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
            return new NullProfilerHandle();
        }
    }
}