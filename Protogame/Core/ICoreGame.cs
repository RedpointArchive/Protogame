namespace Protogame
{
    /// <summary>
    /// The CoreGame interface.
    /// </summary>
    public interface ICoreGame
    {
        /// <summary>
        /// Gets the game context.
        /// </summary>
        /// <value>
        /// The game context.
        /// </value>
        IGameContext GameContext { get; }

        /// <summary>
        /// Gets the render context.
        /// </summary>
        /// <value>
        /// The render context.
        /// </value>
        IRenderContext RenderContext { get; }

        /// <summary>
        /// Gets the update context.
        /// </summary>
        /// <value>
        /// The update context.
        /// </value>
        IUpdateContext UpdateContext { get; }

#if PLATFORM_ANDROID || PLATFORM_OUYA
        /// <summary>
        /// Gets the Android game view.
        /// </summary>
        /// <value>
        /// The Android game view.
        /// </value>
        Android.Views.View AndroidGameView { get; }
#endif
    }
}