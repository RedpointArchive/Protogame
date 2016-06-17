namespace Protogame
{
    using System;

    public interface IServerWorld : IDisposable
    {
        /// <summary>
        /// This is called by <see cref="IServerWorldManager"/> after all of the entities have been updated.
        /// </summary>
        /// <param name="serverContext">
        /// The server context.
        /// </param>
        /// <param name="updateContext">
        /// The update context.
        /// </param>
        void Update(IServerContext serverContext, IUpdateContext updateContext);
    }
}

