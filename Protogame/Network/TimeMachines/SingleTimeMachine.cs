namespace Protogame
{
    /// <summary>
    /// The single time machine.
    /// </summary>
    /// <module>Network</module>
    public class SingleTimeMachine : InterpolatedTimeMachine<float>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleTimeMachine"/> class.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public SingleTimeMachine(int history)
            : base(history)
        {
        }

        /// <summary>
        /// The add type.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        protected override float AddType(float a, float b)
        {
            return a + b;
        }

        /// <summary>
        /// The default type.
        /// </summary>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        protected override float DefaultType()
        {
            return 0;
        }

        /// <summary>
        /// The divide type.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        protected override float DivideType(float a, int b)
        {
            return a / b;
        }

        /// <summary>
        /// The multiply type.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        protected override float MultiplyType(float a, int b)
        {
            return a * b;
        }

        /// <summary>
        /// The subtract type.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        protected override float SubtractType(float a, float b)
        {
            return a - b;
        }

        /// <summary>
        /// The value is zero type.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool ValueIsZeroType(float value)
        {
            return value == 0;
        }
    }
}