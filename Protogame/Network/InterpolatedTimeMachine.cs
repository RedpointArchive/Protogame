namespace Protogame
{
    /// <summary>
    /// A form of time machine that supports interpolation and extrapolation of values
    /// between keys.
    /// </summary>
    /// <typeparam name="T">
    /// The type of data that will be tracked by the time machine.
    /// </typeparam>
    /// <module>Network</module>
    public abstract class InterpolatedTimeMachine<T> : TimeMachine<T>
        where T : struct
    {
        protected InterpolatedTimeMachine(int history)
            : base(history)
        {
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
        public override sealed T Get(int tick)
        {
            int previousTick, nextTick;

            this.FindSurroundingTickValues(
                this.KnownKeys,
                tick,
                out previousTick,
                out nextTick);

            if (previousTick != -1)
            {
                previousTick = this.KnownKeys[previousTick];
            }

            if (nextTick != -1)
            {
                nextTick = this.KnownKeys[nextTick];
            }

            if (previousTick != -1 && nextTick != -1)
            {
                // If they are the same, skip and return the value.
                if (previousTick == nextTick)
                {
                    return this.KnownValues[previousTick];
                }

                // We can interpolate the values.
                var previousValue = this.KnownValues[previousTick];
                var nextValue = this.KnownValues[nextTick];

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
                return this.KnownValues[previousTick];
            }

            if (nextTick == -1 && previousTick != -1)
            {
                // TODO: Extrapolation
                return this.KnownValues[previousTick];
            }

            // TODO: something better
            return this.DefaultType();
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