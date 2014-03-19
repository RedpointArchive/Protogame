namespace Protogame
{
    /// <summary>
    /// An analytics engine that does no reporting.
    /// </summary>
    /// <remarks>
    /// The CoreGame implementation will bind <see cref="IAnalyticsEngine"/> to
    /// this implementation if there is no other engine bound already.  This is 
    /// because several areas of Protogame automatically generate and report
    /// events through the analytics engine, and therefore an implementation
    /// of <see cref="IAnalyticsEngine"/> is always required.
    /// </remarks>
    public class NullAnalyticsInitializer : IAnalyticsInitializer
    {
        public void Initialize(IAnalyticsEngine engine)
        {
        }
    }
}

