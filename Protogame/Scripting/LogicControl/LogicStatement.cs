namespace LogicControl
{
    using System.Linq.Expressions;

    /// <summary>
    /// Represents an abstract statement in a LogicControl script.
    /// </summary>
    public abstract class LogicStatement
    {
        /// <summary>
        /// Executes the specified statement using interpretation.
        /// </summary>
        /// <param name="state">The current execution state.</param>
        public abstract void Execute(LogicExecutionState state);

        /// <summary>
        /// Compile the statement into a C# expression, used by the script compiler.
        /// </summary>
        /// <param name="stateParameterExpression">The expression that references the current execution state.</param>
        /// <param name="returnTarget">The return target for returning from code.</param>
        /// <returns>A LINQ expression.</returns>
        public abstract Expression Compile(
            ParameterExpression stateParameterExpression,
            LabelTarget returnTarget);
    }
}