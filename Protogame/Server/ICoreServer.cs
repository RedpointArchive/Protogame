namespace Protogame
{
    using System;

    public interface ICoreServer
    {
        /// <summary>
        /// Gets the server context.
        /// </summary>
        /// <value>
        /// The server context.
        /// </value>
        IServerContext ServerContext { get; }

        /// <summary>
        /// Gets the update context.
        /// </summary>
        /// <value>
        /// The update context.
        /// </value>
        IUpdateContext UpdateContext { get; }
    }
}

