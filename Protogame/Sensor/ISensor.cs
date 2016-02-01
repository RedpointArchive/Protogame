namespace Protogame
{
    /// <summary>
    /// An abstract sensor attached to this device.
    /// </summary>
    /// <module>Sensor</module>
    public interface ISensor
    {
        /// <summary>
        /// Updates the sensor during the render step of the game.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="renderContext">The current render context.</param>
        void Render(IGameContext gameContext, IRenderContext renderContext);

        /// <summary>
        /// Updates the sensor during the update step of the game.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="updateContext">The current update context.</param>
        void Update(IGameContext gameContext, IUpdateContext updateContext);
    }
}