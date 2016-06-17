namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="IEngineHook"/>.
    /// </summary>
    /// <module>Sensor</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IEngineHook</interface_ref>
    public class SensorEngineHook : IEngineHook
    {
        private readonly ISensorEngine _sensorEngine;

        public SensorEngineHook(ISensorEngine sensorEngine)
        {
            _sensorEngine = sensorEngine;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            _sensorEngine.Render(gameContext, renderContext);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            _sensorEngine.Update(gameContext, updateContext);
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
        }
    }
}