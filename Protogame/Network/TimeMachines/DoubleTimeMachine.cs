namespace Protogame
{
    using System;

    public class DoubleTimeMachine : TimeMachine<Double>
    {
        public DoubleTimeMachine(int history) : base(history)
        {
        }

        protected override double SubtractType(double a, double b)
        {
            return a - b;
        }

        protected override double DivideType(double a, int b)
        {
            return a / b;
        }

        protected override double MultiplyType(double a, int b)
        {
            return a * b;
        }

        protected override double AddType(double a, double b)
        {
            return a + b;
        }

        protected override double DefaultType()
        {
            return 0;
        }

        protected override bool ValueIsZeroType(double value)
        {
            return value == 0;
        }
    }
}
