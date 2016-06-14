using System.Collections.Generic;
using Microsoft.Xna.Framework;
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
    public class ComponentizedEntity : ComponentizedObject, IEventListener<IGameContext>, IHasLights, IEntity
    {
        /// <summary>
        /// The component callable for handling updatable components.
        /// </summary>
        private IComponentCallable<IGameContext, IUpdateContext> _update;

        /// <summary>
        /// The component callable for handling renderable components.
        /// </summary>
        private IComponentCallable<IGameContext, IRenderContext> _render;

        /// <summary>
        /// The component callable for handling event handling components.
        /// </summary>
        private IComponentCallable<IGameContext, IEventEngine<IGameContext>, Event, EventState> _handleEvent;

        /// <summary>
        /// The component callable for retrieving lights defined by components.
        /// </summary>
        private IComponentCallable<List<ILight>> _getLights;

        /// <summary>
        /// Initializes a new <see cref="ComponentizedEntity"/>.
        /// <para>
        /// Componentized entities are entities which automatically support the attachment
        /// of updatable and renderable components (<see cref="IUpdatableComponent"/> and
        /// <see cref="IRenderableComponent"/> respectively).
        /// </para>
        /// </summary>
        public ComponentizedEntity()
        {
            Transform = new DefaultTransform();

            _update = RegisterCallable<IUpdatableComponent, IGameContext, IUpdateContext>((t, g, u) => t.Update(this, g, u));
            _render = RegisterCallable<IRenderableComponent, IGameContext, IRenderContext>((t, g, r) => t.Render(this, g, r));
            _handleEvent = RegisterCallable<IEventfulComponent, IGameContext, IEventEngine<IGameContext>, Event, EventState>(EventCallback);
            _getLights = RegisterCallable<ILightableComponent, List<ILight>>(GetLightCallback);

            RegisterPrivateComponent(this); // Register this as IHasMatrix.
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
        /// Updates the entity.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="updateContext">The current update context.</param>
        public virtual void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            _update.Invoke(gameContext, updateContext);
        }

        /// <summary>
        /// The local transform.
        /// </summary>
        public ITransform Transform { get; set; }

        public IFinalTransform FinalTransform
        {
            get { return this.GetDetachedFinalTransformImplementation(); }
        }

        /// <summary>
        /// Handles events from an event engine.  This implementation propagates events through the
        /// component hierarchy to components that implement <see cref="IEventfulComponent"/>.
        /// </summary>
        /// <param name="context">The current game context.</param>
        /// <param name="eventEngine">The event engine from which the event was fired.</param>
        /// <param name="event">The event that is to be handled.</param>
        /// <returns>Whether or not the event was consumed.</returns>
        public bool Handle(IGameContext context, IEventEngine<IGameContext> eventEngine, Event @event)
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
    }
}
