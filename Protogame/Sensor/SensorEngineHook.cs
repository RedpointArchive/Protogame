namespace Protogame
{
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
    }
}