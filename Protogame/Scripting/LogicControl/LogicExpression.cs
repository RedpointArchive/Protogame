namespace LogicControl
{
    using System;

    public abstract class LogicExpression
    {
        public abstract bool Truthful(LogicExecutionState state);

        public abstract object Result(LogicExecutionState state);
    }
}