namespace Protogame
{
    /// <summary>
    /// The Entity interface.
    /// </summary>
    public interface IEntity : IHasPosition
    {
        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        void Render(IGameContext gameContext, IRenderContext renderContext);

        /// <summary>
        /// The update.
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