using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public partial class NetworkSynchronisationComponent : IUpdatableComponent, IServerUpdatableComponent, INetworkedComponent, INetworkIdentifiable, ISynchronisationApi
    {
        private readonly INetworkEngine _networkEngine;
        private readonly IUniqueIdentifierAllocator _uniqueIdentifierAllocator;
        private readonly INetworkMessageSerialization _networkMessageSerialization;

        private int? _uniqueIdentifierForEntity;

        private readonly List<IPEndPoint> _clientsEntityIsKnownOn;

        private readonly Dictionary<string, SynchronisedData> _synchronisedData;
        private readonly List<SynchronisedData> _synchronisedDataToTransmit;
        private ISynchronisedObject _synchronisationContext;

        public NetworkSynchronisationComponent(
            INetworkEngine networkEngine,
            IUniqueIdentifierAllocator uniqueIdentifierAllocator,
            INetworkMessageSerialization networkMessageSerialization)
        {
            _networkEngine = networkEngine;
            _uniqueIdentifierAllocator = uniqueIdentifierAllocator;
            _networkMessageSerialization = networkMessageSerialization;

            _clientsEntityIsKnownOn = new List<IPEndPoint>();
            _synchronisedData = new Dictionary<string, SynchronisedData>();
            _synchronisedDataToTransmit = new List<SynchronisedData>();
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
            
            // Sync the entity to the client if it hasn't been already.
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

            // If the entity is a synchronised entity, collect properties of the synchronised object
            // directly.
            var synchronisedEntity = entity as ISynchronisedObject;
            if (synchronisedEntity != null)
            {
                _synchronisationContext = synchronisedEntity;
                _synchronisationContext.DeclareSynchronisedProperties(this);
            }

            // Iterate through all the components on the entity and get their synchronisation data as well.
            foreach (var synchronisedComponent in entity.Components.OfType<ISynchronisedObject>())
            {
                _synchronisationContext = synchronisedComponent;
                _synchronisationContext.DeclareSynchronisedProperties(this);
            }

            if (_synchronisedData.Count > 0)
            {
                // Now calculate the delta to transmit over the network.
                var currentTick = serverContext.Tick; // TODO: Use TimeTick
                _synchronisedDataToTransmit.Clear();
                foreach (var data in _synchronisedData.Values)
                {
                    var needsSync = false;
                    if (!data.HasPerformedInitialSync)
                    {
                        needsSync = true;
                    }
                    if (data.LastValue != data.CurrentValue)
                    {
                        if (data.LastFrameSynced > currentTick + data.FrameInterval)
                        {
                            needsSync = true;
                        }
                    }

                    if (needsSync)
                    {
                        _synchronisedDataToTransmit.Add(data);
                    }
                }

                if (_synchronisedDataToTransmit.Count > 0)
                {
                    // Build up the synchronisation message.
                    var message = new EntityPropertiesMessage();
                    message.EntityID = _uniqueIdentifierForEntity.Value;
                    message.FrameTick = currentTick;
                    message.PropertyNames = new string[_synchronisedDataToTransmit.Count];
                    message.PropertyTypes = new int[_synchronisedDataToTransmit.Count];

                    AssignSyncDataToMessage(_synchronisedDataToTransmit, message, currentTick);

                    // Sync properties to each client.
                    foreach (var dispatcher in _networkEngine.CurrentDispatchers)
                    {
                        // TODO: Tracking clients by endpoint almost certainly needs to change...
                        foreach (var endpoint in dispatcher.Endpoints)
                        {
                            if (_clientsEntityIsKnownOn.Contains(endpoint))
                            {
                                // Send an entity properties message to the client.
                                dispatcher.Send(
                                    endpoint,
                                    _networkMessageSerialization.Serialize(message),
                                    false);
                            }
                        }
                    }
                }
            }
        }

        public bool ReceiveMessage(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext, MxDispatcher dispatcher, MxClient server,
            byte[] payload, uint protocolId)
        {
            if (_uniqueIdentifierForEntity == null)
            {
                return false;
            }

            var propertyMessage = _networkMessageSerialization.Deserialize(payload) as EntityPropertiesMessage;

            if (propertyMessage == null || propertyMessage.EntityID != _uniqueIdentifierForEntity.Value)
            {
                return false;
            }

            // If the entity is a synchronised entity, collect properties of the synchronised object
            // directly.
            var synchronisedEntity = entity as ISynchronisedObject;
            if (synchronisedEntity != null)
            {
                _synchronisationContext = synchronisedEntity;
                _synchronisationContext.DeclareSynchronisedProperties(this);
            }

            // Iterate through all the components on the entity and get their synchronisation data as well.
            foreach (var synchronisedComponent in entity.Components.OfType<ISynchronisedObject>())
            {
                _synchronisationContext = synchronisedComponent;
                _synchronisationContext.DeclareSynchronisedProperties(this);
            }

            AssignMessageToSyncData(propertyMessage, _synchronisedData);
            return true;
        }

        public bool ReceiveMessage(ComponentizedEntity entity, IServerContext serverContext, IUpdateContext updateContext, MxDispatcher dispatcher, MxClient client, byte[] payload, uint protocolId)
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

        public void Synchronise<T>(string name, int frameInterval, T currentValue, Action<T> setValue)
        {
            // TODO: Make this value more unique, and synchronised across the network (so we can have multiple components of the same type).
            var context = "unknown";
            if (_synchronisationContext is ComponentizedEntity)
            {
                context = "entity";
            }
            else
            {
                context = _synchronisationContext.GetType().Name;
            }
            var contextFullName = context + "." + name;

            // Find or add synchronisation data.
            SynchronisedData entry;
            if (_synchronisedData.ContainsKey(contextFullName))
            {
                entry = _synchronisedData[contextFullName];
            }
            else
            {
                _synchronisedData[contextFullName] = new SynchronisedData
                {
                    Name = contextFullName,
                    HasPerformedInitialSync = false,
                };

                entry = _synchronisedData[contextFullName];
            }

            _synchronisedData[contextFullName].IsActiveInSynchronisation = true;
            _synchronisedData[contextFullName].FrameInterval = frameInterval;
            _synchronisedData[contextFullName].LastValue = _synchronisedData[contextFullName].CurrentValue;
            _synchronisedData[contextFullName].CurrentValue = currentValue;
            _synchronisedData[contextFullName].SetValueDelegate = x => { setValue((T)x); }; // TODO: This causes a memory allocation.
        }

        private class SynchronisedData
        {
            public string Name;

            public int FrameInterval;

            public object LastValue;

            public object CurrentValue;

            public int LastFrameSynced;

            public bool HasPerformedInitialSync;

            public Action<object> SetValueDelegate;

            public bool IsActiveInSynchronisation;
        }

        #region Conversion Methods

        public float[] ConvertToVector2(object obj)
        {
            var vector = (Vector2) obj;
            return new[] { vector.X, vector.Y };
        }

        public float[] ConvertToVector3(object obj)
        {
            var vector = (Vector3)obj;
            return new[] { vector.X, vector.Y, vector.Z };
        }

        public float[] ConvertToVector4(object obj)
        {
            var vector = (Vector4)obj;
            return new[] { vector.X, vector.Y, vector.Z, vector.W };
        }

        public float[] ConvertToQuaternion(object obj)
        {
            var quat = (Quaternion)obj;
            return new[] { quat.X, quat.Y, quat.Z, quat.W };
        }

        public float[] ConvertToMatrix(object obj)
        {
            var matrix = (Matrix)obj;
            return new[]
            {
                matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                matrix.M41, matrix.M42, matrix.M43, matrix.M44,
            };
        }

        public NetworkTransform ConvertToTransform(object obj)
        {
            var transform = (ITransform)obj;
            return transform.SerializeToNetwork();
        }

        public Vector2 ConvertFromVector2(float[] obj, int offset)
        {
            return new Vector2(
                obj[offset],
                obj[offset + 1]);
        }

        public Vector3 ConvertFromVector3(float[] obj, int offset)
        {
            return new Vector3(
                obj[offset],
                obj[offset + 1],
                obj[offset + 2]);
        }

        public Vector4 ConvertFromVector4(float[] obj, int offset)
        {
            return new Vector4(
                obj[offset],
                obj[offset + 1],
                obj[offset + 2],
                obj[offset + 3]);
        }

        public Quaternion ConvertFromQuaternion(float[] obj, int offset)
        {
            return new Quaternion(
                obj[offset],
                obj[offset + 1],
                obj[offset + 2],
                obj[offset + 3]);
        }

        public Matrix ConvertFromMatrix(float[] obj, int offset)
        {
            return new Matrix(
                obj[offset],
                obj[offset + 1],
                obj[offset + 2],
                obj[offset + 3],
                obj[offset + 4],
                obj[offset + 5],
                obj[offset + 6],
                obj[offset + 7],
                obj[offset + 8],
                obj[offset + 9],
                obj[offset + 10],
                obj[offset + 11],
                obj[offset + 12],
                obj[offset + 13],
                obj[offset + 14],
                obj[offset + 15]);
        }

        public ITransform ConvertFromTransform(NetworkTransform obj)
        {
            return obj.DeserializeFromNetwork();
        }


        #endregion
    }
}
