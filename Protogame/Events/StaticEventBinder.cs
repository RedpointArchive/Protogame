namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Protoinject;

    /// <summary>
    /// The static event binder.
    /// </summary>
    /// <typeparam name="TContext">
    /// </typeparam>
    public abstract class StaticEventBinder<TContext> : IEventBinder<TContext>
    {
        /// <summary>
        /// The bindings that have been configured.
        /// </summary>
        private readonly List<Func<TContext, IEventEngine<TContext>, Event, bool>> _bindings;

        /// <summary>
        /// Whether or not configuration of this event binder has finished.
        /// </summary>
        private bool _configured;

        /// <summary>
        /// The dependency injection kernel.
        /// </summary>
        private IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticEventBinder{TContext}"/> class.
        /// </summary>
        protected StaticEventBinder()
        {
            this._bindings = new List<Func<TContext, IEventEngine<TContext>, Event, bool>>();
        }
        
        protected interface IBindable<TEvent>
            where TEvent : Event
        {
            IBindableOn<TEvent, TEntity> On<TEntity>() where TEntity : IEntity;
            
            IBindableOn<TEvent, TEntity> On<TEntity>(Func<TEntity, bool> filter) where TEntity : IEntity;
            
            IBindableOnTogglable<TEvent, TEntity> OnTogglable<TEntity>() where TEntity : IEntity, IEventTogglable;
            
            IBindableTo<TEvent, TAction> To<TAction>() where TAction : IEventAction<TContext>;
            
            IBindableTo<TEvent, TCommand> ToCommand<TCommand>(params string[] arguments) where TCommand : ICommand;
            
            IBindableTo<TEvent, TListener> ToListener<TListener>() where TListener : IEventListener<TContext>;

            /// <summary>
            /// Binds the event to a componentized entity in the world.
            /// </summary>
            /// <typeparam name="TComponentizedEntity">The componentized entity that should handle the event.</typeparam>
            /// <param name="onlyFirst">
            /// <c>true</c> if only the first componentized entity in the world that matches this type should have the ability to handle the event, or
            /// <c>false</c> if the event should be propagated to all componentized entities in the world matching the type until the event is consumed
            /// </param>
            IBindableTo<TEvent, TComponentizedEntity> ToComponentizedEntity<TComponentizedEntity>(bool onlyFirst) where TComponentizedEntity : ComponentizedEntity;

            void ToNothing();

            IBindableTo<TEvent, ComponentizedEntity> ToAllComponentizedEntities();
        }
        
        protected interface IBindableOn<TEvent, TEntity>
            where TEvent : Event where TEntity : IEntity
        {
            /// <summary>
            /// The to.
            /// </summary>
            /// <typeparam name="TEntityAction">
            /// </typeparam>
            /// <returns>
            /// The <see cref="StaticEventBinder"/>.
            /// </returns>
            IBindableOnTo<TEvent, TEntity, TEntityAction> To<TEntityAction>()
                where TEntityAction : IEventEntityAction<TEntity>;
        }

        protected interface IPropagate
        {
            /// <summary>
            /// Allow the event to propagate to other bindings even if
            /// handled by this bindings.
            /// </summary>
            /// <typeparam name="TEntityAction">
            /// </typeparam>
            /// <returns>
            /// The <see cref="StaticEventBinder"/>.
            /// </returns>
            void Propagate();
        }
        
        protected interface IBindableOnTo<TEvent, TEntity, TEntityAction> : IPropagate
            where TEvent : Event where TEntity : IEntity where TEntityAction : IEventEntityAction<TEntity>
        {
        }
        
        protected interface IBindableOnTo<TEvent, TEntity> : IPropagate
            where TEvent : Event where TEntity : IEntity, IEventTogglable
        {
        }
        
        protected interface IBindableOnTogglable<TEvent, TEntity> : IBindableOn<TEvent, TEntity>
            where TEvent : Event where TEntity : IEntity, IEventTogglable
        {
            /// <summary>
            /// The to toggle.
            /// </summary>
            /// <param name="id">
            /// The id.
            /// </param>
            /// <returns>
            /// The <see cref="StaticEventBinder"/>.
            /// </returns>
            IBindableOnTo<TEvent, TEntity> ToToggle(string id);
        }
        
        protected interface IBindableTo<TEvent, TTarget>
            where TEvent : Event
        {
        }
        
        public int Priority
        {
            get
            {
                return 100;
            }
        }
        
        public void Assign(IKernel kernel)
        {
            this._kernel = kernel;
        }
        
        public abstract void Configure();

        /// <summary>
        /// A global filter check that is applied before any events are handled.  If this
        /// returns false, then this event binder will not handle the event.
        /// </summary>
        /// <remarks>
        /// This is often used to filter static event binders based on the world.
        /// </remarks>
        /// <param name="context">The context of the event.</param>
        /// <param name="event">The event that was fired.</param>
        /// <returns>Whether or not this event binder should handle the event.</returns>
        protected virtual bool Filter(TContext context, Event @event)
        {
            return true;
        }
        
        public bool Handle(TContext context, IEventEngine<TContext> eventEngine, Event @event)
        {
            if (!this._configured)
            {
                this.Configure();
                this._configured = true;
            }

            if (!this.Filter(context, @event))
            {
                return false;
            }

            foreach (var binding in this._bindings)
            {
                if (binding(context, eventEngine, @event))
                {
                    return true;
                }
            }

            return false;
        }
        
        protected IBindable<TEvent> Bind<TEvent>(Func<TEvent, bool> filter) where TEvent : Event
        {
            return new DefaultBindable<TEvent>(_kernel, this, filter);
        }
        
        private class DefaultBindable<T> : IBindable<T>
            where T : Event
        {
            private readonly Func<T, bool> m_Filter;
            
            private readonly IKernel _kernel;

            private readonly StaticEventBinder<TContext> m_StaticEventBinder;
            
            public DefaultBindable(IKernel kernel, StaticEventBinder<TContext> staticEventBinder, Func<T, bool> filter)
            {
                _kernel = kernel;
                this.m_StaticEventBinder = staticEventBinder;
                this.m_Filter = filter;
            }
            
            public IBindableOn<T, TEntity> On<TEntity>() where TEntity : IEntity
            {
                return new DefaultBindableOn<T, TEntity>(this.m_StaticEventBinder, this.m_Filter, t => true);
            }
            
            public IBindableOn<T, TEntity> On<TEntity>(Func<TEntity, bool> filter) where TEntity : IEntity
            {
                return new DefaultBindableOn<T, TEntity>(this.m_StaticEventBinder, this.m_Filter, filter);
            }
            
            public IBindableOnTogglable<T, TEntity> OnTogglable<TEntity>() where TEntity : IEntity, IEventTogglable
            {
                return new DefaultBindableOnTogglable<T, TEntity>(this.m_StaticEventBinder, this.m_Filter);
            }
            
            public IBindableTo<T, TAction> To<TAction>() where TAction : IEventAction<TContext>
            {
                var bindable = new DefaultBindableTo<T, TAction>(_kernel, this.m_StaticEventBinder, this.m_Filter);
                bindable.BindAsAction<TAction>();
                return bindable;
            }

            public IBindableTo<T, TCommand> ToCommand<TCommand>(params string[] arguments) where TCommand : ICommand
            {
                var bindable = new DefaultBindableTo<T, TCommand>(_kernel, this.m_StaticEventBinder, this.m_Filter);
                bindable.BindAsCommand<TCommand>(arguments);
                return bindable;
            }
            
            public IBindableTo<T, TListener> ToListener<TListener>() where TListener : IEventListener<TContext>
            {
                var bindable = new DefaultBindableTo<T, TListener>(_kernel, this.m_StaticEventBinder, this.m_Filter);
                bindable.BindAsListener<TListener>();
                return bindable;
            }

            public IBindableTo<T, TComponentizedEntity> ToComponentizedEntity<TComponentizedEntity>(bool onlyFirst) where TComponentizedEntity : ComponentizedEntity
            {
                var bindable = new DefaultBindableTo<T, TComponentizedEntity>(_kernel, this.m_StaticEventBinder, this.m_Filter);
                bindable.BindAsComponentizedEntity<TComponentizedEntity>(onlyFirst);
                return bindable;
            }

            public IBindableTo<T, ComponentizedEntity> ToAllComponentizedEntities()
            {
                var bindable = new DefaultBindableTo<T, ComponentizedEntity>(_kernel, this.m_StaticEventBinder, this.m_Filter);
                bindable.BindAsComponentizedEntity<ComponentizedEntity>(false);
                return bindable;
            }

            public void ToNothing()
            {
                this.m_StaticEventBinder._bindings.Add(
                    (gameContext, eventEngine, @event) =>
                    {
                        if (!(@event is T))
                        {
                            return false;
                        }

                        if (!this.m_Filter(@event as T))
                        {
                            return false;
                        }

                        return true;
                    });
            }
        }
        
        private class DefaultBindableOn<TEvent, TEntity> : IBindableOn<TEvent, TEntity>
            where TEvent : Event where TEntity : IEntity
        {
            /// <summary>
            /// The m_ filter.
            /// </summary>
            protected readonly Func<TEvent, bool> m_Filter;

            /// <summary>
            /// The m_ filter.
            /// </summary>
            protected readonly Func<TEntity, bool> m_EntityFilter;

            /// <summary>
            /// The m_ static event binder.
            /// </summary>
            protected readonly StaticEventBinder<TContext> m_StaticEventBinder;

            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultBindableOn{TEvent,TEntity}"/> class.
            /// </summary>
            /// <param name="staticEventBinder">
            /// The static event binder.
            /// </param>
            /// <param name="filter">
            /// The filter.
            /// </param>
            public DefaultBindableOn(StaticEventBinder<TContext> staticEventBinder, Func<TEvent, bool> filter, Func<TEntity, bool> entityFilter)
            {
                this.m_StaticEventBinder = staticEventBinder;
                this.m_Filter = filter;
                this.m_EntityFilter = entityFilter;
            }

            /// <summary>
            /// The to.
            /// </summary>
            /// <typeparam name="TEntityAction">
            /// </typeparam>
            /// <returns>
            /// The <see cref="StaticEventBinder"/>.
            /// </returns>
            public IBindableOnTo<TEvent, TEntity, TEntityAction> To<TEntityAction>()
                where TEntityAction : IEventEntityAction<TEntity>
            {
                var bindable = new DefaultBindableOnTo<TEvent, TEntity, TEntityAction>(
                    this.m_StaticEventBinder, 
                    this.m_Filter,
                    this.m_EntityFilter);
                bindable.Bind();
                return bindable;
            }
        }
        
        private class DefaultBindableOnTo<TEvent, TEntity, TEntityAction> :
            IBindableOnTo<TEvent, TEntity, TEntityAction>
            where TEvent : Event where TEntity : IEntity where TEntityAction : IEventEntityAction<TEntity>
        {
            private bool m_Propagate = false;

            /// <summary>
            /// The m_ filter.
            /// </summary>
            private readonly Func<TEvent, bool> m_Filter;

            /// <summary>
            /// The m_ filter.
            /// </summary>
            private readonly Func<TEntity, bool> m_EntityFilter;

            /// <summary>
            /// The m_ static event binder.
            /// </summary>
            private readonly StaticEventBinder<TContext> m_StaticEventBinder;

            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultBindableOnTo{TEvent,TEntity,TEntityAction}"/> class.
            /// </summary>
            /// <param name="staticEventBinder">
            /// The static event binder.
            /// </param>
            /// <param name="kernel">
            /// The dependency injection kernel.
            /// </param>
            /// <param name="filter">
            /// The filter.
            /// </param>
            public DefaultBindableOnTo(StaticEventBinder<TContext> staticEventBinder, Func<TEvent, bool> filter, Func<TEntity, bool> entityFilter)
            {
                this.m_StaticEventBinder = staticEventBinder;
                this.m_Filter = filter;
                this.m_EntityFilter = entityFilter;
            }

            /// <summary>
            /// The bind.
            /// </summary>
            /// <exception cref="InvalidOperationException">
            /// </exception>
            public void Bind()
            {
                if (typeof(TContext) != typeof(IGameContext))
                {
                    throw new InvalidOperationException(
                        "Entity action bindings can only be used against IGameContext-based event engines.");
                }

                var action = this.m_StaticEventBinder._kernel.Get<TEntityAction>();
                this.m_StaticEventBinder._bindings.Add(
                    (context, eventEngine, @event) =>
                    {
                        var gameContext = (IGameContext)context;
                        if (!(@event is TEvent))
                        {
                            return false;
                        }

                        if (!this.m_Filter(@event as TEvent))
                        {
                            return false;
                        }

                        if (gameContext.World == null)
                        {
                            return false;
                        }

                        var entities = gameContext.World.GetEntitiesForWorld(gameContext.Hierarchy).OfType<TEntity>().Where(this.m_EntityFilter).ToList();
                        var exactMatch = entities.FirstOrDefault(x => x.GetType() == typeof(TEntity));
                        var exactOrDerivedMatch = entities.FirstOrDefault(x => x is TEntity);
                        if (exactMatch != null)
                        {
                            action.Handle(gameContext, (TEntity)exactMatch, @event);
                            return !this.m_Propagate;
                        }

                        if (exactOrDerivedMatch != null)
                        {
                            action.Handle(gameContext, (TEntity)exactOrDerivedMatch, @event);
                            return !this.m_Propagate;
                        }

                        return false;
                    });
            }

            public void Propagate()
            {
                this.m_Propagate = true;
            }
        }
        
        private class DefaultBindableOnToTogglable<TEvent, TEntity> : IBindableOnTo<TEvent, TEntity>
            where TEvent : Event where TEntity : IEntity, IEventTogglable
        {
            private bool m_Propagate = false;

            /// <summary>
            /// The m_ filter.
            /// </summary>
            private readonly Func<TEvent, bool> m_Filter;

            /// <summary>
            /// The m_ static event binder.
            /// </summary>
            private readonly StaticEventBinder<TContext> m_StaticEventBinder;

            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultBindableOnToTogglable{TEvent,TEntity}"/> class.
            /// </summary>
            /// <param name="staticEventBinder">
            /// The static event binder.
            /// </param>
            /// <param name="filter">
            /// The filter.
            /// </param>
            public DefaultBindableOnToTogglable(
                StaticEventBinder<TContext> staticEventBinder, 
                Func<TEvent, bool> filter)
            {
                this.m_StaticEventBinder = staticEventBinder;
                this.m_Filter = filter;
            }

            /// <summary>
            /// The bind.
            /// </summary>
            /// <param name="id">
            /// The id.
            /// </param>
            /// <exception cref="InvalidOperationException">
            /// </exception>
            public void Bind(string id)
            {
                if (typeof(TContext) != typeof(IGameContext))
                {
                    throw new InvalidOperationException(
                        "Entity action bindings can only be used against IGameContext-based event engines.");
                }

                this.m_StaticEventBinder._bindings.Add(
                    (context, eventEngine, @event) =>
                    {
                        var gameContext = (IGameContext)context;
                        if (!(@event is TEvent))
                        {
                            return false;
                        }

                        if (!this.m_Filter(@event as TEvent))
                        {
                            return false;
                        }

                        if (gameContext.World == null)
                        {
                            return false;
                        }

                        var entities = gameContext.World.GetEntitiesForWorld(gameContext.Hierarchy).ToList();
                        var exactMatch = entities.FirstOrDefault(x => x.GetType() == typeof(TEntity));
                        var exactOrDerivedMatch = entities.FirstOrDefault(x => x is TEntity);
                        if (exactMatch != null)
                        {
                            ((TEntity)exactMatch).Toggle(id);
                            return !this.m_Propagate;
                        }

                        if (exactOrDerivedMatch != null)
                        {
                            ((TEntity)exactOrDerivedMatch).Toggle(id);
                            return !this.m_Propagate;
                        }

                        return false;
                    });
            }

            public void Propagate()
            {
                this.m_Propagate = true;
            }
        }
        
        private class DefaultBindableOnTogglable<TEvent, TEntity> : DefaultBindableOn<TEvent, TEntity>, 
                                                                    IBindableOnTogglable<TEvent, TEntity>
            where TEvent : Event where TEntity : IEntity, IEventTogglable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultBindableOnTogglable{TEvent,TEntity}"/> class.
            /// </summary>
            /// <param name="staticEventBinder">
            /// The static event binder.
            /// </param>
            /// <param name="filter">
            /// The filter.
            /// </param>
            public DefaultBindableOnTogglable(StaticEventBinder<TContext> staticEventBinder, Func<TEvent, bool> filter)
                : base(staticEventBinder, filter, t => true)
            {
            }

            /// <summary>
            /// The to toggle.
            /// </summary>
            /// <param name="id">
            /// The id.
            /// </param>
            /// <returns>
            /// The <see cref="StaticEventBinder"/>.
            /// </returns>
            public IBindableOnTo<TEvent, TEntity> ToToggle(string id)
            {
                var bindable = new DefaultBindableOnToTogglable<TEvent, TEntity>(
                    this.m_StaticEventBinder,
                    this.m_Filter);
                bindable.Bind(id);
                return bindable;
            }
        }
        
        private class DefaultBindableTo<TEvent, TTarget> : IBindableTo<TEvent, TTarget>
            where TEvent : Event
        {
            private readonly Func<TEvent, bool> m_Filter;

            private readonly IKernel _kernel;

            private readonly StaticEventBinder<TContext> m_StaticEventBinder;
            
            public DefaultBindableTo(IKernel kernel, StaticEventBinder<TContext> staticEventBinder, Func<TEvent, bool> filter)
            {
                _kernel = kernel;
                this.m_StaticEventBinder = staticEventBinder;
                this.m_Filter = filter;
            }
            
            public void BindAsAction<TAction>() where TAction : IEventAction<TContext>
            {
                var action = this.m_StaticEventBinder._kernel.Get<TAction>();
                this.m_StaticEventBinder._bindings.Add(
                    (gameContext, eventEngine, @event) =>
                    {
                        if (!(@event is TEvent))
                        {
                            return false;
                        }

                        if (!this.m_Filter(@event as TEvent))
                        {
                            return false;
                        }

                        action.Handle(gameContext, @event);
                        return true;
                    });
            }
            
            public void BindAsCommand<TCommand>(params string[] parameters) where TCommand : ICommand
            {
                if (typeof(TContext) != typeof(IGameContext))
                {
                    throw new InvalidOperationException(
                        "Command bindings can only be used against IGameContext-based event engines.");
                }

                var command = this.m_StaticEventBinder._kernel.Get<TCommand>();
                this.m_StaticEventBinder._bindings.Add(
                    (gameContext, eventEngine, @event) =>
                    {
                        if (!(@event is TEvent))
                        {
                            return false;
                        }

                        if (!this.m_Filter(@event as TEvent))
                        {
                            return false;
                        }

                        command.Execute((IGameContext)gameContext, string.Empty, parameters);
                        return true;
                    });
            }
            
            public void BindAsListener<TListener>() where TListener : IEventListener<TContext>
            {
                var listener = this.m_StaticEventBinder._kernel.Get<TListener>();
                this.m_StaticEventBinder._bindings.Add(
                    (gameContext, eventEngine, @event) =>
                    {
                        if (!(@event is TEvent))
                        {
                            return false;
                        }

                        if (!this.m_Filter(@event as TEvent))
                        {
                            return false;
                        }

                        return listener.Handle(gameContext, eventEngine, @event);
                    });
            }

            public void BindAsComponentizedEntity<TComponentizedEntity>(bool onlyFirst) where TComponentizedEntity : ComponentizedEntity
            {
                if (typeof(TContext) != typeof(IGameContext))
                {
                    throw new InvalidOperationException(
                        "Componentized entity bindings can only be used against IGameContext-based event engines.");
                }
                
                this.m_StaticEventBinder._bindings.Add(
                    (context, eventEngine, @event) =>
                    {
                        if (!(@event is TEvent))
                        {
                            return false;
                        }

                        if (!this.m_Filter(@event as TEvent))
                        {
                            return false;
                        }

                        var gameContext = (IGameContext) context;

                        if (gameContext.World == null)
                        {
                            return false;
                        }

                        var gameEventEngine = (IEventEngine<IGameContext>)eventEngine;
                        var allEntities = gameContext.World.GetEntitiesForWorld(_kernel.Hierarchy);
                        var entitiesMatchingType = allEntities.OfType<TComponentizedEntity>();

                        return onlyFirst ? 
                            (entitiesMatchingType.FirstOrDefault()?.Handle(gameContext, gameEventEngine, @event) ?? false) : 
                            entitiesMatchingType.ToArray().Select(entity => entity.Handle(gameContext, gameEventEngine, @event)).Any(handled => handled);
                    });
            }
        }
    }
}