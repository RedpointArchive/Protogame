namespace LogicControl
{
    public abstract class LogicAssignmentTarget
    {
        public abstract void Assign(LogicExecutionState state, object value);

        public abstract LogicExpression GetReadExpression();
    }
}