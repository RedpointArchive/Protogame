namespace Protogame
{
    /// <summary>
    /// A component which performs logic as the game ticks.  A component which implements
    /// this interface will have <see cref="Update"/> called when the parent
    /// component or entity is within the update loop.
    /// </summary>
    /// <module>Component</module>
    public interface IUpdatableComponent
    {
        /// <summary>
        /// Called by the entity or parent component to indicate that updating
        /// should be performed.
        /// </summary>
        /// <param name="entity">The entity containing all components.</param>
        /// <param name="gameContext">The game context.</param>
        /// <param name="updateContext">The update context.</param>
        void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext);
    }
}