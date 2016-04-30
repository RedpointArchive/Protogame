namespace Protogame
{
    /// <summary>
    /// The default update context.
    /// </summary>
    internal class DefaultUpdateContext : IUpdateContext
    {
        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Update(IGameContext context)
        {
            // No logic required for our default update context.  Normally
            // you would use this function to initialize properties of
            // the update context based on the state of the game.
        }
    }
}