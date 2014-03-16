namespace LogicControl
{
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
    }
}