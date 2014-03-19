namespace Protogame
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// An interface that describes an analytics engine.
    /// </summary>
    /// <remarks>
    /// It is important that the analytics engine implementation buffers events before sending
    /// them to a remote server.  This is because the number of generated events can be quite
    /// large and because calling the interface's methods should be an asynchronous operation
    /// (with the actual data send performed in the background).
    /// </remarks>
    public interface IAnalyticsEngine
    {
        /// <summary>
        /// Initialize the keys used to contact the analytics service.
        /// </summary>
        /// <param name="publicKey">The public (or game) key.</param>
        /// <param name="secretKey">The secret key.</param>
        void InitializeKeys(string publicKey, string secretKey);

        /// <summary>
        /// Initialize the analytics engine with the required session information.
        /// </summary>
        /// <param name="userId">
        /// A unique identifier for the user; it should remain the same across play
        /// sessions.  Pass null for auto-generated of user ID based on network MAC address.
        /// </param>
        /// <param name="sessionId">
        /// A unique identifier for the session; it should change across play
        /// sessions.  Pass null for auto-generated of session ID based 
        /// on network MAC address and current time.
        /// </param>
        /// <param name="buildHash">
        /// A unique identifier for this build, usually derived from 
        /// source control information.  Pass null for "Not Specified".
        /// </param>
        void InitializeSession(string userId, string sessionId, string buildHash);

        /// <summary>
        /// Logs a gameplay or design event with the analytics service.
        /// </summary>
        /// <remarks>
        /// The event ID will depend on the analytics service.
        /// </remarks>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="value">The optional numeric value to attach to the event.</param>
        /// <param name="area">The optional area or level name information.</param>
        /// <param name="position">The optional position of the event.</param>
        void LogGameplayEvent(string eventId, float? value = null, string area = null, Vector3? position = null);

        /// <summary>
        /// Logs an error event with the analytics service.
        /// </summary>
        /// <param name="message">The error or exception message.</param>
        /// <param name="severity">The severity of the event.</param>
        /// <param name="area">The optional area or level name information.</param>
        /// <param name="position">The optional position of the event.</param>
        void LogErrorEvent(string message, AnalyticsErrorSeverity severity, string area = null, Vector3? position = null);

        /// <summary>
        /// Logs additional user information with the game analytics service.
        /// </summary>
        /// <param name="userInfo">
        /// The user information structure to pass back to the service.
        /// </param>
        /// <remarks>
        /// This should generally only need to be called once depending on the game analytics service.  This
        /// information is tied with the user ID specified during initialization.
        /// </remarks>
        void LogUserInformation(Dictionary<string, object> userInfo);

        /// <summary>
        /// Causes the analytics engine to flush all events to the server and stop.
        /// </summary>
        /// <remarks>
        /// This is called by CoreGame or CoreServer before the game exits to ensure everything
        /// has been sent to the server.
        /// </remarks>
        void FlushAndStop();
    }
}

