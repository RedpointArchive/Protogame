using System;
using System.Collections.Generic;
using Ninject;
using System.Linq;
using Ninject.Syntax;

namespace Protogame
{
    public abstract class StaticEventBinder : IEventBinder
    {
        private IResolutionRoot m_ResolutionRoot;
        private List<Func<IGameContext, IEventEngine, Event, bool>> m_Bindings;
        private bool m_Configured;
    
        public int Priority { get { return 100; } }
        
        protected StaticEventBinder()
        {
            this.m_Bindings = new List<Func<IGameContext, IEventEngine, Event, bool>>();
        }
        
        public void Assign(IResolutionRoot resolutionRoot)
        {
            this.m_ResolutionRoot = resolutionRoot;
        }
        
        public bool Handle(IGameContext gameContext, IEventEngine eventEngine, Event @event)
        {
            if (!this.m_Configured)
            {
                this.Configure();
                this.m_Configured = true;
            }
            foreach (var binding in this.m_Bindings)
            {
                if (binding(gameContext, eventEngine, @event))
                    return true;
            }
            return false;
        }
        
        public abstract void Configure();
        
        #region Binding Helpers
        
        protected IBindable<TEvent> Bind<TEvent>(Func<TEvent, bool> filter) where TEvent : Event
        {
            return new DefaultBindable<TEvent>(this, filter);
        }
        
        #region Interfaces
        
        protected interface IBindable<TEvent> where TEvent : Event
        {
            IBindableTo<TEvent, TAction> To<TAction>() where TAction : IEventAction;
            IBindableTo<TEvent, TListener> ToListener<TListener>() where TListener : IEventListener;
            IBindableTo<TEvent, TCommand> ToCommand<TCommand>(params string[] arguments) where TCommand : ICommand;
            IBindableOn<TEvent, TEntity> On<TEntity>() where TEntity : IEntity;
            IBindableOnTogglable<TEvent, TEntity> OnTogglable<TEntity>() where TEntity : IEntity, IEventTogglable;
        }
        
        protected interface IBindableTo<TEvent, TTarget> where TEvent : Event
        {
        }
        
        protected interface IBindableOn<TEvent, TEntity>
            where TEvent : Event
            where TEntity : IEntity
        {
            IBindableOnTo<TEvent, TEntity, TEntityAction> To<TEntityAction>()
                where TEntityAction : IEventEntityAction<TEntity>;
        }
        
        protected interface IBindableOnTogglable<TEvent, TEntity> : IBindableOn<TEvent, TEntity>
            where TEvent : Event
            where TEntity : IEntity, IEventTogglable
        {
            IBindableOnTo<TEvent, TEntity> ToToggle(string id);
        }
        
        protected interface IBindableOnTo<TEvent, TEntity, TEntityAction>
            where TEvent : Event
            where TEntity : IEntity
            where TEntityAction : IEventEntityAction<TEntity>
        {
        }
        
        protected interface IBindableOnTo<TEvent, TEntity>
            where TEvent : Event
            where TEntity : IEntity, IEventTogglable
        {
        }
        
        #endregion
        
        #region Implementations
        
        private class DefaultBindable<T> : IBindable<T> where T : Event
        {
            private readonly StaticEventBinder m_StaticEventBinder;
            private readonly Func<T, bool> m_Filter;
            
            public DefaultBindable(
                StaticEventBinder staticEventBinder,
                Func<T, bool> filter)
            {
                this.m_StaticEventBinder = staticEventBinder;
                this.m_Filter = filter;
            }

            public IBindableTo<T, TAction> To<TAction>() where TAction : IEventAction
            {
                var bindable = new DefaultBindableTo<T, TAction>(
                    this.m_StaticEventBinder,
                    this.m_Filter);
                bindable.BindAsAction<TAction>();
                return bindable;
            }

            public IBindableTo<T, TListener> ToListener<TListener>() where TListener : IEventListener
            {
                var bindable = new DefaultBindableTo<T, TListener>(
                    this.m_StaticEventBinder,
                    this.m_Filter);
                bindable.BindAsListener<TListener>();
                return bindable;
            }

            public IBindableTo<T, TCommand> ToCommand<TCommand>(params string[] arguments) where TCommand : ICommand
            {
                var bindable = new DefaultBindableTo<T, TCommand>(
                    this.m_StaticEventBinder,
                    this.m_Filter);
                bindable.BindAsCommand<TCommand>(arguments);
                return bindable;
            }

            public IBindableOn<T, TEntity> On<TEntity>() where TEntity : IEntity
            {
                return new DefaultBindableOn<T, TEntity>(
                    this.m_StaticEventBinder,
                    this.m_Filter);
            }

            public IBindableOnTogglable<T, TEntity> OnTogglable<TEntity>() where TEntity : IEntity, IEventTogglable
            {
                return new DefaultBindableOnTogglable<T, TEntity>(
                    this.m_StaticEventBinder,
                    this.m_Filter);
            }
        }
        
        private class DefaultBindableTo<TEvent, TTarget> : IBindableTo<TEvent, TTarget>
            where TEvent : Event
        {
            private readonly StaticEventBinder m_StaticEventBinder;
            private readonly Func<TEvent, bool> m_Filter;
            
            public DefaultBindableTo(
                StaticEventBinder staticEventBinder,
                Func<TEvent, bool> filter)
            {
                this.m_StaticEventBinder = staticEventBinder;
                this.m_Filter = filter;
            }
            
            public void BindAsAction<TAction>() where TAction : IEventAction
            {
                var action = this.m_StaticEventBinder.m_ResolutionRoot.Get<TAction>();
                this.m_StaticEventBinder.m_Bindings.Add((gameContext, eventEngine, @event) =>
                {
                    if (!(@event is TEvent))
                        return false;
                    if (!this.m_Filter(@event as TEvent))
                        return false;
                    action.Handle(gameContext, @event);
                    return true;
                });
            }
            
            public void BindAsListener<TListener>() where TListener : IEventListener
            {
                var listener = this.m_StaticEventBinder.m_ResolutionRoot.Get<TListener>();
                this.m_StaticEventBinder.m_Bindings.Add((gameContext, eventEngine, @event) =>
                {
                    if (!(@event is TEvent))
                        return false;
                    if (!this.m_Filter(@event as TEvent))
                        return false;
                    return listener.Handle(gameContext, eventEngine, @event);
                });
            }
            
            public void BindAsCommand<TCommand>(params string[] parameters) where TCommand : ICommand
            {
                var command = this.m_StaticEventBinder.m_ResolutionRoot.Get<TCommand>();
                this.m_StaticEventBinder.m_Bindings.Add((gameContext, eventEngine, @event) =>
                {
                    if (!(@event is TEvent))
                        return false;
                    if (!this.m_Filter(@event as TEvent))
                        return false;
                    command.Execute(gameContext, "", parameters);
                    return true;
                });
            }
        }
        
        private class DefaultBindableOn<TEvent, TEntity> : IBindableOn<TEvent, TEntity>
            where TEvent : Event
            where TEntity : IEntity
        {
            protected readonly StaticEventBinder m_StaticEventBinder;
            protected readonly Func<TEvent, bool> m_Filter;
            
            public DefaultBindableOn(
                StaticEventBinder staticEventBinder,
                Func<TEvent, bool> filter)
            {
                this.m_StaticEventBinder = staticEventBinder;
                this.m_Filter = filter;
            }
            
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
        
        private class DefaultBindableOnTogglable<TEvent, TEntity> : 
            DefaultBindableOn<TEvent, TEntity>,
            IBindableOnTogglable<TEvent, TEntity>
            where TEvent : Event
            where TEntity : IEntity, IEventTogglable
        {
            public DefaultBindableOnTogglable(
                StaticEventBinder staticEventBinder,
                Func<TEvent, bool> filter) : base(staticEventBinder, filter)
            {
            }
            
            public IBindableOnTo<TEvent, TEntity> ToToggle(string id)
            {
                var bindable = new DefaultBindableOnToTogglable<TEvent, TEntity>(
                    this.m_StaticEventBinder,
                    this.m_Filter);
                bindable.Bind(id);
                return bindable;
            }
        }
        
        private class DefaultBindableOnTo<TEvent, TEntity, TEntityAction>
            : IBindableOnTo<TEvent, TEntity, TEntityAction>
            where TEvent : Event
            where TEntity : IEntity
            where TEntityAction : IEventEntityAction<TEntity>
        {
            private readonly StaticEventBinder m_StaticEventBinder;
            private readonly Func<TEvent, bool> m_Filter;
            
            public DefaultBindableOnTo(
                StaticEventBinder staticEventBinder,
                Func<TEvent, bool> filter)
            {
                this.m_StaticEventBinder = staticEventBinder;
                this.m_Filter = filter;
            }
            
            public void Bind()
            {
                var action = this.m_StaticEventBinder.m_ResolutionRoot.Get<TEntityAction>();
                this.m_StaticEventBinder.m_Bindings.Add((gameContext, eventEngine, @event) =>
                {
                    if (!(@event is TEvent))
                        return false;
                    if (!this.m_Filter(@event as TEvent))
                        return false;
                    if (gameContext.World == null)
                        return false;
                    if (gameContext.World.Entities == null)
                        return false;
                    var exactMatch = gameContext.World.Entities.FirstOrDefault(x => x.GetType() == typeof(TEntity));
                    var exactOrDerivedMatch = gameContext.World.Entities.FirstOrDefault(x => x is TEntity);
                    if (exactMatch != null)
                    {
                        action.Handle((TEntity)exactMatch, @event);
                        return true;
                    }
                    if (exactOrDerivedMatch != null)
                    {
                        action.Handle((TEntity)exactOrDerivedMatch, @event);
                        return true;
                    }
                    return false;
                });
            }
        }
        
        private class DefaultBindableOnToTogglable<TEvent, TEntity>
            : IBindableOnTo<TEvent, TEntity>
            where TEvent : Event
            where TEntity : IEntity, IEventTogglable
        {
            private readonly StaticEventBinder m_StaticEventBinder;
            private readonly Func<TEvent, bool> m_Filter;
            
            public DefaultBindableOnToTogglable(
                StaticEventBinder staticEventBinder,
                Func<TEvent, bool> filter)
            {
                this.m_StaticEventBinder = staticEventBinder;
                this.m_Filter = filter;
            }
            
            public void Bind(string id)
            {
                this.m_StaticEventBinder.m_Bindings.Add((gameContext, eventEngine, @event) =>
                {
                    if (!(@event is TEvent))
                        return false;
                    if (!this.m_Filter(@event as TEvent))
                        return false;
                    if (gameContext.World == null)
                        return false;
                    if (gameContext.World.Entities == null)
                        return false;
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
        
        #endregion
        
        #endregion
    }
}

