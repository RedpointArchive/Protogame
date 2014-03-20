namespace Protogame
{
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
        /// Creates a new instance of the specified script engine and returns it.
        /// </summary>
        /// <returns>A new instance of the specified script engine.</returns>
        public IScriptEngineInstance GetNewInstance()
        {
            return new LogicControlScriptEngineInstance(this.m_LogicScript.CreateUnmappedInstance());
        }
    }
}