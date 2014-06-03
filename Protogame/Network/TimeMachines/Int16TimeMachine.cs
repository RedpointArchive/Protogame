namespace Protogame
{
    using System;

    /// <summary>
    /// The int 16 time machine.
    /// </summary>
    public class Int16TimeMachine : InterpolatedTimeMachine<short>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int16TimeMachine"/> class.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public Int16TimeMachine(int history)
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
        /// The <see cref="short"/>.
        /// </returns>
        protected override short AddType(short a, short b)
        {
            return (Int16)(a + b);
        }

        /// <summary>
        /// The default type.
        /// </summary>
        /// <returns>
        /// The <see cref="short"/>.
        /// </returns>
        protected override short DefaultType()
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
        /// The <see cref="short"/>.
        /// </returns>
        protected override short DivideType(short a, int b)
        {
            return (Int16)(a / b);
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
        /// The <see cref="short"/>.
        /// </returns>
        protected override short MultiplyType(short a, int b)
        {
            return (Int16)(a * b);
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
        /// The <see cref="short"/>.
        /// </returns>
        protected override short SubtractType(short a, short b)
        {
            return (Int16)(a - b);
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
        protected override bool ValueIsZeroType(short value)
        {
            return value == 0;
        }
    }
}