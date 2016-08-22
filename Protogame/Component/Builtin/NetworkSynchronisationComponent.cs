using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public partial class NetworkSynchronisationComponent : IUpdatableComponent, IServerUpdatableComponent, INetworkedComponent, INetworkIdentifiable, ISynchronisationApi, IRenderableComponent, IEnabledComponent
    {
        private readonly IConsoleHandle _consoleHandle;
        private readonly INetworkEngine _networkEngine;
        private readonly IUniqueIdentifierAllocator _uniqueIdentifierAllocator;
        private readonly INetworkMessageSerialization _networkMessageSerialization;
        private readonly IDebugRenderer _debugRenderer;

        private int? _uniqueIdentifierForEntity;

        private readonly List<IPEndPoint> _clientsEntityIsKnownOn;

        private readonly Dictionary<string, SynchronisedData> _synchronisedData;
        private readonly List<SynchronisedData> _synchronisedDataToTransmit;
        private ISynchronisedObject _synchronisationContext;

        private int _localTick;

        private bool _isRunningOnClient;

        private bool _serverOnly;
        private bool _enabled;

        public NetworkSynchronisationComponent(
            IConsoleHandle consoleHandle,
            INetworkEngine networkEngine,
            IUniqueIdentifierAllocator uniqueIdentifierAllocator,
            INetworkMessageSerialization networkMessageSerialization,
            IDebugRenderer debugRenderer)
        {
            _consoleHandle = consoleHandle;
            _networkEngine = networkEngine;
            _uniqueIdentifierAllocator = uniqueIdentifierAllocator;
            _networkMessageSerialization = networkMessageSerialization;
            _debugRenderer = debugRenderer;

            _clientsEntityIsKnownOn = new List<IPEndPoint>();
            _synchronisedData = new Dictionary<string, SynchronisedData>();
            _synchronisedDataToTransmit = new List<SynchronisedData>();

            _enabled = true;
        }
        
        public bool Enabled { get { return _enabled; } set { _enabled = value; } }

        /// <summary>
        /// Whether this entity and it's components only exist on the server.  If this
        /// is set to true, no data is sent to clients about this entity.
        /// </summary>
        public bool ServerOnly { get { return _serverOnly; } set { _serverOnly = value; } }

        /// <summary>
        /// Whether this entity only exists on the server and the authoritive client.  If
        /// clients have no authority over this entity, this option has no effect.  If this
        /// option is false, then all clients are made aware of a client authoritive entity
        /// (even clients who do not control it).
        /// </summary>
        public bool OnlySendToAuthoritiveClient { get; set; }

        /// <summary>
        /// The client which owns this entity.  If <see cref="ClientAuthoritiveMode"/> is
        /// anything other than <c>None</c>, then this indicates which clients can modify
        /// information about this entity on the server.  If this is <c>null</c> while
        /// <see cref="ClientAuthoritiveMode"/> is anything other than <c>None</c>, then any
        /// client can modify this entity.
        /// </summary>
        public IPEndPoint ClientOwnership { get; set; }

        /// <summary>
        /// The authority level given to clients over this entity.
        /// </summary>
        public ClientAuthoritiveMode ClientAuthoritiveMode { get; set; }

        /// <summary>
        /// The unique network ID for the entity.  This should only be used for debugging purposes.
        /// </summary>
        public int? NetworkID => _uniqueIdentifierForEntity;

        public void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext)
        {
            if (!_enabled || _serverOnly)
            {
                return;
            }

            _localTick++;
            _isRunningOnClient = true;

            // For all synchronised values, update them with their time machine if they have one.
            foreach (var data in _synchronisedData)
            {
                if (data.Value.TimeMachine != null)
                {
                    data.Value.SetValueDelegate(
                        data.Value.TimeMachine.Get(_localTick));
                }
            }

            if (!_uniqueIdentifierForEntity.HasValue)
            {
                // TODO: Support predicted entities here.
                // We don't have an identifier provided by the server, so skip all this logic.
                return;
            }

            if (ClientAuthoritiveMode != ClientAuthoritiveMode.None)
            {
                // This client has some level of authority over the entity, so send data that's important.
                switch (ClientAuthoritiveMode)
                {
                    case ClientAuthoritiveMode.TrustClient:
                        PrepareAndTransmitSynchronisation(entity, _localTick, true, ClientAuthoritiveMode);
                        break;
                    case ClientAuthoritiveMode.ReplayInputs:
                        throw new NotSupportedException("Replaying inputs provided by clients is not yet supported.");
                    default:
                        throw new InvalidOperationException("Unknown client authoritivity mode: " + ClientAuthoritiveMode);
                }

            }
        }

        public void Update(ComponentizedEntity entity, IServerContext serverContext, IUpdateContext updateContext)
        {
            if (!_enabled)
            {
                return;
            }

            _localTick++;
            _isRunningOnClient = false;

            if (_uniqueIdentifierForEntity == null)
            {
                _uniqueIdentifierForEntity = _uniqueIdentifierAllocator.Allocate();
                _networkEngine.RegisterObjectAsNetworkId(_uniqueIdentifierForEntity.Value, entity);
            }

            if (_serverOnly)
            {
                return;
            }
            
            // Sync the entity to the client if it hasn't been already.
            foreach (var dispatcher in _networkEngine.CurrentDispatchers)
            {
                // TODO: Tracking clients by endpoint almost certainly needs to change...
                foreach (var endpoint in dispatcher.Endpoints)
                {
                    if (ClientAuthoritiveMode != ClientAuthoritiveMode.None &&
                        ClientOwnership != null && 
                        OnlySendToAuthoritiveClient)
                    {
                        if (!ClientOwnership.Equals(endpoint))
                        {
                            // This client doesn't own the entity, and this entity is only
                            // synchronised with clients that own it.
                            continue;
                        }
                    }

                    if (!_clientsEntityIsKnownOn.Contains(endpoint))
                    {
                        // Send an entity creation message to the client.
                        var createMessage = new EntityCreateMessage
                        {
                            EntityID = _uniqueIdentifierForEntity.Value,
                            EntityType = entity.GetType().AssemblyQualifiedName,
                            InitialTransform = entity.Transform.SerializeToNetwork(),
                            FrameTick = _localTick
                        };
                        _networkEngine.Send(
                            dispatcher,
                            endpoint,
                            createMessage,
                            true);

                        _clientsEntityIsKnownOn.Add(endpoint);
                    }
                }
            }
            
            PrepareAndTransmitSynchronisation(entity, _localTick, false, ClientAuthoritiveMode);
        }

        public bool ReceiveMessage(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext, MxDispatcher dispatcher, MxClient server,
            byte[] payload, uint protocolId)
        {
            if (!_enabled)
            {
                return false;
            }

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

            AssignMessageToSyncData(propertyMessage, _synchronisedData, server.Endpoint);
            return true;
        }

        public bool ReceiveMessage(ComponentizedEntity entity, IServerContext serverContext, IUpdateContext updateContext, MxDispatcher dispatcher, MxClient client, byte[] payload, uint protocolId)
        {
            if (!_enabled)
            {
                return false;
            }

            if (_uniqueIdentifierForEntity == null)
            {
                return false;
            }

            // See what kind of messages we accept, based on the client authority.
            switch (ClientAuthoritiveMode)
            {
                case ClientAuthoritiveMode.None:
                    // We don't accept any client data about this entity, so ignore it.
                    return false;
                case ClientAuthoritiveMode.TrustClient:
                    {
                        // Check to see if the message is coming from a client that has authority.
                        if (ClientOwnership != null && !ClientOwnership.Equals(client.Endpoint))
                        {
                            // We don't trust this message.
                            return false;
                        }

                        // We trust the client, so process this information like a client would.
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

                        AssignMessageToSyncData(propertyMessage, _synchronisedData, client.Endpoint);
                        return true;
                    }
                case ClientAuthoritiveMode.ReplayInputs:
                    // We don't implement this yet, but we don't want to allow client packets to cause
                    // a server error, so silently consume it.
                    return false;
            }

            return false;
        }

        public void ReceiveNetworkIDFromServer(IGameContext gameContext, IUpdateContext updateContext, int identifier, int initialFrameTick)
        {
            if (!_enabled)
            {
                return;
            }

            _uniqueIdentifierForEntity = identifier;
            _localTick = initialFrameTick;
        }

        public void ReceivePredictedNetworkIDFromClient(IServerContext serverContext, IUpdateContext updateContext, MxClient client,
            int predictedIdentifier)
        {
            if (!_enabled)
            {
                return;
            }
        }

        private enum SynchroniseTargets
        {
            AllClients,

            OwningClient,

            NonOwningClients
        };

        public void Synchronise<T>(string name, int frameInterval, T currentValue, Action<T> setValue, int? timeMachineHistory)
        {
            if (!_enabled)
            {
                return;
            }

            InternalSynchronise(SynchroniseTargets.AllClients, name, frameInterval, currentValue, setValue, timeMachineHistory);
        }

        public void SynchroniseToOwner<T>(string name, int frameInterval, T currentValue, Action<T> setValue, int? timeMachineHistory)
        {
            if (!_enabled)
            {
                return;
            }

            InternalSynchronise(SynchroniseTargets.OwningClient, name, frameInterval, currentValue, setValue, timeMachineHistory);
        }

        public void SynchroniseToNonOwner<T>(string name, int frameInterval, T currentValue, Action<T> setValue, int? timeMachineHistory)
        {
            if (!_enabled)
            {
                return;
            }

            InternalSynchronise(SynchroniseTargets.NonOwningClients, name, frameInterval, currentValue, setValue, timeMachineHistory);
        }

        private void InternalSynchronise<T>(SynchroniseTargets targets, string name, int frameInterval, T currentValue, Action<T> setValue, int? timeMachineHistory)
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
                    HasPerformedInitialSync = new Dictionary<IPEndPoint, bool>(),
                    HasReceivedInitialSync = new Dictionary<IPEndPoint, bool>(),
                    LastFrameSynced = new Dictionary<IPEndPoint, int>()
                };

                entry = _synchronisedData[contextFullName];
            }

            object convertedValue = currentValue;
            if (convertedValue is ITransform)
            {
                convertedValue = ((ITransform)convertedValue).SerializeToNetwork();
            }

            _synchronisedData[contextFullName].IsActiveInSynchronisation = true;
            _synchronisedData[contextFullName].FrameInterval = frameInterval;
            _synchronisedData[contextFullName].LastValue = _synchronisedData[contextFullName].CurrentValue;
            _synchronisedData[contextFullName].CurrentValue = convertedValue;
            _synchronisedData[contextFullName].SynchronisationTargets = targets;

            // TODO: This causes a memory allocation.
            _synchronisedData[contextFullName].SetValueDelegate = x =>
            {
                var assignValue = x;
                if (assignValue is NetworkTransform)
                {
                    assignValue = ((NetworkTransform)assignValue).DeserializeFromNetwork();
                }
                setValue((T)assignValue);
            };

            if (ClientAuthoritiveMode == ClientAuthoritiveMode.None && _synchronisedData[contextFullName].TimeMachine == null)
            {
                if (timeMachineHistory != null)
                {
                    // Set up a time machine if this is a type which can be interpolated.
                    if (typeof(T) == typeof(int))
                    {
                        _synchronisedData[contextFullName].TimeMachine = new Int32TimeMachine(timeMachineHistory.Value);
                    }
                    if (typeof(T) == typeof(short))
                    {
                        _synchronisedData[contextFullName].TimeMachine = new Int16TimeMachine(timeMachineHistory.Value);
                    }
                    if (typeof(T) == typeof(bool))
                    {
                        _synchronisedData[contextFullName].TimeMachine = new BooleanTimeMachine(timeMachineHistory.Value);
                    }
                    if (typeof(T) == typeof(double))
                    {
                        _synchronisedData[contextFullName].TimeMachine = new DoubleTimeMachine(timeMachineHistory.Value);
                    }
                    if (typeof(T) == typeof(float))
                    {
                        _synchronisedData[contextFullName].TimeMachine = new SingleTimeMachine(timeMachineHistory.Value);
                    }
                    if (typeof(T) == typeof(string))
                    {
                        _synchronisedData[contextFullName].TimeMachine = new StringTimeMachine(timeMachineHistory.Value);
                    }
                    if (typeof(T) == typeof(Vector3))
                    {
                        _synchronisedData[contextFullName].TimeMachine = new Vector3TimeMachine(timeMachineHistory.Value);
                    }
                    if (typeof(ITransform).IsAssignableFrom(typeof(T)))
                    {
                        _synchronisedData[contextFullName].TimeMachine =
                            new TransformTimeMachine(timeMachineHistory.Value);
                    }
                }
            }
        }
        
        private class SynchronisedData
        {
            public string Name;

            public int FrameInterval;

            public object LastValue;

            public object LastValueFromServer;

            public object CurrentValue;

            public Dictionary<IPEndPoint, int> LastFrameSynced;

            public Dictionary<IPEndPoint, bool> HasPerformedInitialSync;

            public Action<object> SetValueDelegate;

            public bool IsActiveInSynchronisation;

            public Dictionary<IPEndPoint, bool> HasReceivedInitialSync;

            public ITimeMachine TimeMachine;

            public SynchroniseTargets SynchronisationTargets;
        }

        #region Synchronisation Preperation
        
        private void PrepareAndTransmitSynchronisation(ComponentizedEntity entity, int tick, bool isFromClient, ClientAuthoritiveMode clientAuthoritiveMode)
        {
            if (!_uniqueIdentifierForEntity.HasValue)
            {
                throw new InvalidOperationException("PrepareAndTransmit should not be called without an entity ID!");
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

            // Sync properties to each client.
            foreach (var dispatcher in _networkEngine.CurrentDispatchers)
            {
                // TODO: Tracking clients by endpoint almost certainly needs to change...
                foreach (var endpoint in dispatcher.Endpoints)
                {
                    if (ClientAuthoritiveMode != ClientAuthoritiveMode.None &&
                        ClientOwnership != null &&
                        OnlySendToAuthoritiveClient)
                    {
                        if (!ClientOwnership.Equals(endpoint))
                        {
                            // This client doesn't own the entity, and this entity is only
                            // synchronised with clients that own it.
                            continue;
                        }
                    }

                    if (isFromClient || _clientsEntityIsKnownOn.Contains(endpoint))
                    {
                        if (_synchronisedData.Count > 0)
                        {
                            // Now calculate the delta to transmit over the network.
                            var currentTick = tick; // TODO: Use TimeTick
                            _synchronisedDataToTransmit.Clear();
                            foreach (var data in _synchronisedData.Values)
                            {
                                var needsSync = false;

                                // Check the target for this data to see whether or not we send it to
                                // this particular client.
                                if (data.SynchronisationTargets == SynchroniseTargets.OwningClient &&
                                    (ClientOwnership == null ||
                                     !ClientOwnership.Equals(endpoint)))
                                {
                                    // This data should only be synchronised to the owning client, and
                                    // we are not the owning client.
                                    continue;
                                }
                                else if (data.SynchronisationTargets == SynchroniseTargets.NonOwningClients &&
                                    (ClientOwnership == null ||
                                     ClientOwnership.Equals(endpoint)))
                                {
                                    // This data should only be synchronised to non-owning clients, and
                                    // we either are the owning client, or no client ownership has been set.
                                    continue;
                                }

                                // If we're on the client and we haven't had an initial piece of data from the server,
                                // we never synchronise because we don't know what the initial value is.
                                if (isFromClient && !data.HasReceivedInitialSync.GetOrDefault(endpoint, false))
                                {
                                    continue;
                                }

                                // If we haven't performed the initial synchronisation, we always transmit the data.
                                if (!data.HasPerformedInitialSync.GetOrDefault(endpoint, false))
                                {
                                    _networkEngine.LogSynchronisationEvent(
                                        "Must send property '" + data.Name + "' on entity ID " + _uniqueIdentifierForEntity + 
                                        " because the endpoint " + endpoint + " has not received it's initial sync.");
                                    needsSync = true;
                                }

                                // If we are on the client (i.e. the client assumes it's authoritive), or if the 
                                // server knows that the client does not have authority, then allow this next section.
                                // Or to put it another way, if we're not on the client and we know the client has
                                // authority, only transmit data for the first time because the client will make 
                                // decisions from that point onwards.
                                if (isFromClient || clientAuthoritiveMode != ClientAuthoritiveMode.TrustClient ||
                                    (clientAuthoritiveMode == ClientAuthoritiveMode.TrustClient &&
                                    !endpoint.Equals(ClientOwnership)))
                                {
                                    var lastValue = data.LastValue;
                                    var currentValue = data.CurrentValue;

                                    if (lastValue is ITransform)
                                    {
                                        throw new InvalidOperationException(
                                            "Last value property got stored as ITransform, but should have been stored as NetworkTransform!");
                                    }

                                    if (currentValue is ITransform)
                                    {
                                        currentValue = ((ITransform) currentValue).SerializeToNetwork();
                                    }

                                    if (!Equals(lastValue, currentValue))
                                    {
                                        if (data.LastFrameSynced.GetOrDefault(endpoint, 0) + data.FrameInterval < currentTick)
                                        {
                                            _networkEngine.LogSynchronisationEvent(
                                                "Sending property '" + data.Name + "' on entity ID " + _uniqueIdentifierForEntity +
                                                " because the value has changed (old value: " + lastValue + ", new value: " + currentValue + ")," +
                                                " and the next frame synced target for endpoint " + endpoint + "" +
                                                " is " + (data.LastFrameSynced.GetOrDefault(endpoint, 0) + data.FrameInterval) + "" +
                                                " and the current tick is " + currentTick + ".");
                                            needsSync = true;
                                        }
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
                                message.IsClientMessage = isFromClient;

                                bool reliable;
                                AssignSyncDataToMessage(_synchronisedDataToTransmit, message, currentTick, endpoint, out reliable);

                                // Send an entity properties message to the client.
                                _networkEngine.Send(
                                    dispatcher,
                                    endpoint,
                                    message,
                                    reliable);
                            }
                        }
                    }
                }
            }
        }

        #endregion

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

        #endregion

        public void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext)
        {
            if (!_enabled || _serverOnly)
            {
                return;
            }

            if (renderContext.IsCurrentRenderPass<IDebugRenderPass>())
            {
                var debugRenderPass = renderContext.GetCurrentRenderPass<IDebugRenderPass>();
                var entityTransformSync = _synchronisedData.Select(x => x.Value).FirstOrDefault(x => x.Name == "entity.transform");

                if (entityTransformSync != null && debugRenderPass.EnabledLayers.OfType<ServerStateDebugLayer>().Any())
                {
                    var lastValueSerialized = entityTransformSync.LastValueFromServer as NetworkTransform;
                    if (lastValueSerialized != null)
                    {
                        var timeMachine = entityTransformSync.TimeMachine;
                        if (timeMachine != null)
                        {
                            var lastValueRelative = lastValueSerialized.DeserializeFromNetwork();
                            var lastValueAbsolute = DefaultFinalTransform.Create(entity.FinalTransform.ParentObject,
                                new TransformContainer(lastValueRelative));

                            var clientLocalTickValueRelative = timeMachine.Get(_localTick) as ITransform;
                            var clientLocalTickValueAbsolute = DefaultFinalTransform.Create(entity.FinalTransform.ParentObject,
                                new TransformContainer(clientLocalTickValueRelative));

                            var clientRewindValueRelative = timeMachine.Get(_localTick - _networkEngine.ClientRenderDelayTicks) as ITransform;
                            var clientRewindValueAbsolute = DefaultFinalTransform.Create(entity.FinalTransform.ParentObject,
                                new TransformContainer(clientRewindValueRelative));

                            var lastValuePoint = Vector3.Transform(Vector3.Zero, lastValueAbsolute.AbsoluteMatrix);
                            var lastValueUp = Vector3.Transform(Vector3.Up, lastValueAbsolute.AbsoluteMatrix);
                            var lastValueForward = Vector3.Transform(Vector3.Forward, lastValueAbsolute.AbsoluteMatrix);
                            var lastValueLeft = Vector3.Transform(Vector3.Left, lastValueAbsolute.AbsoluteMatrix);

                            var clientLocalTickPoint = Vector3.Transform(Vector3.Zero, clientLocalTickValueAbsolute.AbsoluteMatrix);
                            var clientLocalTickUp = Vector3.Transform(Vector3.Up, clientLocalTickValueAbsolute.AbsoluteMatrix);
                            var clientLocalTickForward = Vector3.Transform(Vector3.Forward, clientLocalTickValueAbsolute.AbsoluteMatrix);
                            var clientLocalTickLeft = Vector3.Transform(Vector3.Left, clientLocalTickValueAbsolute.AbsoluteMatrix);

                            var clientRewindValueTickPoint = Vector3.Transform(Vector3.Zero, clientRewindValueAbsolute.AbsoluteMatrix);
                            var clientRewindValueTickUp = Vector3.Transform(Vector3.Up, clientRewindValueAbsolute.AbsoluteMatrix);
                            var clientRewindValueTickForward = Vector3.Transform(Vector3.Forward, clientRewindValueAbsolute.AbsoluteMatrix);
                            var clientRewindValueTickLeft = Vector3.Transform(Vector3.Left, clientRewindValueAbsolute.AbsoluteMatrix);

                            if (entity.GetType().Name == "CubeEntity")
                            {
                                Console.WriteLine(lastValueSerialized);
                            }

                            // Render the previous and next server states.
                            _debugRenderer.RenderDebugLine(renderContext, lastValuePoint, lastValueUp, Color.Red, Color.Red);
                            _debugRenderer.RenderDebugLine(renderContext, lastValuePoint, lastValueForward, Color.Red, Color.Red);
                            _debugRenderer.RenderDebugLine(renderContext, lastValuePoint, lastValueLeft, Color.Red, Color.Red);
                            _debugRenderer.RenderDebugLine(renderContext, clientLocalTickPoint, clientLocalTickUp, Color.Orange, Color.Orange);
                            _debugRenderer.RenderDebugLine(renderContext, clientLocalTickPoint, clientLocalTickForward, Color.Orange, Color.Orange);
                            _debugRenderer.RenderDebugLine(renderContext, clientLocalTickPoint, clientLocalTickLeft, Color.Orange, Color.Orange);
                            _debugRenderer.RenderDebugLine(renderContext, clientRewindValueTickPoint, clientRewindValueTickUp, Color.Yellow, Color.Yellow);
                            _debugRenderer.RenderDebugLine(renderContext, clientRewindValueTickPoint, clientRewindValueTickForward, Color.Yellow, Color.Yellow);
                            _debugRenderer.RenderDebugLine(renderContext, clientRewindValueTickPoint, clientRewindValueTickLeft, Color.Yellow, Color.Yellow);
                        }
                    }
                }
            }
        }
    }
}
