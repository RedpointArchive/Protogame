using System;
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

        public int Priority => 150;

        public void Assign(IKernel kernel)
        {
            _kernel = kernel;
            _hierarchy = kernel.Hierarchy;
            _networkMessageSerialization = kernel.Get<INetworkMessageSerialization>();
            _networkEngine = kernel.Get<INetworkEngine>();
            _consoleHandle = kernel.Get<IConsoleHandle>(kernel.Hierarchy.Lookup(this));
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
                        _consoleHandle.Log("Got entity create message, but ignored it because the client already knows about it.");
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
                        _consoleHandle.Log("Assigned initial transform to the entity.");
                        spawnedEntity.Transform.Assign(createEntityMessage.InitialTransform.DeserializeFromNetwork());
                    }

                    var networkIdentifiableEntity = spawnedEntity as INetworkIdentifiable;
                    if (networkIdentifiableEntity != null)
                    {
                        _consoleHandle.Log("Recieved network ID: " + createEntityMessage.EntityID);
                        networkIdentifiableEntity.ReceiveNetworkIDFromServer(
                            networkReceiveEvent.GameContext,
                            networkReceiveEvent.UpdateContext,
                            createEntityMessage.EntityID,
                            createEntityMessage.FrameTick);
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
                        _consoleHandle.Log("warning: got property message for missing entity {0}", entityPropertiesMessage.EntityID);
                    }
                }
            }

            return false;
        }
    }
}
