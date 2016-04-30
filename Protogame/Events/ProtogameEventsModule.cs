using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    using Protoinject;

    /// <summary>
    /// The event management module, which provides functionality for translating
    /// input data into events and propagating it through an event system.  While
    /// you can still use the conventional <see cref="Mouse"/>, <see cref="Keyboard"/>,
    /// <see cref="GamePad"/> and <see cref="Joystick"/> APIs from XNA, events allow
    /// you to handle scenarios where input should be consumed by an object and not
    /// propagated to any others.
    /// </summary>
    /// <module>Events</module>
    public class ProtogameEventsModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind<IEventEngine<IGameContext>>().To<DefaultEventEngine<IGameContext>>().InSingletonScope();
            kernel.Bind<IEngineHook>().To<EventEngineHook>();
            kernel.Bind<IEventBinder<IGameContext>>().To<ConsoleEventBinder>();
            kernel.Bind<IEventBinder<IGameContext>>().To<UIEventBinder>();
        }
    }
}