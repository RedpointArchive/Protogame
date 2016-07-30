using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The Protogame module for interacting with Hive Multiplayer Services.
    /// </summary>
    public class ProtogameHiveModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind<IHivePublicAuthentication>().To<DefaultHivePublicAuthentication>().InSingletonScope();
            kernel.Bind<IHiveSessionManagement>().To<DefaultHiveSessionManagement>().InSingletonScope();
            kernel.Bind<IHiveLobbyManagement>().To<DefaultHiveLobbyManagement>().InSingletonScope();
            kernel.Bind<IEngineHook>().To<HiveEngineHook>();
        }
    }
}
