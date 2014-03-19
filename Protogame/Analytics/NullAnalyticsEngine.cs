namespace Protogame
{
    using System.Collections.Generic;

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
    public class NullAnalyticsEngine : IAnalyticsEngine
    {
        public void InitializeKeys(string publicKey, string secretKey)
        {
        }

        public void InitializeSession(string userId, string sessionId, string buildHash)
        {
        }

        public void LogGameplayEvent(string eventId, float? value, string area, Microsoft.Xna.Framework.Vector3? position)
        {
        }

        public void LogErrorEvent(string message, AnalyticsErrorSeverity severity, string area, Microsoft.Xna.Framework.Vector3? position)
        {
        }

        public void LogUserInformation(Dictionary<string, object> userInfo)
        {
        }

        public void FlushAndStop()
        {
        }
    }
}

