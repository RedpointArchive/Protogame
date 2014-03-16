namespace LogicControl
{
    using System;
    using Microsoft.Xna.Framework;

    public class MultiplyLogicExpression : TruthfulLogicExpression
    {
        public LogicExpression LeftHandExpression { get; set; }

        public string Op { get; set; }

        public LogicExpression RightHandExpression { get; set; }

        public MultiplyLogicExpression(LogicExpression leftHandExpression, string op, LogicExpression rightHandExpression)
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
                    case "*":
                        return (float)leftObj * (float)rightObj;
                    case "/":
                        return (float)leftObj / (float)rightObj;
                    default:
                        throw new InvalidOperationException();
                }
            }

            if (this.Op == "*")
            {
                if ((leftObj is Vector2 || leftObj is Vector3 || leftObj is Vector4) && rightObj is Matrix)
                {
                    return LogicBuiltins.Transform(leftObj, rightObj);
                }

                if (leftObj is Matrix && (rightObj is Vector2 || rightObj is Vector3 || rightObj is Vector4))
                {
                    return LogicBuiltins.Transform(rightObj, leftObj);
                }
            }

            switch (this.Op)
            {
                case "*":
                    return LogicBuiltins.Multiply(leftObj, rightObj);
                case "/":
                    return LogicBuiltins.Divide(leftObj, rightObj);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}