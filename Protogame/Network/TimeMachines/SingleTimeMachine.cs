namespace Protogame
{
    using System;

    public class SingleTimeMachine : TimeMachine<Single>
    {
        public SingleTimeMachine(int history) : base(history)
        {
        }

        protected override float SubtractType(float a, float b)
        {
            return a - b;
        }

        protected override float DivideType(int a, float b)
        {
            return a / b;
        }

        protected override float MultiplyType(float a, float b)
        {
            return a * b;
        }

        protected override float AddType(float a, float b)
        {
            return a + b;
        }

        protected override float DefaultType()
        {
            return 0;
        }

        protected override bool ValueIsZeroType(float value)
        {
            return value == 0;
        }
    }
}
