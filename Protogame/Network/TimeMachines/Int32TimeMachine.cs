namespace Protogame
{
    using System;

    public class Int32TimeMachine : TimeMachine<Int32>
    {
        public Int32TimeMachine(int history) : base(history)
        {
        }

        protected override int SubtractType(int a, int b)
        {
            return a - b;
        }

        protected override int DivideType(int a, int b)
        {
            return a / b;
        }

        protected override int MultiplyType(int a, int b)
        {
            return a * b;
        }

        protected override int AddType(int a, int b)
        {
            return a + b;
        }

        protected override int DefaultType()
        {
            return 0;
        }

        protected override bool ValueIsZeroType(int value)
        {
            return value == 0;
        }
    }
}
