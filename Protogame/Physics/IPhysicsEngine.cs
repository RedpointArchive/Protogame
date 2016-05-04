using Jitter.Dynamics;

namespace Protogame
{
    public interface IPhysicsEngine
    {
        void Update(IGameContext gameContext, IUpdateContext updateContext);

        void RegisterRigidBodyForHasMatrixInCurrentWorld(RigidBody rigidBody, IHasMatrix hasMatrix);

        void DebugRender(IGameContext gameContext, IRenderContext renderContext);
    }
}