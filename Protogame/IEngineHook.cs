namespace Protogame
{
    /// <summary>
    /// An interface representing a low-level engine hook.  These hooks are should be registered
    /// in the dependency injection container.  All registered engine hooks will be automatically
    /// fired during execution of the game.
    /// </summary>
    /// <module>Core API</module>
    public interface IEngineHook
    {
        /// <summary>
        /// The render callback for the engine hook.  This is triggered right before the rendering
        /// of the world manager.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        void Render(IGameContext gameContext, IRenderContext renderContext);

        /// <summary>
        /// The update callback for the engine hook.  This is triggered right before the update of the
        /// world manager.
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