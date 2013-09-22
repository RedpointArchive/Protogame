using System.Collections.Generic;
using System.Linq;

namespace Protogame
{
    public class DefaultEventEngine : IEventEngine
    {
        private readonly IEnumerable<IEventBinder> m_EventBinders;
    
        public DefaultEventEngine(
            IEnumerable<IEventBinder> eventBinders)
        {
            this.m_EventBinders = eventBinders;
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

