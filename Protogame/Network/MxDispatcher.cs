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
    public class MxDispatcher
    {
        /// <summary>
        /// A list of currently connected real time Mx clients.
        /// </summary>
        private readonly Dictionary<DualIPEndPoint, MxClient> m_RealtimeMxClients;

        /// <summary>
        /// The UDP client that real time messages will be received on.
        /// </summary>
        private readonly UdpClient m_RealtimeUdpClient;

        /// <summary>
        /// A list of currently connected reliable Mx clients.
        /// </summary>
        private readonly Dictionary<DualIPEndPoint, MxClient> m_ReliableMxClients;

        /// <summary>
        /// A list of reliability objects that provide reliability for Mx clients.
        /// </summary>
        private readonly Dictionary<DualIPEndPoint, MxReliability> m_Reliabilities; 

        /// <summary>
        /// The UDP client that reliable messages will be received on.
        /// </summary>
        private readonly UdpClient m_ReliableUdpClient;

        /// <summary>
        /// Whether this dispatcher has been closed.
        /// </summary>
        private bool m_Closed;

        /// <summary>
        /// The total number of unreliable bytes sent during the last frame.
        /// </summary>
        private int m_BytesLastSent;

        /// <summary>
        /// The total number of unreliable bytes received during the last frame.
        /// </summary>
        private int m_BytesLastReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="MxDispatcher"/> class.
        /// </summary>
        /// <param name="realtimePort">
        /// The real time port of the UDP client.
        /// </param>
        /// <param name="reliablePort">
        /// The reliable port of the UDP client.
        /// </param>
        public MxDispatcher(int realtimePort, int reliablePort)
        {
            this.m_RealtimeUdpClient = new UdpClient(realtimePort) { Client = { Blocking = false } };
            this.m_RealtimeMxClients = new Dictionary<DualIPEndPoint, MxClient>();
            this.m_ReliableUdpClient = new UdpClient(reliablePort) { Client = { Blocking = false } };
            this.m_ReliableMxClients = new Dictionary<DualIPEndPoint, MxClient>();
            this.m_Reliabilities = new Dictionary<DualIPEndPoint, MxReliability>();
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
        /// Gets an enumeration of the endpoints of all connected clients.
        /// </summary>
        /// <value>
        /// An enumeration of the endpoints of all connected clients.
        /// </value>
        public IEnumerable<DualIPEndPoint> Endpoints
        {
            get
            {
                return this.m_RealtimeMxClients.Select(x => x.Key).Union(
                    this.m_ReliableMxClients.Select(x => x.Key)).ToArray();
            }
        }

        /// <summary>
        /// Gets an enumeration of the latencies for all connected endpoints.
        /// </summary>
        /// <value>
        /// An enumeration of the latencies for all connected endpoints.
        /// </value>
        public IEnumerable<KeyValuePair<DualIPEndPoint, float>> Latencies
        {
            get
            {
                return this.m_RealtimeMxClients
                        .Select(x => new KeyValuePair<DualIPEndPoint, float>(x.Key, x.Value.Latency))
                        .ToArray();
            }
        }

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
        /// Closes the dispatcher permanently, terminating all inbound and outbound connections.
        /// </summary>
        public void Close()
        {
            this.m_RealtimeUdpClient.Close();
            this.m_ReliableUdpClient.Close();

            foreach (var endpoint in this.Endpoints)
            {
                this.Disconnect(endpoint);
            }

            this.m_RealtimeMxClients.Clear();
            this.m_ReliableMxClients.Clear();
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
        public void Connect(DualIPEndPoint endpoint)
        {
            this.AssertNotClosed();

            this.m_RealtimeMxClients[endpoint] = new MxClient(
                this, 
                endpoint.RealtimeEndPoint,
                endpoint,
                this.m_RealtimeUdpClient, 
                false);
            this.OnClientConnected(this.m_RealtimeMxClients[endpoint]);
            this.RegisterForEvents(this.m_RealtimeMxClients[endpoint]);

            this.m_ReliableMxClients[endpoint] = new MxClient(
                this,
                endpoint.ReliableEndPoint,
                endpoint,
                this.m_ReliableUdpClient, 
                true);
            this.m_Reliabilities[endpoint] = new MxReliability(this.m_ReliableMxClients[endpoint]);
            this.OnClientConnected(this.m_ReliableMxClients[endpoint]);
            this.RegisterForEvents(this.m_Reliabilities[endpoint]);
        }

        public int GetBytesLastSentAndReset()
        {
            var value = this.m_BytesLastSent;
            this.m_BytesLastSent = 0;
            return value;
        }

        public int GetBytesLastReceivedAndReset()
        {
            var value = this.m_BytesLastReceived;
            this.m_BytesLastReceived = 0;
            return value;
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
        public void Disconnect(DualIPEndPoint endpoint)
        {
            this.AssertNotClosed();

            if (this.m_RealtimeMxClients.ContainsKey(endpoint))
            {
                var realtimeClient = this.m_RealtimeMxClients[endpoint];
                this.UnregisterFromEvents(realtimeClient);
                this.m_RealtimeMxClients.Remove(endpoint);
                this.OnClientDisconnected(realtimeClient);
            }

            if (this.m_ReliableMxClients.ContainsKey(endpoint))
            {
                var reliableClient = this.m_ReliableMxClients[endpoint];
                this.m_RealtimeMxClients.Remove(endpoint);
                this.OnClientDisconnected(reliableClient);
            }

            if (this.m_Reliabilities.ContainsKey(endpoint))
            {
                var reliability = this.m_Reliabilities[endpoint];
                this.UnregisterFromEvents(reliability);
                this.m_Reliabilities.Remove(endpoint);
            }
        }

        /// <summary>
        /// Resolves an IP endpoint and reliability information to a dual IP endpoint.
        /// </summary>
        /// <param name="endpoint">
        /// The IP endpoint.
        /// </param>
        /// <param name="reliable">
        /// Whether this endpoint is a reliable endpoint.
        /// </param>
        /// <returns>
        /// The <see cref="DualIPEndPoint"/>, or null if no dual endpoint matched.
        /// </returns>
        public DualIPEndPoint ResolveDualIPEndPoint(IPEndPoint endpoint, bool reliable)
        {
            var mxClients = !reliable ? this.m_RealtimeMxClients : this.m_ReliableMxClients;

            return
                mxClients.Select(x => x.Key)
                    .FirstOrDefault(
                        x =>
                        object.Equals(x.RealtimeEndPoint.Address, endpoint.Address)
                        && (!reliable
                                ? (x.RealtimeEndPoint.Port == endpoint.Port)
                                : (x.ReliableEndPoint.Port == endpoint.Port)));
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
        public void Send(DualIPEndPoint endpoint, byte[] packet, bool reliable = false)
        {
            this.AssertNotClosed();

            if (!reliable)
            {
                if (this.m_RealtimeMxClients.ContainsKey(endpoint))
                {
                    var client = this.m_RealtimeMxClients[endpoint];
                    client.EnqueueSend(packet);
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

            this.UpdateFor(this.m_RealtimeUdpClient, this.m_RealtimeMxClients, false);
            this.UpdateFor(this.m_ReliableUdpClient, this.m_ReliableMxClients, true);
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

        /// <summary>
        /// Updates the dispatcher for a particular type of endpoint (real time or reliable).
        /// </summary>
        /// <param name="udpClient">
        /// The UDP client.
        /// </param>
        /// <param name="mxClients">
        /// The associated Mx clients.
        /// </param>
        /// <param name="reliable">
        /// Whether or not the update is the for the reliable clients.
        /// </param>
        private void UpdateFor(UdpClient udpClient, Dictionary<DualIPEndPoint, MxClient> mxClients, bool reliable)
        {
            // Receive as many messages from the connection as we possibly
            // can, and dispatch them to the correct MxClient.
            while (true)
            {
                var receive = (IPEndPoint)null;
                var packet = this.ReceiveNonBlocking(udpClient, ref receive);
                if (packet == null)
                {
                    break;
                }

                var dualEndPoint = this.ResolveDualIPEndPoint(receive, reliable);
                if (dualEndPoint == null)
                {
                    // Do some heuristics to guess what the best policy is here.  There's no way to infer
                    // what the reliable port is from a real time message, so we do our best and try to pair
                    // up the missing component of a dual endpoint if it's available.  This will obviously 
                    // have incorrect results if two clients from the same IP address on different ports connect
                    // at the exact same time, but the chance of this happening is extremely improbable.

                    // We want to pick the opposite Mx client list, since if there's an existing connection
                    // on the opposing connection type, that's where we'll find it.
                    var opposingMxClients = reliable ? this.m_RealtimeMxClients : this.m_ReliableMxClients;

                    var candidates =
                        opposingMxClients.Select(x => x.Key)
                            .Where(
                                x =>
                                object.Equals(
                                    reliable ? x.RealtimeEndPoint.Address : x.ReliableEndPoint.Address,
                                    receive.Address))
                            .ToList();

                    // Find the first candidate that has one of the endpoints omitted.
                    var firstAvailable =
                        candidates.FirstOrDefault(x => (reliable ? x.ReliableEndPoint : x.RealtimeEndPoint) == null);

                    if (firstAvailable != null)
                    {
                        // We have an existing endpoint on the same address for the opposite connection type,
                        // and it is missing an endpoint for this connection type.
                        
                        // Before we change the dual IP endpoint, we have to temporarily remove it out of the
                        // opposing client list.  This is because by changing the dual endpoint's properties,
                        // we are also changing it's hash code, so it will not resolve in future lookups in
                        // the dictionary even though it's there.
                        var tempClient = opposingMxClients[firstAvailable];
                        opposingMxClients.Remove(firstAvailable);

                        // Backfill the current endpoint into that dual endpoint.
                        if (reliable)
                        {
                            firstAvailable.ReliableEndPoint = receive;
                        }
                        else
                        {
                            firstAvailable.RealtimeEndPoint = receive;
                        }

                        // Now add the key back into the opposing dictionary.
                        opposingMxClients.Add(firstAvailable, tempClient);

                        // And assign the endpoint so we use it.
                        dualEndPoint = firstAvailable;
                    }
                    else
                    {
                        // We do not have any preexisting connections that are from the same address and missing
                        // an endpoint for this connection type.  Create a new dual endpoint with the opposing side
                        // missing and use that as our endpoint.
                        dualEndPoint = reliable ? new DualIPEndPoint(null, receive) : new DualIPEndPoint(receive, null);
                    }
                }

                if (mxClients.ContainsKey(dualEndPoint))
                {
                    // Dispatch to an existing client.
                    mxClients[dualEndPoint].EnqueueReceive(packet);
                }
                else
                {
                    // Create a new client for this address.
                    mxClients[dualEndPoint] = new MxClient(this, receive, dualEndPoint, udpClient, reliable);
                    if (!reliable)
                    {
                        this.RegisterForEvents(mxClients[dualEndPoint]);
                    }
                    else
                    {
                        this.m_Reliabilities.Add(dualEndPoint, new MxReliability(mxClients[dualEndPoint]));
                        this.RegisterForEvents(this.m_Reliabilities[dualEndPoint]);
                    }

                    mxClients[dualEndPoint].EnqueueReceive(packet);
                    this.OnClientConnected(mxClients[dualEndPoint]);
                }
            }

            // Update all of the clients.
            foreach (var client in mxClients.Values.ToArray())
            {
                client.Update();
            }

            // Update all of the reliabilities if needed.
            if (reliable)
            {
                foreach (var reliability in this.m_Reliabilities.Select(x => x.Value).ToArray())
                {
                    reliability.Update();
                }
            }
        }
    }
}