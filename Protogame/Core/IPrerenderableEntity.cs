namespace Protogame
{
    /// <summary>
    /// Because the prerender step must be performed before any rendering, we use an interface
    /// to filter out any entities which don't want to take part in prerendering.  This reduces
    /// the amount of iteration and calls that occur during prerendering.
    /// </summary>
    public interface IPrerenderableEntity
    {
        /// <summary>
        /// Called by the <see cref="IWorldManager"/> when it's time for this entity to be
        /// pre-rendered in the game (used by entities that set up cameras).
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        void Prerender(IGameContext gameContext, IRenderContext renderContext);
    }
}
