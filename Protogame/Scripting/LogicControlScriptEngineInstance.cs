namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LogicControl;

    /// <summary>
    /// The LogicControl script engine instance.
    /// </summary>
    public class LogicControlScriptEngineInstance : IScriptEngineInstance
    {
        /// <summary>
        /// The instance of the LogicControl script.
        /// </summary>
        private readonly LogicUnmappedScriptInstance m_Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicControlScriptEngineInstance"/> class. 
        /// </summary>
        /// <param name="instance">
        /// The script instance.
        /// </param>
        public LogicControlScriptEngineInstance(LogicUnmappedScriptInstance instance)
        {
            this.m_Instance = instance;
        }

        /// <summary>
        /// Register an application function with the scripting engine instance.
        /// </summary>
        /// <param name="functionName">The name of the function inside the scripting engine.</param>
        /// <param name="callback">The application callback.</param>
        public void RegisterApplicationFunction(string functionName, Func<object[], object> callback)
        {
            this.m_Instance.RegisterApplicationFunction(functionName, callback);
        }

        /// <summary>
        /// Execute a specified scripting method with semantic-based arguments and return the results.
        /// </summary>
        /// <param name="functionName">
        /// The name of the script function to execute.
        /// </param>
        /// <param name="semanticArguments">
        /// The semantic-based arguments to pass to the script function.
        /// </param>
        /// <returns>
        /// A dictionary of results, where the key is the semantic name associated with the result.
        /// </returns>
        public Dictionary<string, object> Execute(string functionName, Dictionary<string, object> semanticArguments)
        {
            return this.m_Instance.Execute(functionName, semanticArguments);
        }

        /// <summary>
        /// Execute a specified scripting method with semantic-based arguments and return a single result.
        /// </summary>
        /// <typeparam name="TOutput">
        /// The C# type that the result should be as.
        /// </typeparam>
        /// <param name="functionName">
        /// The name of the script function to execute.
        /// </param>
        /// <param name="resultSemanticName">
        /// The semantic name of the return value to return.
        /// </param>
        /// <param name="semanticArguments">
        /// The semantic-based arguments to pass to the script function.
        /// </param>
        /// <returns>
        /// A dictionary of results, where the key is the semantic name associated with the result.
        /// </returns>
        public TOutput Execute<TOutput>(
            string functionName, 
            string resultSemanticName, 
            Dictionary<string, object> semanticArguments)
        {
            var results = this.m_Instance.Execute(functionName, semanticArguments);
            return (TOutput)results.First(x => x.Key == resultSemanticName).Value;
        }
    }
}