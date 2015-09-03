namespace Protogame
{
    /// <summary>
    /// The int 32 time machine.
    /// </summary>
    /// <module>Network</module>
    public class Int32TimeMachine : InterpolatedTimeMachine<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int32TimeMachine"/> class.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public Int32TimeMachine(int history)
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
        /// The <see cref="int"/>.
        /// </returns>
        protected override int AddType(int a, int b)
        {
            return a + b;
        }

        /// <summary>
        /// The default type.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected override int DefaultType()
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
        /// The <see cref="int"/>.
        /// </returns>
        protected override int DivideType(int a, int b)
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
        /// The <see cref="int"/>.
        /// </returns>
        protected override int MultiplyType(int a, int b)
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
        /// The <see cref="int"/>.
        /// </returns>
        protected override int SubtractType(int a, int b)
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
        protected override bool ValueIsZeroType(int value)
        {
            return value == 0;
        }
    }
}