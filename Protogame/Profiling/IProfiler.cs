namespace Protogame
{
    using System;

    /// <summary>
    /// The profiling interface, which allows you to capture the amount
    /// of time an operation took in a frame.
    /// </summary>
    public interface IProfiler
    {
        /// <summary>
        /// Starts a measurement operation.  You should call <see cref="IDisposable.Dispose"/>
        /// on the returned value, or use a <c>using</c> block.
        /// </summary>
        /// <param name="name">The name of this operation.</param>
        /// <param name="parameters">The parameters of this operation.</param>
        /// <returns>
        /// The <see cref="IDisposable"/>.
        /// </returns>
        IDisposable Measure(string name, params string[] parameters);
    }
}