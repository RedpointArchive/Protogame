using System;
using System.Collections.Generic;
using System.Linq;
using Protoinject;

namespace Protogame
{
    public class WorldNetworkEventBinder : IEventBinder<INetworkEventContext>
    {
        private IKernel _kernel;
        private IHierarchy _hierarchy;
        private INetworkMessageSerialization _networkMessageSerialization;
        private INetworkEngine _networkEngine;
        private IConsoleHandle _consoleHandle;
        private Dictionary<int, List<Tuple<int, Event>>> _pendingEntityPropertyMessages;

        public int Priority => 150;

        public void Assign(IKernel kernel)
        {
            _kernel = kernel;
            _hierarchy = kernel.Hierarchy;
            _networkMessageSerialization = kernel.Get<INetworkMessageSerialization>();
            _networkEngine = kernel.Get<INetworkEngine>();
            _consoleHandle = kernel.Get<IConsoleHandle>(kernel.Hierarchy.Lookup(this));
            _pendingEntityPropertyMessages = new Dictionary<int, List<Tuple<int, Event>>>();
        }

        public bool Handle(INetworkEventContext context, IEventEngine<INetworkEventContext> eventEngine, Event @event)
        {
            var networkReceiveEvent = @event as NetworkMessageReceivedEvent;
            if (networkReceiveEvent == null)
            {
                return false;
            }

            var @object = _networkMessageSerialization.Deserialize(networkReceiveEvent.Payload);

            if (networkReceiveEvent.GameContext != null)
            {
                // Messages which are only allowed to be handled by the client.

                var createEntityMessage = @object as EntityCreateMessage;
                if (createEntityMessage != null)
                {
                    if (_networkEngine.FindObjectByNetworkId(createEntityMessage.EntityID) != null)
                    {
                        // This entity was already created on the client, so we ignore it.
                        return true;
                    }

                    // Spawn an entity in the world...
                    var world = networkReceiveEvent.GameContext.World;
                    var spawnedEntity = _kernel.Get(
                        Type.GetType(createEntityMessage.EntityType),
                        _hierarchy.Lookup(world)) as IEntity;

                    _networkEngine.RegisterObjectAsNetworkId(
                        createEntityMessage.EntityID,
                        spawnedEntity);

                    if (spawnedEntity != null)
                    {
                        spawnedEntity.Transform.Assign(createEntityMessage.InitialTransform.DeserializeFromNetwork());
                    }

                    var networkIdentifiableEntity = spawnedEntity as INetworkIdentifiable;
                    if (networkIdentifiableEntity != null)
                    {
                        networkIdentifiableEntity.ReceiveNetworkIDFromServer(
                            networkReceiveEvent.GameContext,
                            networkReceiveEvent.UpdateContext,
                            createEntityMessage.EntityID,
                            createEntityMessage.FrameTick);
                    }

                    // Send any pending property messages?
                    var networkEventListener = spawnedEntity as IEventListener<INetworkEventContext>;
                    if (networkEventListener != null)
                    {
                        if (_pendingEntityPropertyMessages.ContainsKey(createEntityMessage.EntityID))
                        {
                            foreach (var propertyMessage in _pendingEntityPropertyMessages[createEntityMessage.EntityID]
                                .Where(x => x.Item1 > createEntityMessage.MessageOrder).OrderBy(x => x.Item1))
                            {
                                networkEventListener.Handle(context, eventEngine, propertyMessage.Item2);
                            }

                            _pendingEntityPropertyMessages.Remove(createEntityMessage.EntityID);
                        }
                    }

                    return true;
                }

                var entityPropertiesMessage = @object as EntityPropertiesMessage;
                if (entityPropertiesMessage != null)
                {
                    var targetObject = _networkEngine.FindObjectByNetworkId(entityPropertiesMessage.EntityID);
                    if (targetObject != null)
                    {
                        // The object willingly didn't accept the message.
                    }
                    else
                    {
                        if (!_pendingEntityPropertyMessages.ContainsKey(entityPropertiesMessage.EntityID))
                        {
                            _pendingEntityPropertyMessages[entityPropertiesMessage.EntityID] = new List<Tuple<int, Event>>();
                        }

                        _pendingEntityPropertyMessages[entityPropertiesMessage.EntityID].Add(new Tuple<int, Event>(entityPropertiesMessage.MessageOrder, networkReceiveEvent));
                    }
                }
            }

            return false;
        }
    }
}
