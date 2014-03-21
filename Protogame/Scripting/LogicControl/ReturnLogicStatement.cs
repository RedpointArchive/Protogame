namespace LogicControl
{
    using System.Linq.Expressions;

    public class ReturnLogicStatement : LogicStatement
    {
        public LogicExpression Expression { get; set; }

        public ReturnLogicStatement(LogicExpression expression)
        {
            this.Expression = expression;
        }

        public override void Execute(LogicExecutionState state)
        {
            state.Return(this.Expression.Result(state));
        }

        public override Expression Compile(ParameterExpression stateParameterExpression, LabelTarget returnTarget)
        {
            return System.Linq.Expressions.Expression.Return(
                returnTarget,
                this.Expression.Compile(stateParameterExpression, returnTarget));
        }
    }
}