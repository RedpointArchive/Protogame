namespace Protogame
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an executable script.  
    /// </summary>
    public class ScriptAsset : MarshalByRefObject, IAsset
    {
        /// <summary>
        /// The script engine for this specific script.
        /// </summary>
        private readonly IScriptEngine m_ScriptEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptAsset"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the script asset.
        /// </param>
        /// <param name="code">
        /// The associated script code.
        /// </param>
        /// <param name="scriptEngine">
        /// The script engine that executes the script.
        /// </param>
        public ScriptAsset(string name, string code, IScriptEngine scriptEngine)
        {
            this.Name = name;
            this.Code = code;
            this.m_ScriptEngine = scriptEngine;
            this.ScriptEngineType = scriptEngine.GetType();
        }

        /// <summary>
        /// Gets a value indicating whether the asset only contains compiled information.
        /// </summary>
        /// <value>
        /// Whether the asset only contains compiled information.
        /// </value>
        public bool CompiledOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the name of the asset.
        /// </summary>
        /// <value>
        /// The name of the asset.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the asset only contains source information.
        /// </summary>
        /// <value>
        /// Whether the asset only contains source information.
        /// </value>
        public bool SourceOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the script code.  Modifying this value
        /// will not change the internal representation of the
        /// script to be executed.
        /// </summary>
        /// <value>
        /// The script code.
        /// </value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the type of script engine associated with this script.
        /// </summary>
        /// <value>
        /// The type of script engine associated with this script.
        /// </value>
        public Type ScriptEngineType { get; set; }

        /// <summary>
        /// Attempt to resolve this asset to the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The target type of the asset.
        /// </typeparam>
        /// <returns>
        /// The current asset as a <see cref="T"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the current asset can not be casted to the designated type.
        /// </exception>
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(ScriptAsset)))
            {
                return this as T;
            }

            throw new InvalidOperationException("Asset already resolved to ScriptAsset.");
        }

        /// <summary>
        /// Creates a new instance of the script which can then be executed.
        /// </summary>
        /// <returns>A new instance of the script which can then be executed.</returns>
        public ScriptAssetInstance CreateInstance()
        {
            return new ScriptAssetInstance(this.m_ScriptEngine.GetNewInstance());
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
            return this.CreateInstance().Execute(functionName, semanticArguments);
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
            return this.CreateInstance().Execute<TOutput>(functionName, resultSemanticName, semanticArguments);
        }
    }
}