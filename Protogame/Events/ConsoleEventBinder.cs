namespace Protogame
{
    public class ConsoleEventBinder : StaticEventBinder<IGameContext>
    {
        public override void Configure()
        {
            this.Bind<Event>(x => true).ToListener<ConsoleEventListener>();
        }
    }
}

