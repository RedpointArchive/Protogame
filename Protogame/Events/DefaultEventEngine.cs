using System.Collections.Generic;
using System.Linq;
using Ninject.Syntax;

namespace Protogame
{
    public class DefaultEventEngine : IEventEngine
    {
        private readonly IEventBinder[] m_EventBinders;
    
        public DefaultEventEngine(
            IResolutionRoot resolutionRoot,
            IEventBinder[] eventBinders)
        {
            this.m_EventBinders = eventBinders;
            foreach (var eventBinder in this.m_EventBinders)
            {
                eventBinder.Assign(resolutionRoot);
            }
        }
        
        public void Fire(IGameContext gameContext, Event @event)
        {
            foreach (var eventBinder in this.m_EventBinders.OrderByDescending(x => x.Priority))
            {
                if (eventBinder.Handle(gameContext, this, @event))
                    break;
            }
        }
    }
}

