﻿using System;
using System.Collections.Generic;
using System.Linq;
using Jitter.Dynamics;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The base class for entities which support having components.
    /// <para>
    /// Componentized entities are entities which automatically support the attachment
    /// of updatable, renderable and eventful components (<see cref="IUpdatableComponent"/>,
    /// <see cref="IRenderableComponent"/> and <see cref="IEventfulComponent"/> respectively).
    /// </para>
    /// <para>
    /// By allowing the registration of components against entities, you can re-use
    /// additional behaviour and presentation on entities without deriving new classes,
    /// or implementing full dependency injected services (with appropriate bindings).
    /// You can register components on a componetized entity by calling
    /// <see cref="ComponentizedObject.RegisterComponent"/> on this object.
    /// </para>
    /// <para>
    /// You can also support additional component types and callbacks through the use
    /// of <see cref="ComponentizedObject.RegisterCallable{T}"/>; refer to the
    /// <see cref="ComponentizedObject"/> documentation for more information on how to
    /// register additional callable component methods.
    /// </para>
    /// </summary>
    /// <module>Component</module>
    [InjectFieldsForBaseObjectInProtectedConstructor]
    public class ComponentizedEntity : 
        ComponentizedObject, 
        IEventListener<IGameContext>, 
        IEventListener<INetworkEventContext>, 
        IEventListener<IPhysicsEventContext>,
        IEventListener<IPerPixelCollisionEventContext>,
        IHasLights, 
        IEntity, 
        IServerEntity, 
        INetworkIdentifiable, 
        ISynchronisedObject, 
        IPrerenderableEntity
    {
        /// <summary>
        /// The dependency injection node, which is automatically set by the kernel
        /// due to the presence of [InjectFieldsForBaseObjectInProtectedConstructor].
        /// </summary>
        private readonly INode _node;

        /// <summary>
        /// The dependency injection hierarchy, which is automatically set by the kernel
        /// due to the presence of [InjectFieldsForBaseObjectInProtectedConstructor].
        /// </summary>
        private readonly IHierarchy _hierarchy;

        /// <summary>
        /// The component callable for handling event handling components.
        /// </summary>
        private readonly IComponentCallable<IGameContext, IEventEngine<IGameContext>, Event, EventState> _handleEvent;

        /// <summary>
        /// The component callable for handling network messages recieved on a client.
        /// </summary>
        private readonly IComponentCallable<IGameContext, IUpdateContext, MxDispatcher, MxClient, byte[], uint, EventState> _handleMessageRecievedClient;

        /// <summary>
        /// The component callable for handling network messages recieved on a server.
        /// </summary>
        private readonly IComponentCallable<IServerContext, IUpdateContext, MxDispatcher, MxClient, byte[], uint, EventState> _handleMessageRecievedServer;

        /// <summary>
        /// The component callable for handling the creation of an entity from the server.
        /// </summary>
        private readonly IComponentCallable<IGameContext, IUpdateContext, int, int> _networkIdentifiableClient;

        /// <summary>
        /// The component callable for handling the creation of a predicted entity from the client.
        /// </summary>
        private readonly IComponentCallable<IServerContext, IUpdateContext, MxClient, int> _networkIdentifiableServer;

        /// <summary>
        /// The component callable for retrieving lights defined by components.
        /// </summary>
        private readonly IComponentCallable<List<ILight>> _getLights;

        /// <summary>
        /// This interface gets get called very frequently, so we optimize their invocation by
        /// iterating over them and calling them directly rather than using the <c>RegisterCallable</c>
        /// infrastructure.
        /// </summary>
        private IUpdatableComponent[] _updatableComponents = new IUpdatableComponent[0];

        /// <summary>
        /// This interface gets called very frequently, so we optimize their invocation by
        /// iterating over them and calling them directly rather than using the <c>RegisterCallable</c>
        /// infrastructure.
        /// </summary>
        private IServerUpdatableComponent[] _serverUpdatableComponents = new IServerUpdatableComponent[0];

        /// <summary>
        /// This interface gets get called very frequently, so we optimize their invocation by
        /// iterating over them and calling them directly rather than using the <c>RegisterCallable</c>
        /// infrastructure.
        /// </summary>
        private IRenderableComponent[] _renderableComponents = new IRenderableComponent[0];

        /// <summary>
        /// This interface gets get called very frequently, so we optimize their invocation by
        /// iterating over them and calling them directly rather than using the <c>RegisterCallable</c>
        /// infrastructure.
        /// </summary>
        private IPrerenderableComponent[] _prerenderableComponents = new IPrerenderableComponent[0];

        /// <summary>
        /// This interface gets get called very frequently, so we optimize their invocation by
        /// iterating over them and calling them directly rather than using the <c>RegisterCallable</c>
        /// infrastructure.
        /// </summary>
        private ICollidableComponent[] _collidableComponents = new ICollidableComponent[0];

        /// <summary>
        /// This interface gets get called very frequently, so we optimize their invocation by
        /// iterating over them and calling them directly rather than using the <c>RegisterCallable</c>
        /// infrastructure.
        /// </summary>
        private IPerPixelCollidableComponent[] _perPixelCollidableComponents = new IPerPixelCollidableComponent[0];

        private bool _hasRenderableComponentDescendants;

        private bool _hasPrerenderableComponentDescendants;

        private bool _hasUpdatableComponentDescendants;

        private bool _hasServerUpdatableComponentDescendants;

        private bool _hasLightableComponentDescendants;

        private bool _hasCollidableComponentDescendants;

        private bool _hasPerPixelCollidableComponentDescendants;

        /// <summary>
        /// Initializes a new <see cref="ComponentizedEntity"/>.
        /// <para>
        /// Componentized entities are entities which automatically support the attachment
        /// of updatable and renderable components (<see cref="IUpdatableComponent"/> and
        /// <see cref="IRenderableComponent"/> respectively).
        /// </para>
        /// </summary>
        protected ComponentizedEntity()
        {
            Transform = new DefaultTransform();
            
            _handleEvent = RegisterCallable<IEventfulComponent, IGameContext, IEventEngine<IGameContext>, Event, EventState>(EventCallback);
            _handleMessageRecievedClient = RegisterCallable<INetworkedComponent, IGameContext, IUpdateContext, MxDispatcher, MxClient, byte[], uint, EventState>(ClientMessageCallback);
            _handleMessageRecievedServer = RegisterCallable<INetworkedComponent, IServerContext, IUpdateContext, MxDispatcher, MxClient, byte[], uint, EventState>(ServerMessageCallback);
            _networkIdentifiableClient = RegisterCallable<INetworkIdentifiable, IGameContext, IUpdateContext, int, int>((c, g, u, i, ft) => c.ReceiveNetworkIDFromServer(g, u, i, ft));
            _networkIdentifiableServer = RegisterCallable<INetworkIdentifiable, IServerContext, IUpdateContext, MxClient, int>((c, s, u, mc, i) => c.ReceivePredictedNetworkIDFromClient(s, u, mc, i));
            _getLights = RegisterCallable<ILightableComponent, List<ILight>>(GetLightCallback);

            OnComponentsChanged();
        }

        protected override void OnComponentsChanged()
        {
            _updatableComponents = Components.OfType<IUpdatableComponent>().ToArray();
            _serverUpdatableComponents = Components.OfType<IServerUpdatableComponent>().ToArray();
            _renderableComponents = Components.OfType<IRenderableComponent>().ToArray();
            _prerenderableComponents = Components.OfType<IPrerenderableComponent>().ToArray();
            _collidableComponents = Components.OfType<ICollidableComponent>().ToArray();
            _perPixelCollidableComponents = Components.OfType<IPerPixelCollidableComponent>().ToArray();
        }
        
        /// <summary>
        /// Renders the entity.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="renderContext">The current render context.</param>
        public virtual void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (_hasRenderableComponentDescendants)
            {
                for (var i = 0; i < _renderableComponents.Length; i++)
                {
                    _renderableComponents[i].Render(this, gameContext, renderContext);
                }
            }
        }

        /// <summary>
        /// Prerenders the entity.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="renderContext">The current render context.</param>
        public virtual void Prerender(IGameContext gameContext, IRenderContext renderContext)
        {
            if (_hasPrerenderableComponentDescendants)
            {
                for (var i = 0; i < _prerenderableComponents.Length; i++)
                {
                    _prerenderableComponents[i].Prerender(this, gameContext, renderContext);
                }
            }
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="updateContext">The current update context.</param>
        public virtual void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (_hasUpdatableComponentDescendants)
            {
                for (var i = 0; i < _updatableComponents.Length; i++)
                {
                    _updatableComponents[i].Update(this, gameContext, updateContext);
                }
            }
        }

        /// <summary>
        /// Updates the entity on the server.
        /// </summary>
        /// <param name="serverContext">The current server context.</param>
        /// <param name="updateContext">The current update context.</param>
        public virtual void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            if (_hasServerUpdatableComponentDescendants)
            {
                for (var i = 0; i < _serverUpdatableComponents.Length; i++)
                {
                    _serverUpdatableComponents[i].Update(this, serverContext, updateContext);
                }
            }
        }

        /// <summary>
        /// The local transform.
        /// </summary>
        public ITransform Transform { get; }

        public IFinalTransform FinalTransform
        {
            get
            {
                if (_node == null || _hierarchy == null)
                {
                    throw new InvalidOperationException("Componentized entities must be created through the dependency injection kernel!");
                }

                return this.GetAttachedFinalTransformImplementation(_node);
            }
        }

        /// <summary>
        /// Handles events from an event engine.  This implementation propagates events through the
        /// component hierarchy to components that implement <see cref="IEventfulComponent"/>.
        /// </summary>
        /// <param name="context">The current game context.</param>
        /// <param name="eventEngine">The event engine from which the event was fired.</param>
        /// <param name="event">The event that is to be handled.</param>
        /// <returns>Whether or not the event was consumed.</returns>
        public virtual bool Handle(IGameContext context, IEventEngine<IGameContext> eventEngine, Event @event)
        {
            if (EnabledInterfaces.Contains(typeof(IEventfulComponent)))
            {
                var state = new EventState
                {
                    Consumed = false
                };
                _handleEvent.Invoke(context, eventEngine, @event, state);
                return state.Consumed;
            }

            return false;
        }

        /// <summary>
        /// Internally handles propagating events to <see cref="IEventfulComponent"/>.  We need to structure
        /// it like this because if a component consumes an event, no other component should then be able to
        /// handle it.
        /// </summary>
        /// <param name="component">The component being processed.</param>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="eventEngine">The event engine from which the event was fired.</param>
        /// <param name="event">The event that is to be handled.</param>
        /// <param name="eventState">Whether the event has been consumed yet.</param>
        private void EventCallback(IEventfulComponent component, IGameContext gameContext, IEventEngine<IGameContext> eventEngine, Event @event, EventState eventState)
        {
            if (!eventState.Consumed)
            {
                eventState.Consumed = component.Handle(this, gameContext, eventEngine, @event);
            }
        }

        /// <summary>
        /// Tracks whether the event was consumed by components.
        /// </summary>
        private class EventState
        {
            /// <summary>
            /// Whether the event was consumed.
            /// </summary>
            public bool Consumed { get; set; }
        }

        private static ILight[] _emptyLights = new ILight[0];

        /// <summary>
        /// Returns lights defined on this entity and it's components.
        /// </summary>
        /// <returns>A list of lights to be used in rendering.</returns>
        public virtual IEnumerable<ILight> GetLights()
        {
            if (_hasLightableComponentDescendants)
            {
                var list = new List<ILight>();
                _getLights.Invoke(list);
                return list;
            }

            return _emptyLights;
        }

        /// <summary>
        /// Internally handles adding lights defined by a component to the list of lights
        /// to return as part of <see cref="GetLights"/>.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="lightList"></param>
        private void GetLightCallback(ILightableComponent component, List<ILight> lightList)
        {
            lightList.AddRange(component.GetLights());
        }
        
        /// <summary>
        /// Handles physics events from an event engine.  This implementation propagates events through the
        /// component hierarchy to components that implement <see cref="ICollidableComponent"/>.
        /// </summary>
        /// <param name="context">The current game context.</param>
        /// <param name="eventEngine">The event engine from which the event was fired.</param>
        /// <param name="event">The physics event that is to be handled.</param>
        /// <returns>Whether or not the event was consumed.</returns>
        public virtual bool Handle(IPhysicsEventContext context, IEventEngine<IPhysicsEventContext> eventEngine, Event @event)
        {
            if (_hasCollidableComponentDescendants)
            {
                var physicsCollisionBeginEvent = @event as PhysicsCollisionBeginEvent;
                var physicsCollisionEndEvent = @event as PhysicsCollisionEndEvent;
                if (physicsCollisionBeginEvent == null && physicsCollisionEndEvent == null)
                {
                    return false;
                }

                if (physicsCollisionBeginEvent != null)
                {
                    CollisionBegin(
                        physicsCollisionBeginEvent.GameContext,
                        physicsCollisionBeginEvent.ServerContext,
                        physicsCollisionBeginEvent.UpdateContext,
                        physicsCollisionBeginEvent.Owner1,
                        physicsCollisionBeginEvent.Owner2,
                        physicsCollisionBeginEvent.Body1,
                        physicsCollisionBeginEvent.Body2);
                }

                if (physicsCollisionEndEvent != null)
                {
                    CollisionEnd(
                        physicsCollisionEndEvent.GameContext,
                        physicsCollisionEndEvent.ServerContext,
                        physicsCollisionEndEvent.UpdateContext,
                        physicsCollisionEndEvent.Owner1,
                        physicsCollisionEndEvent.Owner2,
                        physicsCollisionEndEvent.Body1,
                        physicsCollisionEndEvent.Body2);
                }
            }

            return false;
        }

        /// <summary>
        /// This method is called by the physics system when a collision involving one of this component's
        /// parents and another object with a rigid body starts occurring.
        /// </summary>
        /// <param name="gameContext">The current game context, or null if running on a server.</param>
        /// <param name="serverContext">The current server context, or null if running on a client.</param>
        /// <param name="updateContext">The current update context.</param>
        /// <param name="owner1">The owner of the first rigid body.  This is NOT necessarily one of the component's parents.</param>
        /// <param name="owner2">The owner of the second rigid body.  This is NOT necessarily one of the component's parents.</param>
        /// <param name="body1">The rigid body of the first object.</param>
        /// <param name="body2">The rigid body of the second object.</param>
        public virtual void CollisionBegin(IGameContext gameContext, IServerContext serverContext,
            IUpdateContext updateContext, object owner1, object owner2, RigidBody body1, RigidBody body2)
        {
            for (var i = 0; i < _collidableComponents.Length; i++)
            {
                _collidableComponents[i].CollisionBegin(
                    gameContext,
                    serverContext,
                    updateContext,
                    owner1,
                    owner2,
                    body1,
                    body2);
            }
        }

        /// <summary>
        /// This method is called by the physics system when a collision involving one of this component's
        /// parents and another object with a rigid body finishes (i.e. the rigid bodies have seperated from their collision).
        /// </summary>
        /// <param name="gameContext">The current game context, or null if running on a server.</param>
        /// <param name="serverContext">The current server context, or null if running on a client.</param>
        /// <param name="updateContext">The current update context.</param>
        /// <param name="owner1">The owner of the first rigid body.  This is NOT necessarily one of the component's parents.</param>
        /// <param name="owner2">The owner of the second rigid body.  This is NOT necessarily one of the component's parents.</param>
        /// <param name="body1">The rigid body of the first object.</param>
        /// <param name="body2">The rigid body of the second object.</param>
        public virtual void CollisionEnd(IGameContext gameContext, IServerContext serverContext,
            IUpdateContext updateContext, object owner1, object owner2, RigidBody body1, RigidBody body2)
        {
            for (var i = 0; i < _collidableComponents.Length; i++)
            {
                _collidableComponents[i].CollisionEnd(
                    gameContext,
                    serverContext,
                    updateContext,
                    owner1,
                    owner2,
                    body1,
                    body2);
            }
        }

        /// <summary>
        /// Handles per-pixel collision events from the per-pixel collision engine.
        /// </summary>
        /// <param name="context">The current game context.</param>
        /// <param name="eventEngine">The event engine from which the event was fired.</param>
        /// <param name="event">The per-pixel collision event that is to be handled.</param>
        /// <returns>Whether or not the event was consumed.</returns>
        public virtual bool Handle(IPerPixelCollisionEventContext context, IEventEngine<IPerPixelCollisionEventContext> eventEngine, Event @event)
        {
            if (_hasPerPixelCollidableComponentDescendants)
            {
                var perPixelCollisionEvent = @event as PerPixelCollisionEvent;
                if (perPixelCollisionEvent != null)
                {
                    PerPixelCollision(
                        perPixelCollisionEvent.GameContext,
                        perPixelCollisionEvent.ServerContext,
                        perPixelCollisionEvent.UpdateContext,
                        perPixelCollisionEvent.Object1,
                        perPixelCollisionEvent.Object2);
                }
            }

            return false;
        }

        /// <summary>
        /// This method is called by the per-pixel collision system when a collision involving one of this component's
        /// parents and another object with a per-pixel collision component starts occurring.
        /// </summary>
        /// <param name="gameContext">The current game context, or null if running on a server.</param>
        /// <param name="serverContext">The current server context, or null if running on a client.</param>
        /// <param name="updateContext">The current update context.</param>
        /// <param name="obj1">The owner of the first per-pixel collision component.  This is NOT necessarily one of the component's parents.</param>
        /// <param name="obj2">The owner of the second per-pixel collision component.  This is NOT necessarily one of the component's parents.</param>
        public virtual void PerPixelCollision(IGameContext gameContext, IServerContext serverContext,
            IUpdateContext updateContext, object obj1, object obj2)
        {
            for (var i = 0; i < _perPixelCollidableComponents.Length; i++)
            {
                _perPixelCollidableComponents[i].PerPixelCollision(
                    gameContext,
                    serverContext,
                    updateContext,
                    obj1,
                    obj2);
            }
        }

        /// <summary>
        /// Handles network events from an event engine.  This implementation propagates events through the
        /// component hierarchy to components that implement <see cref="INetworkedComponent"/>.
        /// </summary>
        /// <param name="context">The current game context.</param>
        /// <param name="eventEngine">The event engine from which the event was fired.</param>
        /// <param name="event">The network event that is to be handled.</param>
        /// <returns>Whether or not the event was consumed.</returns>
        public virtual bool Handle(INetworkEventContext context, IEventEngine<INetworkEventContext> eventEngine, Event @event)
        {
            var networkEvent = @event as NetworkMessageReceivedEvent;
            if (networkEvent == null)
            {
                return false;
            }
            
            var state = new EventState
            {
                Consumed = false
            };

            if (networkEvent.GameContext != null)
            {
                _handleMessageRecievedClient.Invoke(
                    networkEvent.GameContext,
                    networkEvent.UpdateContext,
                    networkEvent.Dispatcher,
                    networkEvent.Client,
                    networkEvent.Payload,
                    networkEvent.ProtocolID,
                    state);
            }

            if (networkEvent.ServerContext != null && !state.Consumed)
            {
                _handleMessageRecievedServer.Invoke(
                    networkEvent.ServerContext,
                    networkEvent.UpdateContext,
                    networkEvent.Dispatcher,
                    networkEvent.Client,
                    networkEvent.Payload,
                    networkEvent.ProtocolID,
                    state);
            }

            return state.Consumed;
        }

        /// <summary>
        /// Internally handles propagating network messages to the networked components on the server.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="serverContext"></param>
        /// <param name="updateContext"></param>
        /// <param name="dispatcher"></param>
        /// <param name="client"></param>
        /// <param name="payload"></param>
        /// <param name="protocolId"></param>
        /// <param name="eventState"></param>
        private void ServerMessageCallback(
            INetworkedComponent component,
            IServerContext serverContext,
            IUpdateContext updateContext, 
            MxDispatcher dispatcher,
            MxClient client,
            byte[] payload,
            uint protocolId,
            EventState eventState)
        {
            if (!eventState.Consumed)
            {
                eventState.Consumed = component.ReceiveMessage(this, serverContext,
                    updateContext,
                    dispatcher,
                    client,
                    payload,
                    protocolId);
            }
        }

        /// <summary>
        /// Internally handles propagating network messages to the networked components on the client.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="gameContext"></param>
        /// <param name="updateContext"></param>
        /// <param name="dispatcher"></param>
        /// <param name="client"></param>
        /// <param name="payload"></param>
        /// <param name="protocolId"></param>
        /// <param name="eventState"></param>
        private void ClientMessageCallback(
            INetworkedComponent component,
            IGameContext gameContext,
            IUpdateContext updateContext,
            MxDispatcher dispatcher,
            MxClient client,
            byte[] payload,
            uint protocolId,
            EventState eventState)
        {
            if (!eventState.Consumed)
            {
                eventState.Consumed = component.ReceiveMessage(this, gameContext,
                    updateContext,
                    dispatcher,
                    client,
                    payload,
                    protocolId);
            }
        }

        /// <summary>
        /// Called by the network engine when this entity is spawned with an <see cref="EntityCreateMessage"/>.
        /// </summary>
        /// <param name="gameContext"></param>
        /// <param name="updateContext"></param>
        /// <param name="identifier"></param>
        /// <param name="initialFrameTick"></param>
        public virtual void ReceiveNetworkIDFromServer(IGameContext gameContext, IUpdateContext updateContext, int identifier, int initialFrameTick)
        {
            if (EnabledInterfaces.Contains(typeof(INetworkIdentifiable)))
            {
                _networkIdentifiableClient.Invoke(gameContext, updateContext, identifier, initialFrameTick);
            }
        }

        /// <summary>
        /// Called by the network engine when this entity is spawned with an <see cref="EntityPredictMessage"/>.
        /// </summary>
        /// <param name="serverContext"></param>
        /// <param name="updateContext"></param>
        /// <param name="client"></param>
        /// <param name="predictedIdentifier"></param>
        public virtual void ReceivePredictedNetworkIDFromClient(IServerContext serverContext, IUpdateContext updateContext, MxClient client,
            int predictedIdentifier)
        {
            if (EnabledInterfaces.Contains(typeof(INetworkIdentifiable)))
            {
                _networkIdentifiableServer.Invoke(serverContext, updateContext, client, predictedIdentifier);
            }
        }

        /// <summary>
        /// Declares the synchronised properties for network synchronised objects.  By default this synchronises
        /// the local transform of the entity.
        /// </summary>
        /// <param name="synchronisationApi">The synchronisation API.</param>
        public virtual void DeclareSynchronisedProperties(ISynchronisationApi synchronisationApi)
        {
            synchronisationApi.Synchronise("transform", 5, Transform, x => Transform.Assign(x), 500);
        }

        #region Override Detection

        private bool _isOverriddenCacheSet;
        private bool _isUpdateOverridden;
        private bool _isServerUpdateOverridden;
        private bool _isRenderOverridden;
        private bool _isPrerenderOverridden;
        private bool _isEventHandleOverridden;
        private bool _isCollisionBeginOverridden;
        private bool _isCollisionEndOverridden;
        private bool _isReceiveNetworkIdFromServerOverridden;
        private bool _isReceivePredictedNetworkIdFromClientOverridden;
        private bool _isGetLightsOverridden;
        private bool _isPerPixelCollisionOverridden;

        protected override void AddAdditionalEnabledInterfaces(HashSet<Type> enabledInterfaces)
        {
            if (!_isOverriddenCacheSet)
            {
                _isUpdateOverridden = GetType().GetMethod("Update", new [] { typeof(IGameContext), typeof(IUpdateContext) }).DeclaringType != typeof(ComponentizedEntity);
                _isServerUpdateOverridden = GetType().GetMethod("Update", new[] { typeof(IServerContext), typeof(IUpdateContext) }).DeclaringType != typeof(ComponentizedEntity);
                _isRenderOverridden = GetType().GetMethod("Render").DeclaringType != typeof(ComponentizedEntity);
                _isPrerenderOverridden = GetType().GetMethod("Prerender").DeclaringType != typeof(ComponentizedEntity);
                _isEventHandleOverridden = GetType().GetMethod("Handle", new[] { typeof(IGameContext), typeof(IEventEngine<IGameContext>), typeof(Event) }).DeclaringType != typeof(ComponentizedEntity);
                _isCollisionBeginOverridden = GetType().GetMethod("CollisionBegin", new[]
                {
                    typeof(IGameContext),
                    typeof(IServerContext),
                    typeof(IUpdateContext),
                    typeof(object),
                    typeof(object),
                    typeof(RigidBody),
                    typeof(RigidBody), 
                }).DeclaringType != typeof(ComponentizedEntity);
                _isCollisionEndOverridden = GetType().GetMethod("CollisionEnd", new[]
                {
                    typeof(IGameContext),
                    typeof(IServerContext),
                    typeof(IUpdateContext),
                    typeof(object),
                    typeof(object),
                    typeof(RigidBody),
                    typeof(RigidBody),
                }).DeclaringType != typeof(ComponentizedEntity);
                _isReceiveNetworkIdFromServerOverridden = GetType().GetMethod("ReceiveNetworkIDFromServer").DeclaringType != typeof(ComponentizedEntity);
                _isReceivePredictedNetworkIdFromClientOverridden = GetType().GetMethod("ReceivePredictedNetworkIDFromClient").DeclaringType != typeof(ComponentizedEntity);
                _isGetLightsOverridden = GetType().GetMethod("GetLights").DeclaringType != typeof(ComponentizedEntity);
                _isPerPixelCollisionOverridden = GetType().GetMethod("PerPixelCollision", new[]
                {
                    typeof(IGameContext),
                    typeof(IServerContext),
                    typeof(IUpdateContext),
                    typeof(object),
                    typeof(object),
                }).DeclaringType != typeof(ComponentizedEntity);

                _isOverriddenCacheSet = true;
            }

            if (_isUpdateOverridden && !enabledInterfaces.Contains(typeof(IUpdatableComponent)))
            {
                enabledInterfaces.Add(typeof(IUpdatableComponent));
            }

            if (_isServerUpdateOverridden && !enabledInterfaces.Contains(typeof(IServerUpdatableComponent)))
            {
                enabledInterfaces.Add(typeof(IServerUpdatableComponent));
            }

            if (_isRenderOverridden && !enabledInterfaces.Contains(typeof(IRenderableComponent)))
            {
                enabledInterfaces.Add(typeof(IRenderableComponent));
            }

            if (_isPrerenderOverridden && !enabledInterfaces.Contains(typeof(IPrerenderableComponent)))
            {
                enabledInterfaces.Add(typeof(IPrerenderableComponent));
            }

            if ((_isCollisionBeginOverridden || _isCollisionEndOverridden) && !enabledInterfaces.Contains(typeof(ICollidableComponent)))
            {
                enabledInterfaces.Add(typeof(ICollidableComponent));
            }

            if (_isEventHandleOverridden && !enabledInterfaces.Contains(typeof(IEventListener<IGameContext>)))
            {
                enabledInterfaces.Add(typeof(IEventListener<IGameContext>));
            }

            if ((_isReceiveNetworkIdFromServerOverridden || _isReceivePredictedNetworkIdFromClientOverridden) && !enabledInterfaces.Contains(typeof(INetworkIdentifiable)))
            {
                enabledInterfaces.Add(typeof(INetworkIdentifiable));
            }

            if (_isGetLightsOverridden && !enabledInterfaces.Contains(typeof(ILightableComponent)))
            {
                enabledInterfaces.Add(typeof(ILightableComponent));
            }

            if (_isPerPixelCollisionOverridden && !enabledInterfaces.Contains(typeof(IPerPixelCollidableComponent)))
            {
                enabledInterfaces.Add(typeof(IPerPixelCollidableComponent));
            }

            _hasRenderableComponentDescendants = enabledInterfaces.Contains(typeof(IRenderableComponent));
            _hasPrerenderableComponentDescendants = enabledInterfaces.Contains(typeof(IPrerenderableComponent));
            _hasUpdatableComponentDescendants = enabledInterfaces.Contains(typeof(IUpdatableComponent));
            _hasServerUpdatableComponentDescendants = enabledInterfaces.Contains(typeof(IServerUpdatableComponent));
            _hasLightableComponentDescendants = enabledInterfaces.Contains(typeof(ILightableComponent));
            _hasCollidableComponentDescendants = enabledInterfaces.Contains(typeof(ICollidableComponent));
            _hasPerPixelCollidableComponentDescendants = enabledInterfaces.Contains(typeof(IPerPixelCollidableComponent));
        }

        #endregion
    }
}
