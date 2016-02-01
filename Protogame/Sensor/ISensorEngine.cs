namespace Protogame
{
    /// <summary>
    /// The sensor engine which automatically updates registered sensors.
    /// <para>
    /// Because hardware sensors may activate hardware components, or may
    /// require additional permissions on some operating systems, they are
    /// not activated by default.  Instead, you need to <see cref="Register"/>
    /// sensors after you inject them into your class.
    /// </para>
    /// </summary>
    /// <module>Sensor</module>
    public interface ISensorEngine
    {
        /// <summary>
        /// Internally called by <see cref="SensorEngineHook"/> to update sensors
        /// during the render step.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="renderContext">The current render context.</param>
        void Render(IGameContext gameContext, IRenderContext renderContext);

        /// <summary>
        /// Internally called by <see cref="SensorEngineHook"/> to update sensors
        /// during the update step.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="updateContext">The current update context.</param>
        void Update(IGameContext gameContext, IUpdateContext updateContext);

        /// <summary>
        /// Registers a hardware sensor with the sensor engine, ensuring that it is
        /// updated as the game runs.
        /// </summary>
        /// <param name="sensor">The hardware sensor to register.</param>
        void Register(ISensor sensor);

        /// <summary>
        /// Deregisters a hardware sensor from the sensor engine, ensuring that it is
        /// no longer updated as the game runs.
        /// </summary>
        /// <param name="sensor">The hardware sensor to deregister.</param>
        void Deregister(ISensor sensor);
    }
}