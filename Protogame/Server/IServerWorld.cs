namespace Protogame
{
    using System;
    using System.Collections.Generic;

    public interface IServerWorld : IDisposable
    {
        /// <summary>
        /// Gets the entities.
        /// <para>
        /// This is a list of entities that are currently present in the world.  The <see cref="IServerWorldManager"/>
        /// will use this list to invoke the update of entities at the appropriate time.
        /// </para>
        /// </summary>
        /// <value>
        /// The entities.
        /// </value>
        IList<IServerEntity> Entities { get; }

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

