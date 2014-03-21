namespace LogicControl
{
    using System;
    using System.Linq.Expressions;
    using Microsoft.Xna.Framework;

    public class ComparisonLogicExpression : LogicExpression
    {
        public ComparisonLogicExpression(
            LogicExpression leftHandExpression, 
            string op, 
            LogicExpression rightHandExpression)
        {
            this.LeftHandExpression = leftHandExpression;
            this.Op = op;
            this.RightHandExpression = rightHandExpression;
        }

        public LogicExpression LeftHandExpression { get; set; }

        public string Op { get; set; }

        public LogicExpression RightHandExpression { get; set; }

        public override object Result(LogicExecutionState state)
        {
            return this.Truthful(state);
        }

        public override Expression Compile(ParameterExpression stateParameterExpression, LabelTarget returnTarget)
        {
            return
                Expression.Invoke(
                    (Expression<Func<object, string, object, bool>>)((a, op, b) => DoComparison(a, op, b)),
                    Expression.Convert(this.LeftHandExpression.Compile(stateParameterExpression, returnTarget), typeof(object)),
                    Expression.Constant(this.Op),
                    Expression.Convert(this.RightHandExpression.Compile(stateParameterExpression, returnTarget), typeof(object)));
        }

        public override bool Truthful(LogicExecutionState state)
        {
            var leftObj = this.LeftHandExpression.Result(state);
            var rightObj = this.RightHandExpression.Result(state);

            return DoComparison(leftObj, this.Op, rightObj);
        }

        public static bool DoComparison(object leftObj, string op, object rightObj)
        {
            if (leftObj is IComparable && rightObj is IComparable)
            {
                var comparison = ((IComparable)leftObj).CompareTo(rightObj);

                switch (op)
                {
                    case ">":
                        return comparison > 0;
                    case "<":
                        return comparison < 0;
                    case "<=":
                        return comparison >= 0;
                    case ">=":
                        return comparison <= 0;
                    case "==":
                        return comparison == 0;
                    case "!=":
                        return comparison != 0;
                }

                throw new InvalidOperationException();
            }

            if (leftObj is Vector2 && rightObj is Vector2)
            {
                switch (op)
                {
                    case "==":
                        return (Vector2)leftObj == (Vector2)rightObj;
                    case "!=":
                        return (Vector2)leftObj != (Vector2)rightObj;
                }

                throw new InvalidOperationException();
            }

            if (leftObj is Vector3 && rightObj is Vector3)
            {
                switch (op)
                {
                    case "==":
                        return (Vector3)leftObj == (Vector3)rightObj;
                    case "!=":
                        return (Vector3)leftObj != (Vector3)rightObj;
                }

                throw new InvalidOperationException();
            }

            if (leftObj is Vector4 && rightObj is Vector4)
            {
                switch (op)
                {
                    case "==":
                        return (Vector4)leftObj == (Vector4)rightObj;
                    case "!=":
                        return (Vector4)leftObj != (Vector4)rightObj;
                }

                throw new InvalidOperationException();
            }

            if (leftObj is Matrix && rightObj is Matrix)
            {
                switch (op)
                {
                    case "==":
                        return (Matrix)leftObj == (Matrix)rightObj;
                    case "!=":
                        return (Matrix)leftObj != (Matrix)rightObj;
                }

                throw new InvalidOperationException();
            }

            switch (op)
            {
                case "==":
                    return object.Equals(leftObj, rightObj);
                case "!=":
                    return !object.Equals(leftObj, rightObj);
            }

            throw new InvalidOperationException();
        }
    }
}