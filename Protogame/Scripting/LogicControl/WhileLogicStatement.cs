namespace LogicControl
{
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
    }
}