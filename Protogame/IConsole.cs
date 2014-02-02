namespace Protogame
{
    /// <summary>
    /// The Console interface.
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// Gets a value indicating whether open.
        /// </summary>
        /// <value>
        /// The open.
        /// </value>
        bool Open { get; }

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
        /// The toggle.
        /// </summary>
        void Toggle();

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