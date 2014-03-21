namespace LogicControl
{
    using System.Linq.Expressions;

    public abstract class LogicAssignmentTarget
    {
        public abstract void Assign(LogicExecutionState state, object value);

        public abstract LogicExpression GetReadExpression();

        public abstract Expression Compile(ParameterExpression stateParameterExpression, LabelTarget returnTarget, LogicExpression valueExpression);
    }
}