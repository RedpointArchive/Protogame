namespace Protogame
{
    /// <summary>
    /// An interface which should be implemented to initialize an analytics engine.
    /// </summary>
    /// <remarks>
    /// You should implement this class in your game to initialize the analytics
    /// engine using it's InitializeKeys and InitializeSession methods.  The
    /// Initialize method on this interface will be called by CoreGame or ServerGame
    /// when it is constructed.
    /// </remarks>
    public interface IAnalyticsInitializer
    {
        void Initialize(IAnalyticsEngine engine);
    }
}

