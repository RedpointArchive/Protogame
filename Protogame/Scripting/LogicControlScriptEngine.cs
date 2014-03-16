namespace Protogame
{
    using System.Collections.Generic;
    using System.Linq;
    using LogicControl;

    /// <summary>
    /// The LogicControl script engine.
    /// </summary>
    /// <remarks>
    /// LogicControl is a light-weight, logic focused scripting language.  It is side-effect free
    /// and semantic-based; as such it is very similar to shading languages such as HLSL.
    /// </remarks>
    public class LogicControlScriptEngine : IScriptEngine
    {
        /// <summary>
        /// The LogicControl script.
        /// </summary>
        private readonly LogicScript m_LogicScript;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicControlScriptEngine"/> class. 
        /// </summary>
        /// <remarks>
        /// Constructs a new LogicControl scripting engine with the specified script.
        /// </remarks>
        /// <param name="code">
        /// The script code.
        /// </param>
        public LogicControlScriptEngine(string code)
        {
            this.m_LogicScript = new LogicScript(code);
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
            var instance = this.m_LogicScript.CreateUnmappedInstance();
            return instance.Execute(functionName, semanticArguments);
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
            var instance = this.m_LogicScript.CreateUnmappedInstance();
            var results = instance.Execute(functionName, semanticArguments);
            return (TOutput)results.First(x => x.Key == resultSemanticName).Value;
        }
    }
}