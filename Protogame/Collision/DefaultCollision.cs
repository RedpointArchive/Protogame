using System;
using System.Collections.Generic;

namespace Protogame
{
    public class DefaultCollision : ICollision
    {
        public bool CollidesWithSolid(IEntity self, IWorld world, int x, int y)
        {
            // FIXME: Implement the tileset.
            throw new NotImplementedException();
            #if LEGACY
            var linear = new List<Tile>(); //world.Tileset.AsLinear();
            var src = new BoundingBox(x, y, self.Width, self.Height);
            for (var i = 0; i < linear.Count; i += 1)
            {
                var t = linear[i];
                if (t is ISolid)
                    if (BoundingBox.Check(src, t))
                        return true;
            }
            foreach (var e in world.Entities)
            {
                if (e is ISolid)
                    if (BoundingBox.Check(src, e as IBoundingBox))
                        return true;
            }
            return false;
            #endif
        }

        public T CollidesWith<T>(IBoundingBox self, IWorld world, int x, int y) where T : Entity
        {
            #if LEGACY
            var actors = world.Entities.AsReadOnly();
            var src = new BoundingBox(x, y, self.Width, self.Height);
            foreach (var e in actors)
            {
                if (e is T)
                    if (BoundingBox.Check(src, e as T))
                        return e as T;
            }
            #endif
            return null;
        }
    }
}

