using Protoinject;

namespace Protogame
{
    public interface IPerPixelCollisionFactory : IGenerateFactory
    {
        PerPixelCollisionShadowWorld CreateShadowWorld();
    }
}
