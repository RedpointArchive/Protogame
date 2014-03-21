namespace LogicControl
{
    using System;
    using System.Linq.Expressions;

    public class ConstantLogicExpression : TruthfulLogicExpression
    {
        public IComparable Value { get; set; }

        public ConstantLogicExpression(string str)
        {
            this.Value = str;
        }

        public ConstantLogicExpression(float num)
        {
            this.Value = num;
        }

        public ConstantLogicExpression(bool bol)
        {
            this.Value = bol;
        }

        public override object Result(LogicExecutionState state)
        {
            return this.Value;
        }

        public override Expression Compile(ParameterExpression stateParameterExpression, LabelTarget returnTarget)
        {
            return Expression.Constant(this.Value);
        }
    }
}