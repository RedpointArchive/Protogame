namespace Protogame
{
    using System;

    /// <summary>
    /// The Profiler interface.
    /// </summary>
    public interface IProfiler
    {
        /// <summary>
        /// The attach network dispatcher.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher.
        /// </param>
        void AttachNetworkDispatcher(MxDispatcher dispatcher);

        /// <summary>
        /// The detach network dispatcher.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher.
        /// </param>
        void DetachNetworkDispatcher(MxDispatcher dispatcher);

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
        IDisposable Measure(string name, params string[] parameters);
    }
}