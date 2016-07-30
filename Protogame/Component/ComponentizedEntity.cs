using System;
using System.Collections.Generic;
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
    /// <see cref="ComponentizedObject.RegisterPrivateComponent"/> or
    /// <see cref="ComponentizedObject.RegisterPublicComponent"/> on this object.
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
    public class ComponentizedEntity : ComponentizedObject, IEventListener<IGameContext>, IEventListener<INetworkEventContext>, IHasLights, IEntity, IServerEntity, INetworkIdentifiable, ISynchronisedObject, IPrerenderableEntity
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
        /// The component callable for handling updatable components.
        /// </summary>
        private readonly IComponentCallable<IGameContext, IUpdateContext> _update;

        /// <summary>
        /// The component callable for handling server-side updatable components.
        /// </summary>
        private readonly IComponentCallable<IServerContext, IUpdateContext> _serverUpdate;

        /// <summary>
        /// The component callable for handling prerenderable components.
        /// </summary>
        private readonly IComponentCallable<IGameContext, IRenderContext> _prerender;

        /// <summary>
        /// The component callable for handling renderable components.
        /// </summary>
        private readonly IComponentCallable<IGameContext, IRenderContext> _render;

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

            _update = RegisterCallable<IUpdatableComponent, IGameContext, IUpdateContext>((t, g, u) => t.Update(this, g, u));
            _serverUpdate = RegisterCallable<IServerUpdatableComponent, IServerContext, IUpdateContext>((t, s, u) => t.Update(this, s, u));
            _prerender = RegisterCallable<IPrerenderableComponent, IGameContext, IRenderContext>((t, g, r) => t.Prerender(this, g, r));
            _render = RegisterCallable<IRenderableComponent, IGameContext, IRenderContext>((t, g, r) => t.Render(this, g, r));
            _handleEvent = RegisterCallable<IEventfulComponent, IGameContext, IEventEngine<IGameContext>, Event, EventState>(EventCallback);
            _handleMessageRecievedClient = RegisterCallable<INetworkedComponent, IGameContext, IUpdateContext, MxDispatcher, MxClient, byte[], uint, EventState>(ClientMessageCallback);
            _handleMessageRecievedServer = RegisterCallable<INetworkedComponent, IServerContext, IUpdateContext, MxDispatcher, MxClient, byte[], uint, EventState>(ServerMessageCallback);
            _networkIdentifiableClient = RegisterCallable<INetworkIdentifiable, IGameContext, IUpdateContext, int, int>((c, g, u, i, ft) => c.ReceiveNetworkIDFromServer(g, u, i, ft));
            _networkIdentifiableServer = RegisterCallable<INetworkIdentifiable, IServerContext, IUpdateContext, MxClient, int>((c, s, u, mc, i) => c.ReceivePredictedNetworkIDFromClient(s, u, mc, i));
            _getLights = RegisterCallable<ILightableComponent, List<ILight>>(GetLightCallback);
        }

        /// <summary>
        /// Renders the entity.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="renderContext">The current render context.</param>
        public virtual void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            _render.Invoke(gameContext, renderContext);
        }

        /// <summary>
        /// Prerenders the entity.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="renderContext">The current render context.</param>
        public void Prerender(IGameContext gameContext, IRenderContext renderContext)
        {
            _prerender.Invoke(gameContext, renderContext);
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="updateContext">The current update context.</param>
        public virtual void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            _update.Invoke(gameContext, updateContext);
        }

        /// <summary>
        /// Updates the entity on the server.
        /// </summary>
        /// <param name="serverContext">The current server context.</param>
        /// <param name="updateContext">The current update context.</param>
        public virtual void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            _serverUpdate.Invoke(serverContext, updateContext);
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
            var state = new EventState
            {
                Consumed = false
            };
            _handleEvent.Invoke(context, eventEngine, @event, state);
            return state.Consumed;
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

        /// <summary>
        /// Returns lights defined on this entity and it's components.
        /// </summary>
        /// <returns>A list of lights to be used in rendering.</returns>
        public virtual IEnumerable<ILight> GetLights()
        {
            var list = new List<ILight>();
            _getLights.Invoke(list);
            return list;
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
            _networkIdentifiableClient.Invoke(gameContext, updateContext, identifier, initialFrameTick);
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
            _networkIdentifiableServer.Invoke(serverContext, updateContext, client, predictedIdentifier);
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
    }
}
