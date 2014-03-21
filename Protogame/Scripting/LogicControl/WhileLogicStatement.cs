namespace LogicControl
{
    using System.Linq.Expressions;

    public class WhileLogicStatement : LogicStatement
    {
        public WhileLogicStatement(LogicExpression condition, LogicStatement statement)
        {
            this.Condition = condition;
            this.Statement = statement;
        }

        public LogicExpression Condition { get; set; }

        public LogicStatement Statement { get; set; }

        public override void Execute(LogicExecutionState state)
        {
            while (this.Condition.Truthful(state))
            {
                this.Statement.Execute(state);

                if (state.Finished)
                {
                    return;
                }
            }
        }

        public override Expression Compile(ParameterExpression stateParameterExpression, LabelTarget returnTarget)
        {
            var breakLabel = Expression.Label(Expression.Label());
            return
                Expression.Block(
                    Expression.Loop(
                        Expression.IfThenElse(
                            this.Condition.Compile(stateParameterExpression, returnTarget),
                            this.Statement.Compile(stateParameterExpression, returnTarget),
                            Expression.Break(breakLabel.Target))),
                    breakLabel);
        }
    }
}