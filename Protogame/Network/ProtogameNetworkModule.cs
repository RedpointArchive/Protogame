using Protoinject;

namespace Protogame
{
    public class ProtogameNetworkModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind<INetworkMessageSerialization>().To<AheadOfTimeNetworkMessageSerialization>();
        }
    }
}