namespace Protogame
{
    /// <summary>
    /// A component which handles rendering.  A component which implements
    /// this interface will have <see cref="Render"/> called when the parent
    /// component or entity is within the render loop.
    /// </summary>
    /// <module>Component</module>
    public interface IRenderableComponent
    {
        /// <summary>
        /// Called by the entity or parent component to indicate that rendering
        /// should be performed.
        /// </summary>
        /// <param name="entity">The entity containing all components.</param>
        /// <param name="gameContext">The game context.</param>
        /// <param name="renderContext">The render context.</param>
        void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext);
    }
}