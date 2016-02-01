using System;
using System.Collections.Generic;

namespace Protogame
{
    public class DefaultSensorEngine : ISensorEngine
    {
        private readonly List<ISensor> _sensors;

        public DefaultSensorEngine()
        {
            _sensors = new List<ISensor>();
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            foreach (var sensor in _sensors)
            {
                sensor.Render(gameContext, renderContext);
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            foreach (var sensor in _sensors)
            {
                sensor.Update(gameContext, updateContext);
            }
        }

        public void Register(ISensor sensor)
        {
            if (sensor == null)
            {
                throw new ArgumentNullException(nameof(sensor));
            }

            _sensors.Add(sensor);
        }

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