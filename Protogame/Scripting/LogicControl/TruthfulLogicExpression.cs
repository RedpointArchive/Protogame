namespace LogicControl
{
    using System;

    public abstract class TruthfulLogicExpression : LogicExpression
    {
        public override sealed bool Truthful(LogicExecutionState state)
        {
            var result = this.Result(state);

            if (result is bool)
            {
                return (bool)result;
            }

            if (result is float)
            {
                return Math.Abs((float)result) > 0.000001f;
            }

            if (result is string)
            {
                return !string.IsNullOrEmpty((string)result);
            }

            return false;
        }
    }
}