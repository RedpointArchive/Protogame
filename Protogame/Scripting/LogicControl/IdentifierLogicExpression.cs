namespace LogicControl
{
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
    }
}