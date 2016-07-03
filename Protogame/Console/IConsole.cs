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
        [Obsolete("Use IConsoleHandle.Log instead.")]
        void Log(string message);

        /// <summary>
        /// Logs a structured message to the console.  Use <see cref="IConsoleHandle.Log"/> instead of this method.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void LogStructured(INode node, string format, object[] args);
    }
}