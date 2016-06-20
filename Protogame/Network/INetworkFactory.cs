using Protoinject;

namespace Protogame
{
    public interface INetworkFactory : IGenerateFactory
    {
        NetworkShadowWorld CreateShadowWorld();
    }
}