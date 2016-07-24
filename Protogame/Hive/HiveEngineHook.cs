namespace Protogame
{
    /// <summary>
    /// Handles refreshing sessions and performing other periodic operations
    /// with Hive multiplayer services.
    /// </summary>
    /// <module>Hive</module>
    /// <internal>true</internal>
    public class HiveEngineHook : IEngineHook
    {
        private readonly IHiveSessionManagement _hiveSessionManagement;

        public HiveEngineHook(IHiveSessionManagement hiveSessionManagement)
        {
            _hiveSessionManagement = hiveSessionManagement;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
        }
    }
}