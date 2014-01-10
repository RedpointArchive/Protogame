namespace Protogame
{
    /// <summary>
    /// Represents an event engine.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of contextual data to be associated when event are fired.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// The event engine infrastructure provides a way to fire and handle events throughout the game.  Where possible,
    /// you should use the event engine instead of checking the hardware directly.
    /// </para>
    /// <para>
    /// In particular, the event engine allows you to ensure that only one object handles a given event, whereas checking
    /// the hardware directly provides no such support.  In addition, single time events such as key presses (as opposed to
    /// the key being held down) are supported.
    /// </para>
    /// <para>
    /// To register event bindings (that is, map fired event types to handlers), you should derive a class from either
    /// <see cref="StaticEventBinder&lt;TContext&gt;"/> or implement the <see cref="IEventBinder&lt;TContext&gt;"/> interface.
    /// It is recommended that you derive from <see cref="StaticEventBinder&lt;TContext&gt;"/> as this provides a simple
    /// and fluent mechanism for binding events.
    /// </para>
    /// <para>
    /// To activate the event bindings, register the type against the dependency injection kernel.  Most commonly, you will
    /// write code such as:
    /// <code>
    /// this.Bind&lt;IEventBinder&lt;IGameContext&gt;&gt;().To&lt;MyDefaultBinder&gt;();
    /// </code>
    /// within the appropriate dependency injection module.
    /// </para>
    /// <para>
    /// You can also use the event engine to fire your own types of events.  Derive a class from the <see cref="Event"/> class
    /// and provide the relevant information against the derived class.  You can then bind or handle that specific event type
    /// within your event binder.
    /// </para>
    /// </remarks>
    /// <seealso cref="StaticEventBinder&lt;TContext&gt;"/>
    /// <seealso cref="IEventBinder&lt;TContext&gt;"/>
    public interface IEventEngine<in TContext>
    {
        /// <summary>
        /// Fires an event in the event engine, calling upon the registered event binders to handle the event.
        /// </summary>
        /// <param name="context">
        /// The context of the event.
        /// </param>
        /// <param name="event">
        /// The event itself.
        /// </param>
        void Fire(TContext context, Event @event);
    }
}