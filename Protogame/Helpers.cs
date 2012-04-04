using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Protogame
{
    public static class Helpers
    {
        public static bool CollidesWithSolidAt(Entity self, World world, int x, int y)
        {
            ReadOnlyCollection<Tile> linear = world.Tileset.AsLinear();
            BoundingBox src = new BoundingBox(x, y, self.Width, self.Height);
            for (int i = 0; i < linear.Count; i += 1)
            {
                Tile t = linear[i];
                if (t is ISolid)
                    if (BoundingBox.Check(src, t))
                        return true;
            }
            foreach (IEntity e in world.Entities)
            {
                if (e is ISolid)
                    if (BoundingBox.Check(src, e as IBoundingBox))
                        return true;
            }
            return false;
        }

        public static T CollidesAt<T>(Entity self, World world, int x, int y) where T : Entity
        {
            ReadOnlyCollection<IEntity> actors = world.Entities.AsReadOnly();
            BoundingBox src = new BoundingBox(x, y, self.Width, self.Height);
            foreach (IEntity e in actors)
            {
                if (e is T)
                    if (BoundingBox.Check(src, e as T))
                        return e as T;
            }
            return null;
        }

        public static T CollidesAt<T>(Tile self, World world, int x, int y) where T : Entity
        {
            ReadOnlyCollection<IEntity> actors = world.Entities.AsReadOnly();
            BoundingBox src = new BoundingBox(x, y, self.Width, self.Height);
            foreach (IEntity e in actors)
            {
                if (e is T)
                    if (BoundingBox.Check(src, e as T))
                        return e as T;
            }
            return null;
        }
    }
}
