namespace Protogame
{
    /// <summary>
    /// An interface which represents a script engine.
    /// </summary>
    /// <remarks>
    /// If you want to implement a different scripting engine, you will need to
    /// implement this interface, as well as an asset loader, saver and raw
    /// script load strategy.
    /// </remarks>
    public interface IScriptEngine
    {
        /// <summary>
        /// Creates a new instance of the specified script engine and returns it.
        /// </summary>
        /// <returns>A new instance of the specified script engine.</returns>
        IScriptEngineInstance GetNewInstance();
    }
}