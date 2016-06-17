using Protoinject;

namespace Protogame
{
    public interface IPhysicsFactory : IGenerateFactory
    {
        PhysicsShadowWorld CreateShadowWorld();
    }
}
