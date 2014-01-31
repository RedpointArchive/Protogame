namespace Protogame
{
    using System;

    public class Int16TimeMachine : TimeMachine<Int16>
    {
        public Int16TimeMachine(int history) : base(history)
        {
        }

        protected override Int16 SubtractType(Int16 a, Int16 b)
        {
            return (Int16)(a - b);
        }

        protected override Int16 DivideType(Int16 a, int b)
        {
            return (Int16)(a / b);
        }

        protected override Int16 MultiplyType(Int16 a, int b)
        {
            return (Int16)(a * b);
        }

        protected override Int16 AddType(Int16 a, Int16 b)
        {
            return (Int16)(a + b);
        }

        protected override Int16 DefaultType()
        {
            return 0;
        }

        protected override bool ValueIsZeroType(Int16 value)
        {
            return value == 0;
        }
    }
}
