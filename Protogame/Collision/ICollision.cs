using System;

namespace Protogame.Collision
{
    public interface ICollision
    {
        bool CollidesWithSolid(IEntity self, IWorld world, int x, int y);
        T CollidesWith<T>(IBoundingBox self, IWorld world, int x, int y) where T : Entity;
    }
}

