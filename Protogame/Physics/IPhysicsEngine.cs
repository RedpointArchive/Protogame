using System;
using Jitter.Dynamics;
using Jitter;

namespace Protogame
{
    public interface IPhysicsEngine
    {
        void UpdateWorld(JitterWorld world, IGameContext gameContext, IUpdateContext updateContext);

        void MapPhysicsToEntity(RigidBody body, IPhysicsEntity entity);

        void MapEntityToPhysics(IPhysicsEntity entity, RigidBody body);
    }
}

