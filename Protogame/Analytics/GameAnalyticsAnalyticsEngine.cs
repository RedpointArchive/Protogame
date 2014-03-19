namespace Protogame
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using Microsoft.Xna.Framework;
    using Newtonsoft.Json;

    public class GameAnalyticsAnalyticsEngine : IAnalyticsEngine
    {
        private bool m_KeysInitialized;

        private bool m_SessionInitialized;

        private string m_GameKey;

        private string m_SecretKey;

        private string m_UserId;

        private string m_SessionId;

        private string m_BuildHash;

        private Thread m_EventPostingThread;

        private ConcurrentQueue<dynamic> m_PendingGameplayEvents;

        public GameAnalyticsAnalyticsEngine()
        {
            this.m_PendingGameplayEvents = new ConcurrentQueue<dynamic>();
        }

        public void InitializeKeys(string publicKey, string secretKey)
        {
            this.m_GameKey = publicKey;
            this.m_SecretKey = secretKey;
            this.m_KeysInitialized = true;
        }

        public void InitializeSession(string userId, string sessionId, string buildHash)
        {
            this.m_UserId = userId ?? this.AutogenerateUserId();
            this.m_SessionId = sessionId ?? this.AutogenerateSessionId(this.m_UserId);
            this.m_BuildHash = buildHash ?? "Not Specified";
            this.m_SessionInitialized = true;
        }

        public void LogGameplayEvent(string eventId, float? value, string area, Vector3? position)
        {
            this.AssertInitialized();
        }

        public void LogErrorEvent(string message, AnalyticsErrorSeverity severity, string area, Vector3? position)
        {
            if (!this.m_KeysInitialized || !this.m_SessionInitialized)
            {
                // We don't throw an exception here because we may be reporting a crash.
                return;
            }
        }

        public void LogUserInformation(dynamic userInfo)
        {
            this.AssertInitialized();
        }

        private void AssertInitialized()
        {
            if (!this.m_KeysInitialized || !this.m_SessionInitialized)
            {
                throw new InvalidOperationException(
                    "The Game Analytics keys or session have not been initialized.  " +
                    "Implement IAnalyticsInitializer and bind it to the game kernel" +
                    " to initialize the analytics service.");
            }
        }

        private string AutogenerateUserId()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in nics)
            {
                var address = adapter.GetPhysicalAddress();
                if (address.ToString() != "")
                {
                    var bytes = Encoding.UTF8.GetBytes(address.ToString());
                    var SHA = new SHA1CryptoServiceProvider();
                    return BitConverter.ToString(SHA.ComputeHash(bytes)).Replace("-", "");
                }
            }

            return "No MAC information for user ID generation";
        }

        private string AutogenerateSessionId(string userId)
        {
            var timeBytes = BitConverter.GetBytes(DateTime.Now.ToBinary());
            var userBytes = Encoding.ASCII.GetBytes(userId);
            var bytes = timeBytes.Concat(userBytes).ToArray();

            var SHA = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(SHA.ComputeHash(bytes)).Replace("-", "");
        }

        private void EnsureRunning()
        {
            if (this.m_EventPostingThread != null)
            {
                return;
            }

            this.m_EventPostingThread = new Thread(this.Run);
            this.m_EventPostingThread.IsBackground = true;
            this.m_EventPostingThread.Name = "Game Analytics Posting Thread";
            this.m_EventPostingThread.Start();
        }

        private void Run()
        {
            while (true)
            {
                List<dynamic> pendingMessages = new List<dynamic>();
                dynamic result;

                // Take as many messages as possible.
                while (this.m_PendingGameplayEvents.TryDequeue(out result))
                {
                    pendingMessages.Add(result);
                }

                if (pendingMessages.Count == 0)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                var messageString = JsonConvert.SerializeObject(pendingMessages);

                Console.WriteLine(messageString);

                Thread.Sleep(1000);
            }
        }
    }
}

