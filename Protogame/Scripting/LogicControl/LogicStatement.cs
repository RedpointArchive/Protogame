namespace LogicControl
{
    public abstract class LogicStatement
    {
        public abstract void Execute(LogicExecutionState state);
    }
}