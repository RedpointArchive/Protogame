using System;

namespace Protogame
{
    public interface ITimeMachine
    {
        /// <summary>
        /// The type of the values stored by this time machine.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Retrieves the value at the specified tick, or interpolates / extrapolates a value from
        /// the known values in the time machine.
        /// </summary>
        /// <param name="tick">
        /// The tick at which to retrieve the value.
        /// </param>
        /// <returns>
        /// The interpolated value.
        /// </returns>
        object Get(int tick);

        /// <summary>
        /// Purges old history in the time machine, freeing up memory.
        /// </summary>
        /// <remarks>
        /// This will free up memory in the time machine by removing known values older than
        /// the specified tick minus the amount of history specified when the time machine
        /// was created.
        /// </remarks>
        /// <param name="tick">
        /// The current tick.  This value minus the history setting will
        /// be the tick from which older ticks will be removed.
        /// </param>
        void Purge(int tick);

        /// <summary>
        /// Sets the specified tick and value into the time machine.  Once you have set a value at
        /// a specified time, you can only set values with a higher tick.
        /// </summary>
        /// <param name="tick">
        /// The tick at which this value exists.
        /// </param>
        /// <param name="value">
        /// The value to store in the time machine.
        /// </param>
        void Set(int tick, object value);
    }

    public interface ITimeMachine<T> : ITimeMachine
    {
        /// <summary>
        /// Retrieves the value at the specified tick, or interpolates / extrapolates a value from
        /// the known values in the time machine.
        /// </summary>
        /// <param name="tick">
        /// The tick at which to retrieve the value.
        /// </param>
        /// <returns>
        /// The <typeparamref name="T"/>.
        /// </returns>
        T Get(int tick);

        /// <summary>
        /// Sets the specified tick and value into the time machine.  Once you have set a value at
        /// a specified time, you can only set values with a higher tick.
        /// </summary>
        /// <param name="tick">
        /// The tick at which this value exists.
        /// </param>
        /// <param name="value">
        /// The value to store in the time machine.
        /// </param>
        void Set(int tick, T value);
    }
}
