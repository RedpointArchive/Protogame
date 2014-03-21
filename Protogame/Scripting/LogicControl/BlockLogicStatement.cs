namespace LogicControl
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

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

        public override Expression Compile(ParameterExpression stateParameterExpression, LabelTarget returnTarget)
        {
            return Expression.Block(this.Statements.Select(x => x.Compile(stateParameterExpression, returnTarget)));
        }
    }
}