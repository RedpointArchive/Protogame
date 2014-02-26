using Jitter.LinearMath;

namespace Protogame
{
    public class DefaultPhysicsEngine : IPhysicsEngine
    {
        public void UpdateWorld(Jitter.JitterWorld world, IGameContext gameContext, IUpdateContext updateContext)
        {
            world.Step((float)gameContext.GameTime.ElapsedGameTime.TotalSeconds, true);
        }

        public void MapPhysicsToEntity(Jitter.Dynamics.RigidBody body, IPhysicsEntity entity)
        {
            entity.X = body.Position.X;
            entity.Y = body.Position.Y;
            entity.Z = body.Position.Z;
            entity.Rotation = body.Orientation.ToXNAMatrix();
        }

        public void MapEntityToPhysics(IPhysicsEntity entity, Jitter.Dynamics.RigidBody body)
        {
            body.Position = new JVector(
                entity.X,
                entity.Y,
                entity.Z);
            body.Orientation = entity.Rotation.ToJitterMatrix();
        }
    }
}

