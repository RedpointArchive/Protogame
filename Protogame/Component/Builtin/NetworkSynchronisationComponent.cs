using System.Collections.Generic;
using System.Linq;
using System.Net;
using Protoinject;

namespace Protogame
{
    public class NetworkSynchronisationComponent : IUpdatableComponent, IServerUpdatableComponent, INetworkedComponent, INetworkIdentifiable
    {
        private readonly INetworkEngine _networkEngine;
        private readonly IUniqueIdentifierAllocator _uniqueIdentifierAllocator;
        private readonly INetworkMessageSerialization _networkMessageSerialization;

        private int? _uniqueIdentifierForEntity;

        private readonly List<IPEndPoint> _clientsEntityIsKnownOn;

        public NetworkSynchronisationComponent(
            INetworkEngine networkEngine,
            IUniqueIdentifierAllocator uniqueIdentifierAllocator,
            INetworkMessageSerialization networkMessageSerialization)
        {
            _networkEngine = networkEngine;
            _uniqueIdentifierAllocator = uniqueIdentifierAllocator;
            _networkMessageSerialization = networkMessageSerialization;

            _clientsEntityIsKnownOn = new List<IPEndPoint>();
        }

        public bool ServerOnly { get; set; }

        public void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext)
        {
            
        }

        public void Update(ComponentizedEntity entity, IServerContext serverContext, IUpdateContext updateContext)
        {
            if (_uniqueIdentifierForEntity == null)
            {
                _uniqueIdentifierForEntity = _uniqueIdentifierAllocator.Allocate();
                _networkEngine.RegisterObjectAsNetworkId(_uniqueIdentifierForEntity.Value, entity);
            }

            if (ServerOnly)
            {
                return;
            }

            foreach (var dispatcher in _networkEngine.CurrentDispatchers)
            {
                // TODO: Tracking clients by endpoint almost certainly needs to change...
                foreach (var endpoint in dispatcher.Endpoints)
                {
                    if (!_clientsEntityIsKnownOn.Contains(endpoint))
                    {
                        // Send an entity creation message to the client.
                        var createMessage = new EntityCreateMessage
                        {
                            EntityID = _uniqueIdentifierForEntity.Value,
                            EntityType = entity.GetType().AssemblyQualifiedName,
                            InitialTransform = entity.Transform.SerializeToNetwork(),
                        };
                        dispatcher.Send(
                            endpoint,
                            _networkMessageSerialization.Serialize(createMessage),
                            true);

                        _clientsEntityIsKnownOn.Add(endpoint);
                    }
                }
            }
        }

        public bool ReceiveMessage(IGameContext gameContext, IUpdateContext updateContext, MxDispatcher dispatcher, MxClient server,
            byte[] payload, uint protocolId)
        {
            return false;
        }

        public bool ReceiveMessage(IServerContext serverContext, IUpdateContext updateContext, MxDispatcher dispatcher, MxClient client,
            byte[] payload, uint protocolId)
        {
            return false;
        }

        public void ReceiveNetworkIDFromServer(IGameContext gameContext, IUpdateContext updateContext, int identifier)
        {
            _uniqueIdentifierForEntity = identifier;
        }

        public void ReceivePredictedNetworkIDFromClient(IServerContext serverContext, IUpdateContext updateContext, MxClient client,
            int predictedIdentifier)
        {
            
        }
    }
}
