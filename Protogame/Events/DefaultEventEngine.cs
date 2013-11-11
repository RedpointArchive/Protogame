using System.Linq;
using Ninject.Syntax;

namespace Protogame
{
    public class DefaultEventEngine<TContext> : IEventEngine<TContext>
    {
        private readonly IEventBinder<TContext>[] m_EventBinders;
    
        public DefaultEventEngine(
            IResolutionRoot resolutionRoot,
            IEventBinder<TContext>[] eventBinders)
        {
            this.m_EventBinders = eventBinders;
            foreach (var eventBinder in this.m_EventBinders)
            {
                eventBinder.Assign(resolutionRoot);
            }
        }
        
        public void Fire(TContext context, Event @event)
        {
            foreach (var eventBinder in this.m_EventBinders.OrderByDescending(x => x.Priority))
            {
                if (eventBinder.Handle(context, this, @event))
                    break;
            }
        }
    }
}

