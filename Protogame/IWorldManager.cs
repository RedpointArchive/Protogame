namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The WorldManager interface.
    /// <para>
    /// A world manager handles the main game loop; that is, is the top most object
    /// responsible for rendering and updating the game.  Within a world manager, you will
    /// call Render and Update on the current world, usually after setting up the
    /// appropriate rendering contexts.
    /// </para>
    /// <para>
    /// There is a default world manager implementation bound when loading either the
    /// <see cref="Protogame2DIoCModule"/> or the <see cref="Protogame3DIoCModule"/>.
    /// You can rebind this interface to perform advanced rendering or update logic.
    /// </para>
    /// </summary>
    public interface IWorldManager
    {
        /// <summary>
        /// The main render call.  This is invoked by Protogame's <see cref="CoreGame&lt;TInitialWorld, TWorldManager&gt;"/>
        /// implementation.  You should prefer to implement a custom world manager and override rendering
        /// logic there than override the game's rendering logic, as the minimum setup and teardown is provided
        /// by the game itself.
        /// </summary>
        /// <param name="game">
        /// The current game instance.
        /// </param>
        /// <typeparam name="T">
        /// The type of the game instance that is running.
        /// </typeparam>
        void Render<T>(T game) where T : Game, ICoreGame;

        /// <summary>
        /// The main update call.  This is invoked by Protogame's <see cref="CoreGame&lt;TInitialWorld, TWorldManager&gt;"/>
        /// implementation.  You should prefer to implement a custom world manager and override update
        /// logic there than override the game's update logic, as the minimum setup and teardown is provided
        /// by the game itself.
        /// </summary>
        /// <param name="game">
        /// The current game instance.
        /// </param>
        /// <typeparam name="T">
        /// The type of the game instance that is running.
        /// </typeparam>
        void Update<T>(T game) where T : Game, ICoreGame;
    }
}