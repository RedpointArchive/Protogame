namespace LogicControl
{
    using System.Linq.Expressions;

    public class IfLogicStatement : LogicStatement
    {
        public IfLogicStatement(LogicExpression condition, LogicStatement statement)
        {
            this.Condition = condition;
            this.Statement = statement;
        }

        public LogicExpression Condition { get; set; }

        public LogicStatement Statement { get; set; }

        public override void Execute(LogicExecutionState state)
        {
            if (this.Condition.Truthful(state))
            {
                this.Statement.Execute(state);
            }
        }

        public override Expression Compile(ParameterExpression stateParameterExpression, LabelTarget returnTarget)
        {
            return Expression.IfThen(
                this.Condition.Compile(stateParameterExpression, returnTarget),
                this.Statement.Compile(stateParameterExpression, returnTarget));
        }
    }
}