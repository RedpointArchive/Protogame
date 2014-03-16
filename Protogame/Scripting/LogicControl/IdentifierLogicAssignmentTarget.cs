namespace LogicControl
{
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
    }
}