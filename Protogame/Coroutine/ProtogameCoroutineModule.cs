using Protoinject;

namespace Protogame
{
    public class ProtogameCoroutineModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind<ICoroutineScheduler>().To<DefaultCoroutineScheduler>().InSingletonScope();
            kernel.Bind<IEngineHook>().To<CoroutineEngineHook>();
            kernel.Bind<ICoroutine>().To<DefaultCoroutine>();
        }
    }
}
