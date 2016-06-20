namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// The Mx dispatcher; this handles receiving messages on the UDP client and dispatching
    /// them to the correct connected Mx client.
    /// </summary>
    /// <module>Network</module>
    public class MxDispatcher
    {
        /// <summary>
        /// A list of currently connected Mx clients.
        /// </summary>
        private readonly Dictionary<IPEndPoint, MxClient> m_MxClients;

        /// <summary>
        /// A list of reliability objects that provide reliability for Mx clients.
        /// </summary>
        private readonly Dictionary<IPEndPoint, MxReliability> m_Reliabilities;

        /// <summary>
        /// The UDP client that messages will be received on.
        /// </summary>
        private readonly UdpClient m_UdpClient;

        /// <summary>
        /// The total number of unreliable bytes received during the last frame.
        /// </summary>
        private int m_BytesLastReceived;

        /// <summary>
        /// The total number of unreliable bytes sent during the last frame.
        /// </summary>
        private int m_BytesLastSent;

        /// <summary>
        /// Whether this dispatcher has been closed.
        /// </summary>
        private bool m_Closed;

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
            this.m_Reliabilities = new Dictionary<IPEndPoint, MxReliability>();
            this.m_Closed = false;
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
        /// Raised when progress has been made receiving a reliable message.
        /// </summary>
        public event MxReliabilityTransmitEventHandler ReliableReceivedProgress;

        /// <summary>
        /// Raised when progress has been made sending a reliable message.
        /// </summary>
        public event MxReliabilityTransmitEventHandler ReliableSendProgress;

        /// <summary>
        /// Gets a value indicating whether this dispatcher has been closed.
        /// </summary>
        /// <value>
        /// Whether or not this dispatcher has been closed.
        /// </value>
        public bool Closed
        {
            get
            {
                return this.m_Closed;
            }
        }

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
                return this.m_MxClients.Keys.ToList();
            }
        }

        /// <summary>
        /// Gets an enumeration of the latencies for all connected endpoints.
        /// </summary>
        /// <value>
        /// An enumeration of the latencies for all connected endpoints.
        /// </value>
        public IEnumerable<KeyValuePair<IPEndPoint, float>> Latencies
        {
            get
            {
                return
                    this.m_MxClients.Select(
                        x => new KeyValuePair<IPEndPoint, float>(x.Key, x.Value.Latency)).ToArray();
            }
        }

        /// <summary>
        /// Closes the dispatcher permanently, terminating all inbound and outbound connections.
        /// </summary>
        public void Close()
        {
            this.m_UdpClient.Close();

            foreach (var endpoint in this.Endpoints)
            {
                this.Disconnect(endpoint);
            }

            this.m_MxClients.Clear();
            this.m_Reliabilities.Clear();

            this.m_Closed = true;
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
            this.AssertNotClosed();

            this.m_MxClients[endpoint] = new MxClient(
                this, 
                endpoint, 
                this.m_UdpClient);
            this.RegisterForEvents(this.m_MxClients[endpoint]);

            this.m_Reliabilities[endpoint] = new MxReliability(this.m_MxClients[endpoint]);
            this.RegisterForEvents(this.m_Reliabilities[endpoint]);

            this.OnClientConnected(this.m_MxClients[endpoint]);
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
            this.AssertNotClosed();

            if (this.m_MxClients.ContainsKey(endpoint))
            {
                var realtimeClient = this.m_MxClients[endpoint];
                this.UnregisterFromEvents(realtimeClient);
                this.m_MxClients.Remove(endpoint);
                this.OnClientDisconnected(realtimeClient);
            }

            if (this.m_Reliabilities.ContainsKey(endpoint))
            {
                var reliability = this.m_Reliabilities[endpoint];
                this.UnregisterFromEvents(reliability);
                this.m_Reliabilities.Remove(endpoint);
            }
        }

        /// <summary>
        /// The get bytes last received and reset.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetBytesLastReceivedAndReset()
        {
            var value = this.m_BytesLastReceived;
            this.m_BytesLastReceived = 0;
            return value;
        }

        /// <summary>
        /// The get bytes last sent and reset.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetBytesLastSentAndReset()
        {
            var value = this.m_BytesLastSent;
            this.m_BytesLastSent = 0;
            return value;
        }

        /// <summary>
        /// Returns the Mx client for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The IP endpoint.</param>
        /// <returns>The Mx client.</returns>
        public MxClient GetRealtimeClient(IPEndPoint endpoint)
        {
            return this.m_MxClients[endpoint];
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
        /// <param name="reliable">
        /// Whether or not this message should be sent reliably and intact.  This also
        /// permits messages larger than 512 bytes to be sent.
        /// </param>
        public void Send(IPEndPoint endpoint, byte[] packet, bool reliable = false)
        {
            this.AssertNotClosed();

            if (!reliable)
            {
                if (this.m_MxClients.ContainsKey(endpoint))
                {
                    var client = this.m_MxClients[endpoint];
                    client.EnqueueSend(packet, MxMessage.RealtimeProtocol);
                }
            }
            else
            {
                if (this.m_Reliabilities.ContainsKey(endpoint))
                {
                    var reliability = this.m_Reliabilities[endpoint];
                    reliability.Send(packet);
                }
            }
        }

        /// <summary>
        /// Updates the dispatcher, receiving messages and connecting clients as appropriate.
        /// </summary>
        public void Update()
        {
            this.AssertNotClosed();

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

                // Handle probe packets. Send a UDP packet to either port with the following
                // hexadecimal values to get a response:
                // 
                // 0x12, 0x34
                //
                // This is smaller than the possible MxMessage, so we know that if it's exactly
                // this size with these character, it's a probe packet.
                if (packet.Length == 2 && packet[0] == 0x12 && packet[1] == 0x34)
                {
                    // This sends back 0x56, 0x78.
                    this.m_UdpClient.Send(new byte[] { 0x56, 0x78 }, 2, receive);
                    continue;
                }

                if (this.m_MxClients.ContainsKey(receive))
                {
                    // Dispatch to an existing client.
                    this.m_MxClients[receive].EnqueueReceive(packet);
                }
                else
                {
                    // Create a new client for this address.
                    this.m_MxClients.Add(receive, new MxClient(this, receive, this.m_UdpClient));
                    
                    this.RegisterForEvents(this.m_MxClients[receive]);

                    this.m_Reliabilities.Add(receive, new MxReliability(this.m_MxClients[receive]));
                    this.RegisterForEvents(this.m_Reliabilities[receive]);

                    this.m_MxClients[receive].EnqueueReceive(packet);
                    this.OnClientConnected(this.m_MxClients[receive]);
                }
            }

            // Update all of the clients.
            foreach (var client in this.m_MxClients.Values.ToArray())
            {
                client.Update();
            }

            // Update all of the reliabilities if needed.
            foreach (var reliability in this.m_Reliabilities.Select(x => x.Value).ToArray())
            {
                reliability.Update();
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
            this.m_BytesLastReceived += e.Payload.Length;

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
            this.m_BytesLastSent += e.Payload.Length;

            var handler = this.MessageSent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raise the ReliableReceivedProgress event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnReliableReceivedProgress(MxReliabilityTransmitEventArgs e)
        {
            var handler = this.ReliableReceivedProgress;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raise the ReliableSendProgress event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnReliableSendProgress(MxReliabilityTransmitEventArgs e)
        {
            var handler = this.ReliableSendProgress;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Assert that the dispatcher has not been closed.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown in the dispatcher has already been closed.
        /// </exception>
        private void AssertNotClosed()
        {
            if (this.m_Closed)
            {
                throw new InvalidOperationException("You can not use an MxDispatcher once it has been closed.");
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
            // Exclude reliable protocol messages as they are handled by the
            // reliability layer.
            if (e.ProtocolID == MxMessage.RealtimeProtocol)
            {
                this.OnMessageAcknowledged(e);
            }
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
            // Exclude reliable protocol messages as they are handled by the
            // reliability layer.
            if (e.ProtocolID == MxMessage.RealtimeProtocol)
            {
                this.OnMessageLost(e);
            }
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
            // Exclude reliable protocol messages as they are handled by the
            // reliability layer.
            if (e.ProtocolID == MxMessage.RealtimeProtocol)
            {
                this.OnMessageReceived(e);
            }
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
            // Exclude reliable protocol messages as they are handled by the
            // reliability layer.
            if (e.ProtocolID == MxMessage.RealtimeProtocol)
            {
                this.OnMessageSent(e);
            }
        }

        /// <summary>
        /// Handle receiving a FragmentReceived event from a reliability class.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnReliabilityFragmentReceived(object sender, MxReliabilityTransmitEventArgs e)
        {
            this.OnReliableReceivedProgress(e);
        }

        /// <summary>
        /// Handle receiving a FragmentSent event from a reliability class.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnReliabilityFragmentSent(object sender, MxReliabilityTransmitEventArgs e)
        {
            this.OnReliableSendProgress(e);
        }

        /// <summary>
        /// Handle receiving a MessageAcknowledged event from a reliability class.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnReliabilityMessageAcknowledged(object sender, MxMessageEventArgs e)
        {
            this.OnMessageAcknowledged(e);
        }

        /// <summary>
        /// Handle receiving a MessageReceived event from a reliability class.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnReliabilityMessageReceived(object sender, MxMessageEventArgs e)
        {
            this.OnMessageReceived(e);
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
            try
            {
                if (client.Available == 0)
                {
                    return null;
                }
            }
            catch (ObjectDisposedException)
            {
                return null;
            }
            catch (NullReferenceException)
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
            catch (ObjectDisposedException)
            {
                return null;
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
        /// Register for a reliability's events.
        /// </summary>
        /// <param name="reliability">
        /// The reliability.
        /// </param>
        private void RegisterForEvents(MxReliability reliability)
        {
            reliability.MessageAcknowledged += this.OnReliabilityMessageAcknowledged;
            reliability.MessageReceived += this.OnReliabilityMessageReceived;
            reliability.FragmentReceived += this.OnReliabilityFragmentReceived;
            reliability.FragmentSent += this.OnReliabilityFragmentSent;
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

        /// <summary>
        /// Unregister from a reliability's events.
        /// </summary>
        /// <param name="reliability">
        /// The reliability.
        /// </param>
        private void UnregisterFromEvents(MxReliability reliability)
        {
            reliability.MessageAcknowledged -= this.OnReliabilityMessageAcknowledged;
            reliability.MessageAcknowledged -= this.OnReliabilityMessageReceived;
            reliability.FragmentReceived -= this.OnReliabilityFragmentReceived;
            reliability.FragmentSent -= this.OnReliabilityFragmentSent;
        }
    }
}