namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ninject;
    using Ninject.Syntax;

    /// <summary>
    /// The static event binder.
    /// </summary>
    /// <typeparam name="TContext">
    /// </typeparam>
    public abstract class StaticEventBinder<TContext> : IEventBinder<TContext>
    {
        /// <summary>
        /// The m_ bindings.
        /// </summary>
        private readonly List<Func<TContext, IEventEngine<TContext>, Event, bool>> m_Bindings;

        /// <summary>
        /// The m_ configured.
        /// </summary>
        private bool m_Configured;

        /// <summary>
        /// The m_ resolution root.
        /// </summary>
        private IResolutionRoot m_ResolutionRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticEventBinder{TContext}"/> class.
        /// </summary>
        protected StaticEventBinder()
        {
            this.m_Bindings = new List<Func<TContext, IEventEngine<TContext>, Event, bool>>();
        }

        /// <summary>
        /// The Bindable interface.
        /// </summary>
        /// <typeparam name="TEvent">
        /// </typeparam>
        protected interface IBindable<TEvent>
            where TEvent : Event
        {
            /// <summary>
            /// The on.
            /// </summary>
            /// <typeparam name="TEntity">
            /// </typeparam>
            /// <returns>
            /// The <see cref="StaticEventBinder"/>.
            /// </returns>
            IBindableOn<TEvent, TEntity> On<TEntity>() where TEntity : IEntity;

            /// <summary>
            /// The on togglable.
            /// </summary>
            /// <typeparam name="TEntity">
            /// </typeparam>
            /// <returns>
            /// The <see cref="StaticEventBinder"/>.
            /// </returns>
            IBindableOnTogglable<TEvent, TEntity> OnTogglable<TEntity>() where TEntity : IEntity, IEventTogglable;

            /// <summary>
            /// The to.
            /// </summary>
            /// <typeparam name="TAction">
            /// </typeparam>
            /// <returns>
            /// The <see cref="StaticEventBinder"/>.
            /// </returns>
            IBindableTo<TEvent, TAction> To<TAction>() where TAction : IEventAction<TContext>;

            /// <summary>
            /// The to command.
            /// </summary>
            /// <param name="arguments">
            /// The arguments.
            /// </param>
            /// <typeparam name="TCommand">
            /// </typeparam>
            /// <returns>
            /// The <see cref="StaticEventBinder"/>.
            /// </returns>
            IBindableTo<TEvent, TCommand> ToCommand<TCommand>(params string[] arguments) where TCommand : ICommand;

            /// <summary>
            /// The to listener.
            /// </summary>
            /// <typeparam name="TListener">
            /// </typeparam>
            /// <returns>
            /// The <see cref="StaticEventBinder"/>.
            /// </returns>
            IBindableTo<TEvent, TListener> ToListener<TListener>() where TListener : IEventListener<TContext>;
        }

        /// <summary>
        /// The BindableOn interface.
        /// </summary>
        /// <typeparam name="TEvent">
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// </typeparam>
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

        /// <summary>
        /// The BindableOnTo interface.
        /// </summary>
        /// <typeparam name="TEvent">
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <typeparam name="TEntityAction">
        /// </typeparam>
        protected interface IBindableOnTo<TEvent, TEntity, TEntityAction>
            where TEvent : Event where TEntity : IEntity where TEntityAction : IEventEntityAction<TEntity>
        {
        }

        /// <summary>
        /// The BindableOnTo interface.
        /// </summary>
        /// <typeparam name="TEvent">
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// </typeparam>
        protected interface IBindableOnTo<TEvent, TEntity>
            where TEvent : Event where TEntity : IEntity, IEventTogglable
        {
        }

        /// <summary>
        /// The BindableOnTogglable interface.
        /// </summary>
        /// <typeparam name="TEvent">
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// </typeparam>
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

        /// <summary>
        /// The BindableTo interface.
        /// </summary>
        /// <typeparam name="TEvent">
        /// </typeparam>
        /// <typeparam name="TTarget">
        /// </typeparam>
        protected interface IBindableTo<TEvent, TTarget>
            where TEvent : Event
        {
        }

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public int Priority
        {
            get
            {
                return 100;
            }
        }

        /// <summary>
        /// The assign.
        /// </summary>
        /// <param name="resolutionRoot">
        /// The resolution root.
        /// </param>
        public void Assign(IResolutionRoot resolutionRoot)
        {
            this.m_ResolutionRoot = resolutionRoot;
        }

        /// <summary>
        /// The configure.
        /// </summary>
        public abstract void Configure();

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="eventEngine">
        /// The event engine.
        /// </param>
        /// <param name="event">
        /// The event.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Handle(TContext context, IEventEngine<TContext> eventEngine, Event @event)
        {
            if (!this.m_Configured)
            {
                this.Configure();
                this.m_Configured = true;
            }

            foreach (var binding in this.m_Bindings)
            {
                if (binding(context, eventEngine, @event))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The bind.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <typeparam name="TEvent">
        /// </typeparam>
        /// <returns>
        /// The <see cref="StaticEventBinder"/>.
        /// </returns>
        protected IBindable<TEvent> Bind<TEvent>(Func<TEvent, bool> filter) where TEvent : Event
        {
            return new DefaultBindable<TEvent>(this, filter);
        }

        /// <summary>
        /// The default bindable.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        private class DefaultBindable<T> : IBindable<T>
            where T : Event
        {
            /// <summary>
            /// The m_ filter.
            /// </summary>
            private readonly Func<T, bool> m_Filter;

            /// <summary>
            /// The m_ static event binder.
            /// </summary>
            private readonly StaticEventBinder<TContext> m_StaticEventBinder;

            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultBindable{T}"/> class.
            /// </summary>
            /// <param name="staticEventBinder">
            /// The static event binder.
            /// </param>
            /// <param name="filter">
            /// The filter.
            /// </param>
            public DefaultBindable(StaticEventBinder<TContext> staticEventBinder, Func<T, bool> filter)
            {
                this.m_StaticEventBinder = staticEventBinder;
                this.m_Filter = filter;
            }

            /// <summary>
            /// The on.
            /// </summary>
            /// <typeparam name="TEntity">
            /// </typeparam>
            /// <returns>
            /// The <see cref="StaticEventBinder"/>.
            /// </returns>
            public IBindableOn<T, TEntity> On<TEntity>() where TEntity : IEntity
            {
                return new DefaultBindableOn<T, TEntity>(this.m_StaticEventBinder, this.m_Filter);
            }

            /// <summary>
            /// The on togglable.
            /// </summary>
            /// <typeparam name="TEntity">
            /// </typeparam>
            /// <returns>
            /// The <see cref="StaticEventBinder"/>.
            /// </returns>
            public IBindableOnTogglable<T, TEntity> OnTogglable<TEntity>() where TEntity : IEntity, IEventTogglable
            {
                return new DefaultBindableOnTogglable<T, TEntity>(this.m_StaticEventBinder, this.m_Filter);
            }

            /// <summary>
            /// The to.
            /// </summary>
            /// <typeparam name="TAction">
            /// </typeparam>
            /// <returns>
            /// The <see cref="StaticEventBinder"/>.
            /// </returns>
            public IBindableTo<T, TAction> To<TAction>() where TAction : IEventAction<TContext>
            {
                var bindable = new DefaultBindableTo<T, TAction>(this.m_StaticEventBinder, this.m_Filter);
                bindable.BindAsAction<TAction>();
                return bindable;
            }

            /// <summary>
            /// The to command.
            /// </summary>
            /// <param name="arguments">
            /// The arguments.
            /// </param>
            /// <typeparam name="TCommand">
            /// </typeparam>
            /// <returns>
            /// The <see cref="StaticEventBinder"/>.
            /// </returns>
            public IBindableTo<T, TCommand> ToCommand<TCommand>(params string[] arguments) where TCommand : ICommand
            {
                var bindable = new DefaultBindableTo<T, TCommand>(this.m_StaticEventBinder, this.m_Filter);
                bindable.BindAsCommand<TCommand>(arguments);
                return bindable;
            }

            /// <summary>
            /// The to listener.
            /// </summary>
            /// <typeparam name="TListener">
            /// </typeparam>
            /// <returns>
            /// The <see cref="StaticEventBinder"/>.
            /// </returns>
            public IBindableTo<T, TListener> ToListener<TListener>() where TListener : IEventListener<TContext>
            {
                var bindable = new DefaultBindableTo<T, TListener>(this.m_StaticEventBinder, this.m_Filter);
                bindable.BindAsListener<TListener>();
                return bindable;
            }
        }

        /// <summary>
        /// The default bindable on.
        /// </summary>
        /// <typeparam name="TEvent">
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// </typeparam>
        private class DefaultBindableOn<TEvent, TEntity> : IBindableOn<TEvent, TEntity>
            where TEvent : Event where TEntity : IEntity
        {
            /// <summary>
            /// The m_ filter.
            /// </summary>
            protected readonly Func<TEvent, bool> m_Filter;

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
            public DefaultBindableOn(StaticEventBinder<TContext> staticEventBinder, Func<TEvent, bool> filter)
            {
                this.m_StaticEventBinder = staticEventBinder;
                this.m_Filter = filter;
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
                    this.m_Filter);
                bindable.Bind();
                return bindable;
            }
        }

        /// <summary>
        /// The default bindable on to.
        /// </summary>
        /// <typeparam name="TEvent">
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <typeparam name="TEntityAction">
        /// </typeparam>
        private class DefaultBindableOnTo<TEvent, TEntity, TEntityAction> :
            IBindableOnTo<TEvent, TEntity, TEntityAction>
            where TEvent : Event where TEntity : IEntity where TEntityAction : IEventEntityAction<TEntity>
        {
            /// <summary>
            /// The m_ filter.
            /// </summary>
            private readonly Func<TEvent, bool> m_Filter;

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
            /// <param name="filter">
            /// The filter.
            /// </param>
            public DefaultBindableOnTo(StaticEventBinder<TContext> staticEventBinder, Func<TEvent, bool> filter)
            {
                this.m_StaticEventBinder = staticEventBinder;
                this.m_Filter = filter;
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

                var action = this.m_StaticEventBinder.m_ResolutionRoot.Get<TEntityAction>();
                this.m_StaticEventBinder.m_Bindings.Add(
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

                        if (gameContext.World.Entities == null)
                        {
                            return false;
                        }

                        var exactMatch = gameContext.World.Entities.FirstOrDefault(x => x.GetType() == typeof(TEntity));
                        var exactOrDerivedMatch = gameContext.World.Entities.FirstOrDefault(x => x is TEntity);
                        if (exactMatch != null)
                        {
                            action.Handle(gameContext, (TEntity)exactMatch, @event);
                            return true;
                        }

                        if (exactOrDerivedMatch != null)
                        {
                            action.Handle(gameContext, (TEntity)exactOrDerivedMatch, @event);
                            return true;
                        }

                        return false;
                    });
            }
        }

        /// <summary>
        /// The default bindable on to togglable.
        /// </summary>
        /// <typeparam name="TEvent">
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// </typeparam>
        private class DefaultBindableOnToTogglable<TEvent, TEntity> : IBindableOnTo<TEvent, TEntity>
            where TEvent : Event where TEntity : IEntity, IEventTogglable
        {
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

                this.m_StaticEventBinder.m_Bindings.Add(
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

                        if (gameContext.World.Entities == null)
                        {
                            return false;
                        }

                        var exactMatch = gameContext.World.Entities.FirstOrDefault(x => x.GetType() == typeof(TEntity));
                        var exactOrDerivedMatch = gameContext.World.Entities.FirstOrDefault(x => x is TEntity);
                        if (exactMatch != null)
                        {
                            ((TEntity)exactMatch).Toggle(id);
                            return true;
                        }

                        if (exactOrDerivedMatch != null)
                        {
                            ((TEntity)exactOrDerivedMatch).Toggle(id);
                            return true;
                        }

                        return false;
                    });
            }
        }

        /// <summary>
        /// The default bindable on togglable.
        /// </summary>
        /// <typeparam name="TEvent">
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// </typeparam>
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
                : base(staticEventBinder, filter)
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

        /// <summary>
        /// The default bindable to.
        /// </summary>
        /// <typeparam name="TEvent">
        /// </typeparam>
        /// <typeparam name="TTarget">
        /// </typeparam>
        private class DefaultBindableTo<TEvent, TTarget> : IBindableTo<TEvent, TTarget>
            where TEvent : Event
        {
            /// <summary>
            /// The m_ filter.
            /// </summary>
            private readonly Func<TEvent, bool> m_Filter;

            /// <summary>
            /// The m_ static event binder.
            /// </summary>
            private readonly StaticEventBinder<TContext> m_StaticEventBinder;

            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultBindableTo{TEvent,TTarget}"/> class.
            /// </summary>
            /// <param name="staticEventBinder">
            /// The static event binder.
            /// </param>
            /// <param name="filter">
            /// The filter.
            /// </param>
            public DefaultBindableTo(StaticEventBinder<TContext> staticEventBinder, Func<TEvent, bool> filter)
            {
                this.m_StaticEventBinder = staticEventBinder;
                this.m_Filter = filter;
            }

            /// <summary>
            /// The bind as action.
            /// </summary>
            /// <typeparam name="TAction">
            /// </typeparam>
            public void BindAsAction<TAction>() where TAction : IEventAction<TContext>
            {
                var action = this.m_StaticEventBinder.m_ResolutionRoot.Get<TAction>();
                this.m_StaticEventBinder.m_Bindings.Add(
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

            /// <summary>
            /// The bind as command.
            /// </summary>
            /// <param name="parameters">
            /// The parameters.
            /// </param>
            /// <typeparam name="TCommand">
            /// </typeparam>
            /// <exception cref="InvalidOperationException">
            /// </exception>
            public void BindAsCommand<TCommand>(params string[] parameters) where TCommand : ICommand
            {
                if (typeof(TContext) != typeof(IGameContext))
                {
                    throw new InvalidOperationException(
                        "Command bindings can only be used against IGameContext-based event engines.");
                }

                var command = this.m_StaticEventBinder.m_ResolutionRoot.Get<TCommand>();
                this.m_StaticEventBinder.m_Bindings.Add(
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

            /// <summary>
            /// The bind as listener.
            /// </summary>
            /// <typeparam name="TListener">
            /// </typeparam>
            public void BindAsListener<TListener>() where TListener : IEventListener<TContext>
            {
                var listener = this.m_StaticEventBinder.m_ResolutionRoot.Get<TListener>();
                this.m_StaticEventBinder.m_Bindings.Add(
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
        }
    }
}