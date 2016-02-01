using System;
using System.Collections.Generic;

namespace Protogame
{
    /// <summary>
    /// The default implementation of an <see cref="ISensorEngine"/>.
    /// </summary>
    /// <module>Sensor</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.ISensorEngine</interface_ref>
    public class DefaultSensorEngine : ISensorEngine
    {
        /// <summary>
        /// The sensors that are registered with this sensor engine.
        /// </summary>
        private readonly List<ISensor> _sensors;

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultSensorEngine"/>.
        /// </summary>
        public DefaultSensorEngine()
        {
            _sensors = new List<ISensor>();
        }

        /// <summary>
        /// Internally called by <see cref="SensorEngineHook"/> to update sensors
        /// during the render step.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="renderContext">The current render context.</param>
        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            foreach (var sensor in _sensors)
            {
                sensor.Render(gameContext, renderContext);
            }
        }

        /// <summary>
        /// Internally called by <see cref="SensorEngineHook"/> to update sensors
        /// during the update step.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="updateContext">The current update context.</param>
        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            foreach (var sensor in _sensors)
            {
                sensor.Update(gameContext, updateContext);
            }
        }

        /// <summary>
        /// Registers a hardware sensor with the sensor engine, ensuring that it is
        /// updated as the game runs.
        /// </summary>
        /// <param name="sensor">The hardware sensor to register.</param>
        public void Register(ISensor sensor)
        {
            if (sensor == null)
            {
                throw new ArgumentNullException(nameof(sensor));
            }

            _sensors.Add(sensor);
        }

        /// <summary>
        /// Deregisters a hardware sensor from the sensor engine, ensuring that it is
        /// no longer updated as the game runs.
        /// </summary>
        /// <param name="sensor">The hardware sensor to deregister.</param>
        public void Deregister(ISensor sensor)
        {
            if (sensor == null)
            {
                throw new ArgumentNullException(nameof(sensor));
            }

            _sensors.Remove(sensor);
        }
    }
}