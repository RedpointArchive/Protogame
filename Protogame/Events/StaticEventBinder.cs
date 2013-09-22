using System;
using System.Collections.Generic;
using Ninject;
using System.Linq;

namespace Protogame
{
    public abstract class StaticEventBinder : IEventBinder
    {
        private IKernel m_Kernel;
        private List<Func<IGameContext, IEventEngine, Event, bool>> m_Bindings;
        private bool m_Configured;
    
        public int Priority { get { return 100; } }
        
        protected StaticEventBinder(IKernel kernel)
        {
            this.m_Kernel = kernel;
            this.m_Bindings = new List<Func<IGameContext, IEventEngine, Event, bool>>();
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
            return new DefaultBindable<TEvent>(this);
        }
        
        #region Interfaces
        
        protected interface IBindable<TEvent> where TEvent : Event
        {
            IBindableTo<TEvent, TAction> To<TAction>() where TAction : IEventAction;
            IBindableTo<TEvent, TListener> ToListener<TListener>() where TListener : IEventListener;
            IBindableTo<TEvent, TCommand> ToCommand<TCommand>(params string[] arguments) where TCommand : ICommand;
            IBindableOn<TEvent, TEntity> On<TEntity>() where TEntity : IEntity;
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
        
        protected interface IBindableOnTo<TEvent, TEntity, TEntityAction>
            where TEvent : Event
            where TEntity : IEntity
            where TEntityAction : IEventEntityAction<TEntity>
        {
        }
        
        #endregion
        
        #region Implementations
        
        private class DefaultBindable<T> : IBindable<T> where T : Event
        {
            private readonly StaticEventBinder m_StaticEventBinder;
            
            public DefaultBindable(StaticEventBinder staticEventBinder)
            {
                this.m_StaticEventBinder = staticEventBinder;
            }

            public IBindableTo<T, TAction> To<TAction>() where TAction : IEventAction
            {
                var bindable = new DefaultBindableTo<T, TAction>(
                    this.m_StaticEventBinder);
                bindable.BindAsAction<TAction>();
                return bindable;
            }

            public IBindableTo<T, TListener> ToListener<TListener>() where TListener : IEventListener
            {
                var bindable = new DefaultBindableTo<T, TListener>(
                    this.m_StaticEventBinder);
                bindable.BindAsListener<TListener>();
                return bindable;
            }

            public IBindableTo<T, TCommand> ToCommand<TCommand>(params string[] arguments) where TCommand : ICommand
            {
                var bindable = new DefaultBindableTo<T, TCommand>(
                    this.m_StaticEventBinder);
                bindable.BindAsCommand<TCommand>(arguments);
                return bindable;
            }

            public IBindableOn<T, TEntity> On<TEntity>() where TEntity : IEntity
            {
                return new DefaultBindableOn<T, TEntity>(
                    this.m_StaticEventBinder);
            }
        }
        
        private class DefaultBindableTo<TEvent, TTarget> : IBindableTo<TEvent, TTarget>
            where TEvent : Event
        {
            private readonly StaticEventBinder m_StaticEventBinder;
            
            public DefaultBindableTo(StaticEventBinder staticEventBinder)
            {
                this.m_StaticEventBinder = staticEventBinder;
            }
            
            public void BindAsAction<TAction>() where TAction : IEventAction
            {
                this.m_StaticEventBinder.m_Bindings.Add((gameContext, eventEngine, @event) =>
                {
                    var action = this.m_StaticEventBinder.m_Kernel.Get<TAction>();
                    action.Handle(gameContext, @event);
                    return true;
                });
            }
            
            public void BindAsListener<TListener>() where TListener : IEventListener
            {
                this.m_StaticEventBinder.m_Bindings.Add((gameContext, eventEngine, @event) =>
                {
                    var listener = this.m_StaticEventBinder.m_Kernel.Get<TListener>();
                    return listener.Handle(gameContext, eventEngine, @event);
                });
            }
            
            public void BindAsCommand<TCommand>(params string[] parameters) where TCommand : ICommand
            {
                this.m_StaticEventBinder.m_Bindings.Add((gameContext, eventEngine, @event) =>
                {
                    var command = this.m_StaticEventBinder.m_Kernel.Get<TCommand>();
                    command.Execute(gameContext, "", parameters);
                    return true;
                });
            }
        }
        
        private class DefaultBindableOn<TEvent, TEntity> : IBindableOn<TEvent, TEntity>
            where TEvent : Event
            where TEntity : IEntity
        {
            private readonly StaticEventBinder m_StaticEventBinder;
            
            public DefaultBindableOn(StaticEventBinder staticEventBinder)
            {
                this.m_StaticEventBinder = staticEventBinder;
            }
            
            public IBindableOnTo<TEvent, TEntity, TEntityAction> To<TEntityAction>()
                where TEntityAction : IEventEntityAction<TEntity>
            {
                var bindable = new DefaultBindableOnTo<TEvent, TEntity, TEntityAction>(
                    this.m_StaticEventBinder);
                bindable.Bind();
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
            
            public DefaultBindableOnTo(StaticEventBinder staticEventBinder)
            {
                this.m_StaticEventBinder = staticEventBinder;
            }
            
            public void Bind()
            {
                this.m_StaticEventBinder.m_Bindings.Add((gameContext, eventEngine, @event) =>
                {
                    var action = this.m_StaticEventBinder.m_Kernel.Get<TEntityAction>();
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
        
        #endregion
        
        #endregion
    }
}

