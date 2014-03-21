namespace LogicControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
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

        public override Expression Compile(ParameterExpression stateParameterExpression, LabelTarget returnTarget)
        {
            return
                Expression.Invoke(
                    (Expression<Func<object, string, object, object>>)((a, op, b) => DoMultiply(a, op, b)),
                    Expression.Convert(this.LeftHandExpression.Compile(stateParameterExpression, returnTarget), typeof(object)),
                    Expression.Constant(this.Op),
                    Expression.Convert(this.RightHandExpression.Compile(stateParameterExpression, returnTarget), typeof(object)));
        }

        public override object Result(LogicExecutionState state)
        {
            var leftObj = this.LeftHandExpression.Result(state);
            var rightObj = this.RightHandExpression.Result(state);

            return DoMultiply(leftObj, this.Op, rightObj);
        }

        public static object DoMultiply(object leftObj, string op, object rightObj)
        {
            if (leftObj is float && rightObj is float)
            {
                switch (op)
                {
                    case "*":
                        return (float)leftObj * (float)rightObj;
                    case "/":
                        return (float)leftObj / (float)rightObj;
                    default:
                        throw new InvalidOperationException();
                }
            }

            if (op == "*")
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

            switch (op)
            {
                case "*":
                    return LogicBuiltins.Multiply(new List<object> { leftObj, rightObj });
                case "/":
                    return LogicBuiltins.Divide(new List<object> { leftObj, rightObj });
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}