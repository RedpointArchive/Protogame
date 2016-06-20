using Protoinject;

namespace Protogame
{
    public class ProtogameNetworkModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind<INetworkMessageSerialization>().To<AheadOfTimeNetworkMessageSerialization>();
            kernel.Bind<INetworkFactory>().ToFactory();

            kernel.Bind<IEventEngine<INetworkEventContext>>().To<DefaultEventEngine<INetworkEventContext>>().InSingletonScope();
            kernel.Bind<IEventBinder<INetworkEventContext>>().To<GeneralNetworkEventBinder>();
            kernel.Bind<IEventBinder<INetworkEventContext>>().To<WorldNetworkEventBinder>();

            kernel.Bind<IEngineHook>().To<NetworkEngineHook>();
            kernel.Bind<INetworkEngine>().To<DefaultNetworkEngine>().InSingletonScope();

            kernel.Bind<INetworkMessage>().To<EntityCreateMessage>().AllowManyPerScope();
        }
    }
}