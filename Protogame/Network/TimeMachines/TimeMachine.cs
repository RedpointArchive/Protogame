namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class facilitates lag compensation and value prediction for data that is 
    /// being synchronized over a latent stream (such as a network connection).  For interpolation
    /// and extrapolation, use <see cref="InterpolatedTimeMachine{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of data that will be tracked by the time machine.
    /// </typeparam>
    /// <module>Network</module>
    public abstract class TimeMachine<T> : ITimeMachine<T>
    {
        /// <summary>
        /// Storage of the known values provided by the <see cref="Set(int, T)"/> method.
        /// </summary>
        protected readonly Dictionary<int, T> KnownValues;

        /// <summary>
        /// Storage of the known keys provided by the <see cref="Set(int, T)"/> method.
        /// </summary>
        protected readonly List<int> KnownKeys;

        /// <summary>
        /// The amount of history to keep in ticks.
        /// </summary>
        private readonly int _history;

        /// <summary>
        /// The latest tick that was last set with <see cref="Set(int, T)"/>.
        /// </summary>
        private int _latestTick;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeMachine{T}"/> class. 
        /// </summary>
        /// <param name="history">
        /// The amount of history to keep in ticks.
        /// </param>
        protected TimeMachine(int history)
        {
            KnownValues = new Dictionary<int, T>();
            KnownKeys = new List<int>();
            _latestTick = 0;
            _history = history;
        }

        /// <summary>
        /// Finds the nearest previous and next ticks to the specified tick using a binary
        /// search algorithm.
        /// </summary>
        /// <param name="keys">The list of integer keys to search through.</param>
        /// <param name="current">The current, or target value to find.</param>
        /// <param name="previous">The previous value to this value.</param>
        /// <param name="next">The next value to this value.</param>
        public void FindSurroundingTickValues(IList<int> keys, int current, out int previous, out int next)
        {
            int lowest = 0;
            int mid;
            int highest = keys.Count - 1;

            if (keys.Count == 0)
            {
                previous = -1;
                next = -1;
                return;
            }

            while (true)
            {
                if (lowest >= 0 && keys[lowest] == current)
                {
                    previous = lowest;
                    next = lowest;
                    return;
                }

                if (highest < keys.Count && keys[highest] == current)
                {
                    previous = highest;
                    next = highest;
                    return;
                }

                if (lowest == keys.Count - 1 && keys[lowest] <= current)
                {
                    previous = lowest;
                    next = -1;
                    return;
                }

                if (highest == 0 && keys[highest] > current)
                {
                    previous = -1;
                    next = highest;
                    return;
                }

                if (lowest + 1 < keys.Count && keys[lowest + 1] == current)
                {
                    previous = lowest + 1;
                    next = lowest + 1;
                    return;
                }

                if (highest - 1 >= 0 && keys[highest - 1] == current)
                {
                    previous = highest - 1;
                    next = highest - 1;
                    return;
                }

                if (lowest < keys.Count - 1 && keys[lowest] <= current && current <= keys[lowest + 1])
                {
                    previous = lowest;
                    next = lowest + 1;
                    return;
                }
                
                if (highest >= 1 && keys[highest - 1] >= current && current >= keys[highest])
                {
                    previous = highest - 1;
                    next = highest;
                    return;
                }

                if (lowest == highest && keys[lowest] != current)
                {
                    throw new InvalidOperationException();
                }

                mid = (int)Math.Ceiling(((highest - (double)lowest) / 2f) + lowest);

                if (mid > 0 && keys[mid] <= current)
                {
                    lowest = mid;
                }
                else if (mid < keys.Count && keys[mid] >= current)
                {
                    highest = Math.Max(lowest, mid - 1);
                }
            }
        }

        /// <summary>
        /// The type of the values stored by this time machine.
        /// </summary>
        public Type Type => typeof(T);

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
        public virtual T Get(int tick)
        {
            int previousTick, nextTick;

            FindSurroundingTickValues(
                KnownKeys,
                tick,
                out previousTick,
                out nextTick);

            if (previousTick != -1)
            {
                previousTick = KnownKeys[previousTick];
            }

            if (nextTick != -1)
            {
                nextTick = KnownKeys[nextTick];
            }

            if (nextTick == -1 && previousTick == -1)
            {
                return default(T);
            }

            if (nextTick == -1)
            {
                return KnownValues[previousTick];
            }

            if (previousTick == -1)
            {
                return KnownValues[nextTick];
            }

            // Return the nearest value.
            if (Math.Abs(tick - previousTick) < Math.Abs(nextTick - tick))
            {
                return KnownValues[previousTick];
            }
            
            return KnownValues[nextTick];
        }

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
        object ITimeMachine.Get(int tick)
        {
            return Get(tick);
        }

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
        public void Purge(int tick)
        {
            if (KnownKeys.Count <= 2)
            {
                // Never allow less than 2 values in the list as this prevents extrapolation.
                return;
            }

            var keys = KnownKeys.Where(k => k <= tick - _history).OrderBy(k => k).ToArray();

            foreach (var k in keys)
            {
                KnownKeys.Remove(k);
                KnownValues.Remove(k);
            }
        }

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
        public void Set(int tick, object value)
        {
            Set(tick, (T)value);
        }

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
        public void Set(int tick, T value)
        {
            if (tick < _latestTick)
            {
                return;
            }

            KnownValues[tick] = value;

            if (tick > _latestTick)
            {
                KnownKeys.Add(tick);
                _latestTick = tick;
            }
        }
    }
}