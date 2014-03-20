namespace Protogame
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An interface which represents a script engine instance.
    /// </summary>
    /// <remarks>
    /// If you want to implement a different scripting engine, you will need to
    /// implement this interface, as well as an asset loader, saver and raw
    /// script load strategy.
    /// </remarks>
    public interface IScriptEngineInstance
    {
        /// <summary>
        /// Register an application function with the scripting engine instance.
        /// </summary>
        /// <param name="functionName">The name of the function inside the scripting engine.</param>
        /// <param name="callback">The application callback.</param>
        void RegisterApplicationFunction(string functionName, Func<object[], object> callback);

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
        Dictionary<string, object> Execute(string functionName, Dictionary<string, object> semanticArguments);

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
        TOutput Execute<TOutput>(string functionName, string resultSemanticName, Dictionary<string, object> semanticArguments);
    }
}