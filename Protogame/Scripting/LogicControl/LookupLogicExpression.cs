namespace LogicControl
{
    using System;
    using System.Linq;

    public class LookupLogicExpression : TruthfulLogicExpression
    {
        public LogicExpression Expression { get; set; }

        public string Field { get; set; }

        public LookupLogicExpression(LogicExpression expression, string field)
        {
            this.Expression = expression;
            this.Field = field;
        }

        public override object Result(LogicExecutionState state)
        {
            var obj = this.Expression.Result(state);

            var structureInstance = obj as LogicStructureInstance;
            if (structureInstance != null)
            {
                var field = structureInstance.Fields.Keys.First(x => x.Name == this.Field);
                return structureInstance.Fields[field];
            }

            throw new InvalidCastException();
        }
    }
}