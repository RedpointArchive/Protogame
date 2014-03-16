namespace LogicControl
{
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
    }
}