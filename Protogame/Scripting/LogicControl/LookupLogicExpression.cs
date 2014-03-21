namespace LogicControl
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Expr = System.Linq.Expressions.Expression;

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

        public override Expression Compile(ParameterExpression stateParameterExpression, LabelTarget returnTarget)
        {

            Expression<Func<LogicStructureInstance, string, LogicField>> lookupField =
                (x, z) => x.Fields.Keys.First(y => y.Name == z);

            return
                Expr.Property(
                    Expr.Property(
                        Expr.Convert(
                            this.Expression.Compile(stateParameterExpression, returnTarget),
                            typeof(LogicStructureInstance)),
                        "Fields"),
                    "Item",
                    Expr.Invoke(
                        lookupField,
                        Expr.Convert(
                            this.Expression.Compile(stateParameterExpression, returnTarget),
                            typeof(LogicStructureInstance)),
                        Expr.Constant(this.Field)));
        }
    }
}