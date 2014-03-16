namespace LogicControl
{
    using System.Collections.Generic;

    public class BlockLogicStatement : LogicStatement
    {
        public BlockLogicStatement(List<LogicStatement> statements)
        {
            this.Statements = statements;
        }

        public List<LogicStatement> Statements { get; set; }

        public override void Execute(LogicExecutionState state)
        {
            foreach (var statement in this.Statements)
            {
                statement.Execute(state);

                if (state.Finished)
                {
                    return;
                }
            }
        }
    }
}