namespace LogicControl
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// The LogicScript compiler.
    /// </summary>
    public static class LogicScriptCompiler
    {
        /// <summary>
        /// Compile the specified function into a C# delegate, dramatically increasing the performance of execution.
        /// </summary>
        /// <param name="function">
        /// The function to compile.
        /// </param>
        /// <returns>
        /// The C# delegate that can be called.
        /// </returns>
        public static Func<LogicExecutionState, object> Compile(LogicFunction function)
        {
            var stateParameter = Expression.Parameter(typeof(LogicExecutionState), "state");
            var returnLabel = Expression.Label(Expression.Label(typeof(object)), Expression.Constant(null));
            var lambda =
                Expression.Lambda<Func<LogicExecutionState, object>>(
                    Expression.Block(function.Statements.Select(x => x.Compile(stateParameter, returnLabel.Target)).Concat(new[] { returnLabel })),
                    stateParameter);

            return lambda.Compile();
        }
    }
}