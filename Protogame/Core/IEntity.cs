namespace Protogame
{
    /// <summary>
    /// Represents an entity.
    /// <para>
    /// For a base implementation that has the required interfaces implemented, inherit from <see cref="Entity"/>.
    /// </para>
    /// <para>
    /// For a base implementation that supports components, inherit from <see cref="ComponentizedEntity"/>.
    /// </para>
    /// </summary>
    /// <module>Core API</module>
    public interface IEntity : IHasTransform
    {
        /// <summary>
        /// Called by the <see cref="IWorldManager"/> when it's time for this entity to be
        /// rendered in the game.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        void Render(IGameContext gameContext, IRenderContext renderContext);

        /// <summary>
        /// Called by the <see cref="IWorldManager"/> when this entity's state should be
        /// updated in the game.
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