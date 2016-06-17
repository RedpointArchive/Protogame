using System.Collections.Generic;
using System.Linq;
using Protoinject;

namespace Protogame
{
    public static class KernelExtensions
    {
        public static IEnumerable<IEntity> GetEntitiesForWorld(this IWorld world, IHierarchy hierarchy)
        {
            return hierarchy.Lookup(world).Children.Select(x => x.UntypedValue).OfType<IEntity>();
        }

        public static IEnumerable<IServerEntity> GetEntitiesForWorld(this IServerWorld world, IHierarchy hierarchy)
        {
            return hierarchy.Lookup(world).Children.Select(x => x.UntypedValue).OfType<IServerEntity>();
        }
    }
}