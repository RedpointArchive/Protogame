namespace Protogame
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The World interface.
    /// <para>
    /// A world is a container for the entities and logic of the current game state.  It encapsulates
    /// the general appearance and logic of the game while the world is active (unlike <see cref="IWorldManager"/>
    /// which applies throughout the execution of the entire game).
    /// </para>
    /// <para>
    /// This is an interface which all of the worlds you define should implement.
    /// </para>
    /// </summary>
    public interface IWorld : IDisposable
    {
        /// <summary>
        /// Gets the entities.
        /// <para>
        /// This is a list of entities that are currently present in the world.  The <see cref="IWorldManager"/>
        /// will use this list to invoke the rendering and update of entities at the appropriate time.
        /// </para>
        /// </summary>
        /// <value>
        /// The entities.
        /// </value>
        List<IEntity> Entities { get; }

        /// <summary>
        /// This is called by <see cref="IWorldManager"/> when rendering has started, but no entities have yet
        /// been rendering in the current context.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        void RenderBelow(IGameContext gameContext, IRenderContext renderContext);

        /// <summary>
        /// This is called by <see cref="IWorldManager"/> when rendering is about to finish.  Rendering
        /// has not yet been completely finalized, but all entities have been rendered in the current context.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        void RenderAbove(IGameContext gameContext, IRenderContext renderContext);

        /// <summary>
        /// This is called by <see cref="IWorldManager"/> after all of the entities have been updated.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="updateContext">
        /// The update context.
        /// </param>
        void Update(IGameContext gameContext, IUpdateContext updateContext);
    }
}