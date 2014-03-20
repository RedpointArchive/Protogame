namespace Protogame
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using Microsoft.Xna.Framework;
    using Newtonsoft.Json;

    /// <summary>
    /// The Game Analytics (http://www.gameanalytics.com) analytics engine.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1650:ElementDocumentationMustBeSpelledCorrectly", 
        Justification = "Reviewed. Suppression is OK here.")]
    public class GameAnalyticsAnalyticsEngine : IAnalyticsEngine
    {
        /// <summary>
        /// The endpoint URI for Game Analytics.
        /// </summary>
        private const string EndpointUri = "http://api.gameanalytics.com/1";

        /// <summary>
        /// The queue of game events that still need to be sent to the server.
        /// </summary>
        private readonly ConcurrentQueue<Dictionary<string, object>> m_PendingGameplayEvents;

        /// <summary>
        /// The queue of game errors that still need to be sent to the server.
        /// </summary>
        private readonly ConcurrentQueue<Dictionary<string, object>> m_PendingGameplayErrors;

        /// <summary>
        /// The build hash.
        /// </summary>
        private string m_BuildHash;

        /// <summary>
        /// The thread that is used to post events back to Game Analytics.
        /// </summary>
        private Thread m_EventPostingThread;

        /// <summary>
        /// The game key.
        /// </summary>
        private string m_GameKey;

        /// <summary>
        /// Whether or not the game and secret keys have been specified.
        /// </summary>
        private bool m_KeysInitialized;

        /// <summary>
        /// The secret key.
        /// </summary>
        private string m_SecretKey;

        /// <summary>
        /// The session ID.
        /// </summary>
        private string m_SessionId;

        /// <summary>
        /// Whether or not the user ID, session ID and build hash have been specified.
        /// </summary>
        private bool m_SessionInitialized;

        /// <summary>
        /// The user ID.
        /// </summary>
        private string m_UserId;

        /// <summary>
        /// Whether or not the thread is stopping.
        /// </summary>
        private bool m_Stopping;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameAnalyticsAnalyticsEngine"/> class.
        /// </summary>
        public GameAnalyticsAnalyticsEngine()
        {
            this.m_PendingGameplayEvents = new ConcurrentQueue<Dictionary<string, object>>();
            this.m_PendingGameplayErrors = new ConcurrentQueue<Dictionary<string, object>>();
            this.m_Stopping = false;
        }

        /// <summary>
        /// Initialize the keys used to contact the analytics service.
        /// </summary>
        /// <param name="publicKey">
        /// The public (or game) key.
        /// </param>
        /// <param name="secretKey">
        /// The secret key.
        /// </param>
        public void InitializeKeys(string publicKey, string secretKey)
        {
            this.m_GameKey = publicKey;
            this.m_SecretKey = secretKey;
            this.m_KeysInitialized = true;
        }

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
        public void InitializeSession(string userId, string sessionId, string buildHash)
        {
            this.m_UserId = userId ?? this.AutogenerateUserId();
            this.m_SessionId = sessionId ?? this.AutogenerateSessionId(this.m_UserId);
            this.m_BuildHash = buildHash ?? "Not Specified";
            this.m_SessionInitialized = true;
        }

        /// <summary>
        /// Logs an error event with the analytics service.
        /// </summary>
        /// <param name="message">
        /// The error or exception message.
        /// </param>
        /// <param name="severity">
        /// The severity of the event.
        /// </param>
        /// <param name="area">
        /// The optional area or level name information.
        /// </param>
        /// <param name="position">
        /// The optional position of the event.
        /// </param>
        public void LogErrorEvent(string message, AnalyticsErrorSeverity severity, string area, Vector3? position)
        {
            if (!this.m_KeysInitialized || !this.m_SessionInitialized)
            {
                // We don't throw an exception here because we may be reporting a crash.
            }

            // If we're not already stopping; this method might be called outside the game
            // loop in which case we can't do it via a thread.
            if (!this.m_Stopping)
            {
                this.EnsureRunning();
            }

            var values = new Dictionary<string, object>();
            values.Add("user_id", this.m_UserId);
            values.Add("session_id", this.m_SessionId);
            values.Add("build", this.m_BuildHash);
            values.Add("message", message);

            switch (severity)
            {
                case AnalyticsErrorSeverity.Critical:
                    values.Add("severity", "critical");
                    break;
                case AnalyticsErrorSeverity.Error:
                    values.Add("severity", "error");
                    break;
                case AnalyticsErrorSeverity.Warning:
                    values.Add("severity", "warning");
                    break;
                case AnalyticsErrorSeverity.Info:
                    values.Add("severity", "info");
                    break;
                case AnalyticsErrorSeverity.Debug:
                    values.Add("severity", "debug");
                    break;
            }

            if (area != null)
            {
                values.Add("area", area);
            }

            if (position != null)
            {
                values.Add("x", position.Value.X);
                values.Add("y", position.Value.Y);
                values.Add("z", position.Value.Z);
            }

            this.m_PendingGameplayErrors.Enqueue(values);

            // If we are in the process of stopping, run the submission ourselves.
            if (this.m_Stopping)
            {
                this.Run();
            }
        }

        /// <summary>
        /// Logs a gameplay or design event with the analytics service.
        /// </summary>
        /// <remarks>
        /// The event ID will depend on the analytics service.
        /// </remarks>
        /// <param name="eventId">
        /// The event identifier.
        /// </param>
        /// <param name="value">
        /// The optional numeric value to attach to the event.
        /// </param>
        /// <param name="area">
        /// The optional area or level name information.
        /// </param>
        /// <param name="position">
        /// The optional position of the event.
        /// </param>
        public void LogGameplayEvent(string eventId, float? value, string area, Vector3? position)
        {
            this.EnsureRunning();
            this.AssertInitialized();

            var values = new Dictionary<string, object>();
            values.Add("user_id", this.m_UserId);
            values.Add("session_id", this.m_SessionId);
            values.Add("build", this.m_BuildHash);
            values.Add("event_id", eventId);

            if (value != null)
            {
                values.Add("value", value.Value);
            }

            if (area != null)
            {
                values.Add("area", area);
            }

            if (position != null)
            {
                values.Add("x", position.Value.X);
                values.Add("y", position.Value.Y);
                values.Add("z", position.Value.Z);
            }

            this.m_PendingGameplayEvents.Enqueue(values);
        }

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
        public void LogUserInformation(Dictionary<string, object> userInfo)
        {
            this.AssertInitialized();
        }

        /// <summary>
        /// Causes the analytics engine to flush all events to the server and stop.
        /// </summary>
        /// <remarks>
        /// This is called by CoreGame or CoreServer before the game exits to ensure everything
        /// has been sent to the server.
        /// </remarks>
        public void FlushAndStop()
        {
            this.m_Stopping = true;

            this.m_EventPostingThread.Join();
        }

        /// <summary>
        /// Assert that the keys and session information have been initialized correctly.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if either InitializeKeys or InitializeSession have not been called yet.
        /// </exception>
        private void AssertInitialized()
        {
            if (!this.m_KeysInitialized || !this.m_SessionInitialized)
            {
                throw new InvalidOperationException(
                    "The Game Analytics keys or session have not been initialized.  "
                    + "Implement IAnalyticsInitializer and bind it to the game kernel"
                    + " to initialize the analytics service.");
            }
        }

        /// <summary>
        /// Auto-generate a session ID.
        /// </summary>
        /// <param name="userId">
        /// The user ID.
        /// </param>
        /// <returns>
        /// The generated session ID.
        /// </returns>
        private string AutogenerateSessionId(string userId)
        {
            var timeBytes = BitConverter.GetBytes(DateTime.Now.ToBinary());
            var userBytes = Encoding.ASCII.GetBytes(userId);
            var bytes = timeBytes.Concat(userBytes).ToArray();

            var sha = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", string.Empty);
        }

        /// <summary>
        /// Auto-generate a user ID.
        /// </summary>
        /// <returns>
        /// The generated user ID.
        /// </returns>
        private string AutogenerateUserId()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var adapter in nics)
            {
                var address = adapter.GetPhysicalAddress();
                if (address.ToString() != string.Empty)
                {
                    var bytes = Encoding.UTF8.GetBytes(address.ToString());
                    var sha = new SHA1CryptoServiceProvider();
                    return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", string.Empty);
                }
            }

            return "No MAC information for user ID generation";
        }

        /// <summary>
        /// Ensure the thread has been started and start it if it's not running.
        /// </summary>
        private void EnsureRunning()
        {
            if (this.m_EventPostingThread != null)
            {
                return;
            }

            this.m_EventPostingThread = new Thread(this.Run) { Name = "Game Analytics Posting Thread" };
            this.m_EventPostingThread.Start();
        }

        /// <summary>
        /// The main thread that is used to post events back to the server.
        /// </summary>
        private void Run()
        {
            while (!this.m_Stopping || this.m_PendingGameplayEvents.Count > 0 || this.m_PendingGameplayErrors.Count > 0)
            {
                var pendingGameplayEvents = new List<Dictionary<string, object>>();
                var pendingGameplayErrors = new List<Dictionary<string, object>>();
                Dictionary<string, object> result;

                // Take as many messages as possible.
                while (this.m_PendingGameplayEvents.TryDequeue(out result))
                {
                    pendingGameplayEvents.Add(result);
                }

                while (this.m_PendingGameplayErrors.TryDequeue(out result))
                {
                    pendingGameplayErrors.Add(result);
                }

                Action<string, List<Dictionary<string, object>>> submit = (type, dict) =>
                {
                    var messageString = JsonConvert.SerializeObject(dict);

                    var md5 = new MD5CryptoServiceProvider();
                    var authData = Encoding.Default.GetBytes(messageString + this.m_SecretKey);
                    var authHash = md5.ComputeHash(authData);
                    var hexaHash = authHash.Aggregate(
                        string.Empty,
                        (current, b) => current + string.Format("{0:x2}", b));
                    var jsonData = Encoding.ASCII.GetBytes(messageString);

                    var url = EndpointUri + "/" + this.m_GameKey + "/" + type;
                    var request = WebRequest.Create(url);

                    request.Headers.Add("Authorization", hexaHash);
                    request.Method = "POST";
                    request.ContentLength = jsonData.Length;
                    request.ContentType = "application/x-www-form-urlencoded";

                    try
                    {
                        // Send the json data
                        var dataStream = request.GetRequestStream();
                        dataStream.Write(jsonData, 0, jsonData.Length);
                        dataStream.Close();

                        // Get the response
                        var response = request.GetResponse();
                        var responseData = response.GetResponseStream();

                        if (responseData != null)
                        {
                            using (var reader = new StreamReader(responseData))
                            {
                                var resultString = reader.ReadToEnd();
                                var resultObj = JsonConvert.DeserializeObject<dynamic>(resultString);

                                if (resultObj["status"] != "ok")
                                {
                                    Console.WriteLine(
                                        "WARNING: Some gameplay events were rejected by the Game Analytics server.");
                                    Console.WriteLine(resultString);
                                }
                            }
                        }
                    }
                    catch (WebException)
                    {
                        Console.WriteLine("WARNING: Unable to send data to the Game Analytics server.");
                    }
                    catch (SocketException)
                    {
                        Console.WriteLine("WARNING: Unable to send data to the Game Analytics server.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("WARNING: Internal error while sending data to Game Analytics server.");
                        Console.WriteLine(ex);
                    }
                };

                if (pendingGameplayEvents.Count > 0)
                {
                    submit("design", pendingGameplayEvents);
                }

                if (pendingGameplayErrors.Count > 0)
                {
                    submit("error", pendingGameplayErrors);
                }

                if (!this.m_Stopping)
                {
                    Thread.Sleep(1000);
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }
}