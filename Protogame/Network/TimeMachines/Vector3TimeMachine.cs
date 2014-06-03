namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A time machine for the Vector3 structure.
    /// </summary>
    public class Vector3TimeMachine : InterpolatedTimeMachine<Vector3>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3TimeMachine"/> class.
        /// </summary>
        /// <param name="history">
        /// The amount of history to store in this time machine.
        /// </param>
        public Vector3TimeMachine(int history)
            : base(history)
        {
        }

        protected override Vector3 AddType(Vector3 a, Vector3 b)
        {
            return a + b;
        }

        protected override Vector3 DefaultType()
        {
            return Vector3.Zero;
        }

        protected override Vector3 DivideType(Vector3 b, int a)
        {
            return b / a;
        }

        protected override Vector3 MultiplyType(Vector3 a, int b)
        {
            return a * b;
        }

        protected override Vector3 SubtractType(Vector3 a, Vector3 b)
        {
            return a - b;
        }

        protected override bool ValueIsZeroType(Vector3 value)
        {
            return value == Vector3.Zero;
        }
    }
}