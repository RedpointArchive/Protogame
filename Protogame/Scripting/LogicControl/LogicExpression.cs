namespace LogicControl
{
    using System;
    using System.Linq.Expressions;

    public abstract class LogicExpression
    {
        public abstract bool Truthful(LogicExecutionState state);

        public abstract object Result(LogicExecutionState state);

        public abstract Expression Compile(ParameterExpression stateParameterExpression, LabelTarget returnTarget);
    }
}