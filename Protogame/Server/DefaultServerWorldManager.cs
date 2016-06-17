using System.Linq;

namespace Protogame
{
    public class DefaultServerWorldManager : IServerWorldManager
    {
        public void Update<T>(T server) where T : Server, ICoreServer
        {
            foreach (var entity in server.ServerContext.World.GetEntitiesForWorld(server.ServerContext.Hierarchy).ToList())
            {
                entity.Update(server.ServerContext, server.UpdateContext);
            }

            server.ServerContext.World.Update(server.ServerContext, server.UpdateContext);
        }
    }
}

