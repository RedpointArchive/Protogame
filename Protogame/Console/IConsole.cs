using System;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The Console interface.
    /// </summary>
    public interface IConsole
    {
        bool Open { get; }
        
        void Render(IGameContext gameContext, IRenderContext renderContext);
        
        void Toggle();
        
        void Update(IGameContext gameContext, IUpdateContext updateContext);

        /// <summary>
        /// Add a message to the console.
        /// </summary>
        /// <param name="message">The message to add.</param>
        [Obsolete("Use IConsoleHandle.Log methods instead.")]
        void Log(string message);

        /// <summary>
        /// Logs a structured message to the console.  Use one of the <c>Log</c> methods on <see cref="IConsoleHandle"/>
        /// instead of this method.
        /// </summary>
        /// <param name="node">The node this message is being logged from.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The arguments for the formatted string.</param>
        [Obsolete("Use IConsoleHandle.Log methods instead.")]
        void LogStructured(INode node, string format, object[] args);

        /// <summary>
        /// Logs a structured message to the console.  Use one of the <c>Log</c> methods on <see cref="IConsoleHandle"/>
        /// instead of this method.
        /// </summary>
        /// <param name="node">The node this message is being logged from.</param>
        /// <param name="logLevel">The logging level of the message.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The arguments for the formatted string.</param>
        void LogStructured(INode node, ConsoleLogLevel logLevel, string format, object[] args);
    }
}