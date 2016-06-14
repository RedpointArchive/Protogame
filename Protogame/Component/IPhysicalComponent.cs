using Jitter.Dynamics;

namespace Protogame
{
    public interface IPhysicalComponent
    {
        RigidBody[] RigidBodies { get; }
    }
}
