namespace LogicControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

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

        public override Expression Compile(ParameterExpression stateParameterExpression, LabelTarget returnTarget)
        {
            return
                Expression.Invoke(
                    (Expression<Func<object, string, object, object>>)((a, op, b) => DoAddition(a, op, b)),
                    Expression.Convert(this.LeftHandExpression.Compile(stateParameterExpression, returnTarget), typeof(object)),
                    Expression.Constant(this.Op),
                    Expression.Convert(this.RightHandExpression.Compile(stateParameterExpression, returnTarget), typeof(object)));
        }

        public override object Result(LogicExecutionState state)
        {
            var leftObj = this.LeftHandExpression.Result(state);
            var rightObj = this.RightHandExpression.Result(state);

            return DoAddition(leftObj, this.Op, rightObj);
        }
        
        public static object DoAddition(object leftObj, string op, object rightObj)
        {
            if (leftObj is float && rightObj is float)
            {
                switch (op)
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
                switch (op)
                {
                    case "+":
                        return (string)leftObj + rightObj;
                    default:
                        throw new InvalidOperationException();
                }
            }

            if (rightObj is string)
            {
                switch (op)
                {
                    case "+":
                        return leftObj + (string)rightObj;
                    default:
                        throw new InvalidOperationException();
                }
            }

            switch (op)
            {
                case "+":
                    return LogicBuiltins.Add(new List<object> { leftObj, rightObj });
                case "-":
                    return LogicBuiltins.Subtract(new List<object> { leftObj, rightObj });
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}