namespace Protogame
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// The Mx dispatcher; this handles receiving messages on the UDP client and dispatching
    /// them to the correct connected Mx client.
    /// </summary>
    public class MxDispatcher
    {
        /// <summary>
        /// A list of currently connected Mx clients.
        /// </summary>
        private readonly Dictionary<IPEndPoint, MxClient> m_MxClients;

        /// <summary>
        /// The UDP client that messages will be received on.
        /// </summary>
        private readonly UdpClient m_UdpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MxDispatcher"/> class.
        /// </summary>
        /// <param name="port">
        /// The port of the UDP client.
        /// </param>
        public MxDispatcher(int port)
        {
            this.m_UdpClient = new UdpClient(port) { Client = { Blocking = false } };
            this.m_MxClients = new Dictionary<IPEndPoint, MxClient>();
        }

        /// <summary>
        /// Raised when an Mx client connects.
        /// </summary>
        public event MxClientEventHandler ClientConnected;

        /// <summary>
        /// Raised when an Mx client could potentially disconnect.
        /// </summary>
        public event MxDisconnectEventHandler ClientDisconnectWarning;

        /// <summary>
        /// Raised when an Mx client disconnects.
        /// </summary>
        public event MxClientEventHandler ClientDisconnected;

        /// <summary>
        /// Raised when a message has been acknowledged by the remote endpoint.
        /// </summary>
        public event MxMessageEventHandler MessageAcknowledged;

        /// <summary>
        /// Raised when a message was not acknowledged by a client.
        /// </summary>
        public event MxMessageEventHandler MessageLost;

        /// <summary>
        /// Raised when a message has been received.
        /// </summary>
        public event MxMessageEventHandler MessageReceived;

        /// <summary>
        /// Raised when a message has been sent by a client.
        /// </summary>
        public event MxMessageEventHandler MessageSent;

        /// <summary>
        /// Gets an enumeration of the endpoints of all connected clients.
        /// </summary>
        /// <value>
        /// An enumeration of the endpoints of all connected clients.
        /// </value>
        public IEnumerable<IPEndPoint> Endpoints
        {
            get
            {
                return this.m_MxClients.Select(x => x.Key).ToArray();
            }
        }

        /// <summary>
        /// Closes the dispatcher permanently, terminating all inbound and outbound connections.
        /// </summary>
        public void Close()
        {
            this.m_UdpClient.Close();
        }

        /// <summary>
        /// Explicitly connect to the specified endpoint, assuming there is an Mx dispatcher
        /// at the specified address.
        /// <para>
        /// This method is used to explicitly add clients, not to start the dispatcher.  The
        /// dispatcher does not require an explicit start.
        /// </para>
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint to connect to.
        /// </param>
        public void Connect(IPEndPoint endpoint)
        {
            this.m_MxClients[endpoint] = new MxClient(this, endpoint, this.m_UdpClient);
            this.OnClientConnected(this.m_MxClients[endpoint]);
            this.RegisterForEvents(this.m_MxClients[endpoint]);
        }

        /// <summary>
        /// Disconnect the current client from the specified endpoint.  This method is automatically
        /// called by the Mx client when it detects that it can not send any further messages.
        /// <para>
        /// There is generally no reason to call this method, unless you are sure that the remote
        /// machine will no longer send any more messages.  If the remote machine sends more messages
        /// after calling this method, then the remote machine will automatically reconnect.
        /// </para>
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint to disconnect from.
        /// </param>
        public void Disconnect(IPEndPoint endpoint)
        {
            var client = this.m_MxClients[endpoint];
            this.UnregisterFromEvents(client);
            this.m_MxClients.Remove(endpoint);
            this.OnClientDisconnected(client);
        }

        /// <summary>
        /// Queue a packet for sending to the specified endpoint.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint of the client to send to.
        /// </param>
        /// <param name="packet">
        /// The associated data to send.
        /// </param>
        public void Send(IPEndPoint endpoint, byte[] packet)
        {
            var client = this.m_MxClients[endpoint];
            client.EnqueueSend(packet);
        }

        /// <summary>
        /// Updates the dispatcher, receiving messages and connecting clients as appropriate.
        /// </summary>
        public void Update()
        {
            // Receive as many messages from the connection as we possibly
            // can, and dispatch them to the correct MxClient.
            while (true)
            {
                var receive = (IPEndPoint)null;
                var packet = this.ReceiveNonBlocking(this.m_UdpClient, ref receive);
                if (packet == null)
                {
                    break;
                }

                if (this.m_MxClients.ContainsKey(receive))
                {
                    // Dispatch to an existing client.
                    this.m_MxClients[receive].EnqueueReceive(packet);
                }
                else
                {
                    // Create a new client for this address.
                    this.m_MxClients[receive] = new MxClient(this, receive, this.m_UdpClient);
                    this.RegisterForEvents(this.m_MxClients[receive]);
                    this.m_MxClients[receive].EnqueueReceive(packet);
                    this.OnClientConnected(this.m_MxClients[receive]);
                }
            }

            // Now update all of the clients.
            foreach (var client in this.m_MxClients.Values.ToArray())
            {
                client.Update();
            }
        }

        /// <summary>
        /// Raise the ClientConnected event.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        protected virtual void OnClientConnected(MxClient client)
        {
            var handler = this.ClientConnected;
            if (handler != null)
            {
                handler(this, new MxClientEventArgs { Client = client });
            }
        }

        /// <summary>
        /// Raise the ClientDisconnectWarning event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnClientDisconnectWarning(MxDisconnectEventArgs e)
        {
            var handler = this.ClientDisconnectWarning;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raise the ClientDisconnected event.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        protected virtual void OnClientDisconnected(MxClient client)
        {
            var handler = this.ClientDisconnected;
            if (handler != null)
            {
                handler(this, new MxClientEventArgs { Client = client });
            }
        }

        /// <summary>
        /// Raise the MessageAcknowledged event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnMessageAcknowledged(MxMessageEventArgs e)
        {
            var handler = this.MessageAcknowledged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raise the MessageLost event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnMessageLost(MxMessageEventArgs e)
        {
            var handler = this.MessageLost;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raise the MessageReceived event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnMessageReceived(MxMessageEventArgs e)
        {
            var handler = this.MessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raise the MessageSent event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnMessageSent(MxMessageEventArgs e)
        {
            var handler = this.MessageSent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Handle receiving a DisconnectWarning event from a client.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnClientDisconnectWarning(object sender, MxDisconnectEventArgs e)
        {
            this.OnClientDisconnectWarning(e);
        }

        /// <summary>
        /// Handle receiving a MessageAcknowledged event from a client.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnClientMessageAcknowledged(object sender, MxMessageEventArgs e)
        {
            this.OnMessageAcknowledged(e);
        }

        /// <summary>
        /// Handle receiving a MessageLost event from a client.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnClientMessageLost(object sender, MxMessageEventArgs e)
        {
            this.OnMessageLost(e);
        }

        /// <summary>
        /// Handle receiving a MessageReceived event from a client.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnClientMessageReceived(object sender, MxMessageEventArgs e)
        {
            this.OnMessageReceived(e);
        }

        /// <summary>
        /// Handle receiving a MessageSent event from a client.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnClientMessageSent(object sender, MxMessageEventArgs e)
        {
            this.OnMessageSent(e);
        }

        /// <summary>
        /// Receive a message in an non-blocking manner.  If there is no message to
        /// receive, this function returns null.
        /// </summary>
        /// <param name="client">
        /// The client to receive the message from.
        /// </param>
        /// <param name="receive">
        /// The endpoint that the message was received from.
        /// </param>
        /// <returns>
        /// The message as an array of <see cref="byte"/>, or null if there's no message.
        /// </returns>
        private byte[] ReceiveNonBlocking(UdpClient client, ref IPEndPoint receive)
        {
            if (client.Available == 0)
            {
                return null;
            }

            try
            {
                return client.Receive(ref receive);
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.WouldBlock)
                {
                    return null;
                }

                if (ex.SocketErrorCode == SocketError.ConnectionReset)
                {
                    // This probably implies we're about to disconnect (because the remote
                    // computer is no longer accepting requests), but let the Mx client time
                    // out since there may be other valid clients still.
                    return null;
                }

                throw;
            }
        }

        /// <summary>
        /// Register for a client's events.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        private void RegisterForEvents(MxClient client)
        {
            client.MessageSent += this.OnClientMessageSent;
            client.MessageReceived += this.OnClientMessageReceived;
            client.MessageLost += this.OnClientMessageLost;
            client.MessageAcknowledged += this.OnClientMessageAcknowledged;
            client.DisconnectWarning += this.OnClientDisconnectWarning;
        }

        /// <summary>
        /// Unregister from a client's events.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        private void UnregisterFromEvents(MxClient client)
        {
            client.MessageSent -= this.OnClientMessageSent;
            client.MessageReceived -= this.OnClientMessageReceived;
            client.MessageLost -= this.OnClientMessageLost;
            client.MessageAcknowledged -= this.OnClientMessageAcknowledged;
            client.DisconnectWarning -= this.OnClientDisconnectWarning;
        }
    }
}