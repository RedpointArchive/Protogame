namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class facilitates lag compensation, value prediction, interpolation and extrapolation
    /// for data that is being synchronised over a latent stream (such as a network connection).
    /// </summary>
    /// <typeparam name="T">
    /// The type of data that will be tracked by the time machine.
    /// </typeparam>
    public abstract class TimeMachine<T>
        where T : struct
    {
        /// <summary>
        /// The amount of history to keep in ticks.
        /// </summary>
        private readonly int m_History;

        /// <summary>
        /// Storage of the known values provided by the <see cref="Set"/> method.
        /// </summary>
        private readonly Dictionary<int, T> m_KnownValues;

        /// <summary>
        /// Storage of the known keys provided by the <see cref="Set"/> method.
        /// </summary>
        private readonly List<int> m_KnownKeys; 

        /// <summary>
        /// The latest tick that was last set with <see cref="Set"/>.
        /// </summary>
        private int m_LatestTick;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeMachine{T}"/> class. 
        /// </summary>
        /// <param name="history">
        /// The amount of history to keep in ticks.
        /// </param>
        protected TimeMachine(int history)
        {
            this.m_KnownValues = new Dictionary<int, T>();
            this.m_KnownKeys = new List<int>();
            this.m_LatestTick = 0;
            this.m_History = history;
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
        /// Retrieves the value at the specified tick, or interpolates / extrapolates a value from
        /// the known values in the time machine.
        /// </summary>
        /// <param name="tick">
        /// The tick at which to retrieve the value.
        /// </param>
        /// <returns>
        /// The <typeparamref name="T"/>.
        /// </returns>
        public T Get(int tick)
        {
            int previousTick, nextTick;

            this.FindSurroundingTickValues(
                this.m_KnownKeys,
                tick,
                out previousTick,
                out nextTick);

            if (previousTick != -1)
            {
                previousTick = this.m_KnownKeys[previousTick];
            }

            if (nextTick != -1)
            {
                nextTick = this.m_KnownKeys[nextTick];
            }

            if (previousTick != -1 && nextTick != -1)
            {
                // If they are the same, skip and return the value.
                if (previousTick == nextTick)
                {
                    return this.m_KnownValues[previousTick];
                }

                // We can interpolate the values.
                var previousValue = this.m_KnownValues[previousTick];
                var nextValue = this.m_KnownValues[nextTick];

                var tickDifference = nextTick - previousTick;
                var valueDifference = this.SubtractType(nextValue, previousValue);

                // If there's no difference between the two values, return either of them.
                if (this.ValueIsZeroType(valueDifference))
                {
                    return previousValue;
                }

                var rate = this.DivideType(valueDifference, tickDifference);

                var additionDifference = tick - previousTick;
                var additionValue = this.MultiplyType(rate, additionDifference);

                return this.AddType(previousValue, additionValue);
            }

            if (previousTick != -1 && nextTick == -1)
            {
                // Return the previous value and don't attempt to predict the future.
                return this.m_KnownValues[previousTick];
            }

            if (nextTick == -1 && previousTick != -1)
            {
                // TODO: Extrapolation
                return this.m_KnownValues[previousTick];
            }

            // TODO: something better
            return this.DefaultType();
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
            if (this.m_KnownKeys.Count <= 2)
            {
                // Never allow less than 2 values in the list as this prevents extrapolation.
                return;
            }

            var keys = this.m_KnownKeys.Where(k => k <= tick - this.m_History).OrderBy(k => k).ToArray();

            foreach (var k in keys)
            {
                this.m_KnownKeys.Remove(k);
                this.m_KnownValues.Remove(k);
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
        public void Set(int tick, T value)
        {
            if (tick < this.m_LatestTick)
            {
                throw new InvalidOperationException("You can only set values later than the last set value.");
            }

            this.m_KnownValues[tick] = value;

            if (tick > this.m_LatestTick)
            {
                this.m_KnownKeys.Add(tick);
                this.m_LatestTick = tick;
            }
        }

        /// <summary>
        /// Add an instance of <see cref="T"/> to another instance of <see cref="T"/>.
        /// </summary>
        /// <returns>
        /// The resulting value.
        /// </returns>
        /// <param name="a">
        /// The first value for .
        /// </param>
        /// <param name="b">
        /// The second value for multiplication.
        /// </param>
        protected abstract T AddType(T a, T b);

        /// <summary>
        /// Return the default value of <typeparamref cref="T"/> when neither interpolation or extrapolation can be performed.
        /// </summary>
        /// <returns>
        /// The default value.
        /// </returns>
        protected abstract T DefaultType();

        /// <summary>
        /// Divides an instance of <typeparamref cref="T"/> by a numeric value.  Effectively this is
        /// used to calculate the rate at which something is being changed.
        /// </summary>
        /// <returns>
        /// The resulting value.
        /// </returns>
        /// <param name="b">
        /// The value to divide by.
        /// </param>
        /// <param name="a">
        /// The value to divide.
        /// </param>
        protected abstract T DivideType(T b, int a);

        /// <summary>
        /// Multiply an instance of <typeparamref cref="T"/> by a numeric value.
        /// </summary>
        /// <returns>
        /// The resulting value.
        /// </returns>
        /// <param name="a">
        /// The first value for multiplication.
        /// </param>
        /// <param name="b">
        /// The second value for multiplication.
        /// </param>
        protected abstract T MultiplyType(T a, int b);

        /// <summary>
        /// Subtract an instance of <typeparamref cref="T"/> from another instance of <see cref="T"/>.
        /// </summary>
        /// <returns>
        /// The resulting value.
        /// </returns>
        /// <param name="a">
        /// The value to subtract from.
        /// </param>
        /// <param name="b">
        /// The value to subtract by.
        /// </param>
        protected abstract T SubtractType(T a, T b);

        /// <summary>
        /// Return whether the specified value represents zero, in which case it would not be safe to call
        /// <see cref="DivideType"/>.
        /// </summary>
        /// <param name="value">
        /// The value to check.
        /// </param>
        /// <returns>
        /// Whether the value represents zero.
        /// </returns>
        protected abstract bool ValueIsZeroType(T value);
    }
}