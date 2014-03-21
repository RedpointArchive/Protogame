namespace LogicControl
{
    using System;
    using System.Collections.Generic;

    public class UnaryLogicExpression : TruthfulLogicExpression
    {
        public string Op { get; set; }

        public LogicExpression Expression { get; set; }

        public UnaryLogicExpression(string op, LogicExpression expression)
        {
            this.Op = op;
            this.Expression = expression;
        }

        public override object Result(LogicExecutionState state)
        {
            var obj = this.Expression.Result(state);

            if (obj is float)
            {
                switch (this.Op)
                {
                    case "-":
                        return -(float)obj;
                    default:
                        throw new InvalidOperationException();
                }
            }

            switch (this.Op)
            {
                case "-":
                    return LogicBuiltins.Negate(new List<object> { obj });
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}