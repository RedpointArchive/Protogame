using Ninject;
using Xunit;

namespace Protogame.Tests
{
    public class EventEngineTests
    {
        private class BasicEvent : Event
        {
            public int Original { get; set; }
            public int HitValue { get; set; }
        }
        
        private class BasicAction : IEventAction
        {
            public void Handle(IGameContext gameContext, Event @event)
            {
                var basicEvent = @event as BasicEvent;
                basicEvent.HitValue = basicEvent.Original;
            }
        }
        
        private class BasicStaticEventBinder : StaticEventBinder
        {
            public BasicStaticEventBinder(IKernel kernel) : base(kernel) { }
            
            public override void Configure()
            {
                this.Bind<BasicEvent>(x => true).To<BasicAction>();
            }
        }
        
        private class FilteredStaticEventBinder : StaticEventBinder
        {
            public FilteredStaticEventBinder(IKernel kernel) : base(kernel) { }
            
            public override void Configure()
            {
                this.Bind<BasicEvent>(x => x.Original == 1).To<BasicAction>();
            }
        }
    
        [Fact]
        public void TestBasicPropagation()
        {
            var @event = new BasicEvent { Original = 1 };
            var binder = new BasicStaticEventBinder(new StandardKernel());
            var engine = new DefaultEventEngine(new[] { binder });
            engine.Fire(null, @event);
            Assert.Equal(1, @event.HitValue);
        }
    
        [Fact]
        public void TestFilteredPropagation()
        {
            var event1 = new BasicEvent { Original = 1 };
            var event2 = new BasicEvent { Original = 1 };
            var binder = new BasicStaticEventBinder(new StandardKernel());
            var engine = new DefaultEventEngine(new[] { binder });
            engine.Fire(null, event1);
            Assert.Equal(1, event1.HitValue);
            engine.Fire(null, event2);
            Assert.NotEqual(2, event2.HitValue);
        }
    }
}

