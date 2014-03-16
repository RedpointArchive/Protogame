namespace LogicControl
{
    using System;
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

        public override bool Truthful(LogicExecutionState state)
        {
            var leftObj = this.LeftHandExpression.Result(state);
            var rightObj = this.RightHandExpression.Result(state);

            if (leftObj is IComparable && rightObj is IComparable)
            {
                var comparison = ((IComparable)leftObj).CompareTo(rightObj);

                switch (this.Op)
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
                switch (this.Op)
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
                switch (this.Op)
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
                switch (this.Op)
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
                switch (this.Op)
                {
                    case "==":
                        return (Matrix)leftObj == (Matrix)rightObj;
                    case "!=":
                        return (Matrix)leftObj != (Matrix)rightObj;
                }

                throw new InvalidOperationException();
            }

            switch (this.Op)
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