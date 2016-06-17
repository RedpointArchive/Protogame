using Jitter;
using Jitter.Dynamics;

namespace Protogame
{
    public interface IPhysicsEngine
    {
        void Update(IGameContext gameContext, IUpdateContext updateContext);

        void Update(IServerContext serverContext, IUpdateContext updateContext);

        void RegisterRigidBodyForHasMatrixInCurrentWorld(RigidBody rigidBody, IHasTransform hasTransform);

        void DebugRender(IGameContext gameContext, IRenderContext renderContext);

        JitterWorld GetInternalPhysicsWorld();
    }
}