namespace LogicControl
{
    using System.Linq.Expressions;

    public class IdentifierLogicAssignmentTarget : LogicAssignmentTarget
    {
        public string Identifier { get; set; }

        public IdentifierLogicAssignmentTarget(string identifier)
        {
            this.Identifier = identifier;
        }

        public override void Assign(LogicExecutionState state, object value)
        {
            state.Variables[this.Identifier] = value;
        }

        public override LogicExpression GetReadExpression()
        {
            return new IdentifierLogicExpression(this.Identifier);
        }

        public override Expression Compile(
            ParameterExpression stateParameterExpression,
            LabelTarget returnTarget,
            LogicExpression valueExpression)
        {
            return
                Expression.Assign(
                    Expression.Property(
                        Expression.Property(stateParameterExpression, "Variables"),
                        "Item",
                        Expression.Constant(this.Identifier)),
                    Expression.Convert(valueExpression.Compile(stateParameterExpression, returnTarget), typeof(object)));
        }
    }
}