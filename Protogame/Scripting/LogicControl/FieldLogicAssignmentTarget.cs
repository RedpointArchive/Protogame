namespace LogicControl
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public class FieldLogicAssignmentTarget : LogicAssignmentTarget
    {
        public FieldLogicAssignmentTarget(LogicExpression target, string field)
        {
            this.Target = target;
            this.Field = field;
        }

        public string Field { get; set; }

        public LogicExpression Target { get; set; }

        public override void Assign(LogicExecutionState state, object value)
        {
            var target = this.Target.Result(state);

            var structureInstance = target as LogicStructureInstance;
            if (structureInstance != null)
            {
                var field = structureInstance.Fields.Keys.First(x => x.Name == this.Field);
                structureInstance.Fields[field] = value;
                structureInstance.FieldsSet[field] = true;
                return;
            }

            throw new InvalidCastException();
        }

        public override LogicExpression GetReadExpression()
        {
            return new LookupLogicExpression(this.Target, this.Field);
        }

        public override Expression Compile(ParameterExpression stateParameterExpression, LabelTarget returnTarget, LogicExpression valueExpression)
        {
            var value = this.Target.Compile(stateParameterExpression, returnTarget);
            var convertedValue = Expression.Convert(value, typeof(LogicStructureInstance));

            var valueVariable = Expression.Variable(typeof(LogicStructureInstance), "value");
            var field = Expression.Variable(typeof(LogicField), "field");

            Expression<Func<LogicStructureInstance, string, LogicField>> lookupField =
                (x, z) => x.Fields.Keys.First(y => y.Name == z);

            var assignValue = Expression.Assign(valueVariable, convertedValue);
            var assignFieldName = Expression.Assign(
                field,
                Expression.Invoke(lookupField, valueVariable, Expression.Constant(this.Field)));
            var assignField =
                Expression.Assign(
                    Expression.Property(Expression.Property(valueVariable, "Fields"), "Item", field),
                    Expression.Convert(valueExpression.Compile(stateParameterExpression, returnTarget), typeof(object)));
            var assignFieldSet =
                Expression.Assign(
                    Expression.Property(Expression.Property(valueVariable, "FieldsSet"), "Item", field),
                    Expression.Constant(true));

            return Expression.Block(
                new[] { valueVariable, field },
                assignValue,
                assignFieldName,
                assignField,
                assignFieldSet);
        }
    }
}