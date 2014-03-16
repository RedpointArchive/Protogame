namespace LogicControl
{
    using System;
    using System.Collections.Generic;

    public class LogicFunction : TruthfulLogicExpression
    {
        public string Name { get; set; }

        public string ReturnType { get; set; }

        public string ReturnSemantic { get; set; }

        public List<LogicStatement> Statements { get; set; }

        public List<LogicParameter> Parameters { get; set; }

        public override object Result(LogicExecutionState state)
        {
            foreach (var statement in this.Statements)
            {
                statement.Execute(state);

                if (state.Finished)
                {
                    return state.Result;
                }
            }

            return state.Result;
        }
    }
}