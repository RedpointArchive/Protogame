namespace LogicControl
{
    using System.Linq.Expressions;

    public class IdentifierLogicExpression : TruthfulLogicExpression
    {
        public string Identifier { get; set; }

        public IdentifierLogicExpression(string identifier)
        {
            this.Identifier = identifier;
        }

        public override object Result(LogicExecutionState state)
        {
            return state.Variables[this.Identifier];
        }

        public override Expression Compile(ParameterExpression stateParameterExpression, LabelTarget returnTarget)
        {
            return Expression.Property(
                Expression.Property(stateParameterExpression, "Variables"),
                "Item",
                Expression.Constant(this.Identifier));
        }
    }
}