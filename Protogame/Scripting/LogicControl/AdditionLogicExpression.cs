namespace LogicControl
{
    using System;

    public class AdditionLogicExpression : TruthfulLogicExpression
    {
        public LogicExpression LeftHandExpression { get; set; }

        public string Op { get; set; }

        public LogicExpression RightHandExpression { get; set; }

        public AdditionLogicExpression(
            LogicExpression leftHandExpression,
            string op,
            LogicExpression rightHandExpression)
        {
            this.LeftHandExpression = leftHandExpression;
            this.Op = op;
            this.RightHandExpression = rightHandExpression;
        }

        public override object Result(LogicExecutionState state)
        {
            var leftObj = this.LeftHandExpression.Result(state);
            var rightObj = this.RightHandExpression.Result(state);

            if (leftObj is float && rightObj is float)
            {
                switch (this.Op)
                {
                    case "+":
                        return (float)leftObj + (float)rightObj;
                    case "-":
                        return (float)leftObj - (float)rightObj;
                    default:
                        throw new InvalidOperationException();
                }
            }

            if (leftObj is string)
            {
                switch (this.Op)
                {
                    case "+":
                        return (string)leftObj + rightObj;
                    default:
                        throw new InvalidOperationException();
                }
            }

            if (rightObj is string)
            {
                switch (this.Op)
                {
                    case "+":
                        return leftObj + (string)rightObj;
                    default:
                        throw new InvalidOperationException();
                }
            }

            switch (this.Op)
            {
                case "+":
                    return LogicBuiltins.Add(leftObj, rightObj);
                case "-":
                    return LogicBuiltins.Subtract(leftObj, rightObj);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}