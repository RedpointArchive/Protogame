namespace Protogame
{
    /// <summary>
    /// The UpdateContext interface.
    /// <para>
    /// This interface provides a mechanism to perform additional update
    /// logic based on the current state of the game.
    /// </para>
    /// <para>
    /// Bind this interface in the Ninject kernel before creating an instance
    /// of your game, and the <see cref="CoreGame&lt;TInitialWorld, TWorldManager&gt;"/>
    /// implementation will create an instance of the bound type and use it
    /// during execution.
    /// </para>
    /// </summary>
    /// <module>Core API</module>
    public interface IUpdateContext
    {
        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        void Update(IGameContext context);
    }
}