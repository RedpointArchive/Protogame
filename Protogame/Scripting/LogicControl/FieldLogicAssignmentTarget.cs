namespace LogicControl
{
    using System;
    using System.Linq;

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
    }
}