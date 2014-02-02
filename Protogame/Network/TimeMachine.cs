namespace Protogame
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class facilitates lag compensation, value prediction, interpolation and extrapolation
    /// for data that is being synchronised over a latent stream (such as a network connection).
    /// </summary>
    /// <typeparam name="T">
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
        private readonly SortedList<int, T> m_KnownValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeMachine{T}"/> class. 
        /// Initializes a new instance of the <see cref="Protogame.Network.TimeMachine&lt;&gt;"/> class.
        /// </summary>
        /// <param name="history">
        /// The amount of history to keep in ticks.
        /// </param>
        public TimeMachine(int history)
        {
            this.m_KnownValues = new SortedList<int, T>();
            this.m_History = history;
        }

        /// <summary>
        /// Retrieves the value at the specified tick, or interpolates / extrapolates a value from
        /// the known values in the time machine.
        /// </summary>
        /// <param name="tick">
        /// The tick at which to retrieve the value.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Get(int tick)
        {
            var previousTick = this.m_KnownValues.Keys.Where(k => k <= tick).DefaultIfEmpty(-1).Max();
            var nextTick = this.m_KnownValues.Keys.Where(k => k >= tick).DefaultIfEmpty(-1).Min();

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
            var keys = this.m_KnownValues.Keys.Where(k => k <= tick - this.m_History).OrderBy(k => k).ToArray();

            foreach (var k in keys)
            {
                if (this.m_KnownValues.Count <= 2)
                {
                    // Never allow less than 2 values in the list as this prevents extrapolation.
                    return;
                }

                this.m_KnownValues.Remove(k);
            }
        }

        /// <summary>
        /// Sets the specified tick and value into the time machine.
        /// </summary>
        /// <param name="tick">
        /// The tick at which this value exists.
        /// </param>
        /// <param name="value">
        /// The value to store in the time machine.
        /// </param>
        public void Set(int tick, T value)
        {
            this.m_KnownValues[tick] = value;
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
        /// Return the default value of <see cref="T"/> when neither interpolation or extrapolation can be performed.
        /// </summary>
        /// <returns>
        /// The default value.
        /// </returns>
        protected abstract T DefaultType();

        /// <summary>
        /// Divides an instance of <see cref="T"/> by a numeric value.  Effectively this is
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
        /// Multiply an instance of <see cref="T"/> by a numeric value.
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
        /// Subtract an instance of <see cref="T"/> from another instance of <see cref="T"/>.
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