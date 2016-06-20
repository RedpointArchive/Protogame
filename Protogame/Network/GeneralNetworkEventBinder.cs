using System.Linq;
using Protoinject;

namespace Protogame
{
    public class GeneralNetworkEventBinder : IEventBinder<INetworkEventContext>
    {
        private IHierarchy _hierarchy;

        public int Priority => 100;

        public void Assign(IKernel kernel)
        {
            _hierarchy = kernel.Hierarchy;
        }

        public bool Handle(INetworkEventContext context, IEventEngine<INetworkEventContext> eventEngine, Event @event)
        {
            var networkEvent = @event as NetworkEvent;
            if (networkEvent == null)
            {
                return false;
            }

            var networkMessageReceivedEvent = @event as NetworkMessageReceivedEvent;
            var networkClientConnectedEvent = @event as NetworkClientConnectedEvent;
            var networkClientDisconnectedEvent = @event as NetworkClientDisconnectedEvent;

            if (networkEvent.GameContext != null)
            {
                var networkedWorld = networkEvent.GameContext.World as INetworkedWorld;
                if (networkedWorld != null)
                {
                    if (networkMessageReceivedEvent != null)
                    {
                        if (networkedWorld.ReceiveMessage(
                            networkEvent.GameContext,
                            networkEvent.UpdateContext,
                            networkEvent.Dispatcher,
                            networkEvent.Client,
                            networkMessageReceivedEvent.Payload,
                            networkMessageReceivedEvent.ProtocolID))
                        {
                            return true;
                        }
                    }

                    if (networkClientConnectedEvent != null)
                    {
                        if (networkedWorld.ClientConnected(
                            networkEvent.GameContext,
                            networkEvent.UpdateContext,
                            networkEvent.Dispatcher,
                            networkEvent.Client))
                        {
                            return true;
                        }
                    }

                    if (networkClientDisconnectedEvent != null)
                    {
                        if (networkedWorld.ClientDisconnected(
                            networkEvent.GameContext,
                            networkEvent.UpdateContext,
                            networkEvent.Dispatcher,
                            networkEvent.Client))
                        {
                            return true;
                        }
                    }
                }

                foreach (var entity in networkEvent.GameContext.World.GetEntitiesForWorld(_hierarchy).OfType<IEventListener<INetworkEventContext>>())
                {
                    if (entity.Handle(context, eventEngine, @event))
                    {
                        return true;
                    }
                }
            }
            else if (networkEvent.ServerContext != null)
            {
                var networkedWorld = networkEvent.ServerContext.World as INetworkedServerWorld;
                if (networkedWorld != null)
                {
                    if (networkMessageReceivedEvent != null)
                    {
                        if (networkedWorld.ReceiveMessage(
                            networkEvent.ServerContext,
                            networkEvent.UpdateContext,
                            networkEvent.Dispatcher,
                            networkEvent.Client,
                            networkMessageReceivedEvent.Payload,
                            networkMessageReceivedEvent.ProtocolID))
                        {
                            return true;
                        }
                    }

                    if (networkClientConnectedEvent != null)
                    {
                        if (networkedWorld.ClientConnected(
                            networkEvent.ServerContext,
                            networkEvent.UpdateContext,
                            networkEvent.Dispatcher,
                            networkEvent.Client))
                        {
                            return true;
                        }
                    }

                    if (networkClientDisconnectedEvent != null)
                    {
                        if (networkedWorld.ClientDisconnected(
                            networkEvent.ServerContext,
                            networkEvent.UpdateContext,
                            networkEvent.Dispatcher,
                            networkEvent.Client))
                        {
                            return true;
                        }
                    }
                }

                foreach (var entity in networkEvent.ServerContext.World.GetEntitiesForWorld(_hierarchy).OfType<IEventListener<INetworkEventContext>>())
                {
                    if (entity.Handle(context, eventEngine, @event))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
