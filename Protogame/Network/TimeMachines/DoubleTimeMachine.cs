namespace Protogame
{
    /// <summary>
    /// The double time machine.
    /// </summary>
    /// <module>Network</module>
    public class DoubleTimeMachine : InterpolatedTimeMachine<double>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleTimeMachine"/> class.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public DoubleTimeMachine(int history)
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
        /// The <see cref="double"/>.
        /// </returns>
        protected override double AddType(double a, double b)
        {
            return a + b;
        }

        /// <summary>
        /// The default type.
        /// </summary>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        protected override double DefaultType()
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
        /// The <see cref="double"/>.
        /// </returns>
        protected override double DivideType(double a, int b)
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
        /// The <see cref="double"/>.
        /// </returns>
        protected override double MultiplyType(double a, int b)
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
        /// The <see cref="double"/>.
        /// </returns>
        protected override double SubtractType(double a, double b)
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
        protected override bool ValueIsZeroType(double value)
        {
            return value == 0;
        }
    }
}