using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Protogame
{
    /// <summary>
    /// The Mx dispatcher; this handles receiving messages on the UDP client and dispatching
    /// them to the correct connected Mx client.
    /// </summary>
    /// <module>Network</module>
    public class MxDispatcher
    {
        private readonly Dictionary<string, MxClientGroup> _mxClientGroups;
        
        private readonly UdpClient _udpClient;
        
        private int _bytesLastReceived;
        
        private int _bytesLastSent;
        
        private bool _closed;

        private List<IPEndPoint> _explicitlyDisconnected;

        private object _mxClientGroupLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="MxDispatcher"/> class.
        /// </summary>
        /// <param name="port">
        /// The port of the UDP client.
        /// </param>
        public MxDispatcher(int port)
        {
            _udpClient = new UdpClient(port) { Client = { Blocking = false } };
            _mxClientGroups = new Dictionary<string, MxClientGroup>();
            _closed = false;
            _explicitlyDisconnected = new List<IPEndPoint>();

            _mxClientGroups.Add(MxClientGroup.Ungrouped, new MxClientGroup(this, MxClientGroup.Ungrouped));
            _mxClientGroupLock = new object();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MxDispatcher"/> class.
        /// </summary>
        public MxDispatcher(UdpClient client)
        {
            client.Client.Blocking = false;
            _udpClient = client;
            _mxClientGroups = new Dictionary<string, MxClientGroup>();
            _closed = false;
            _explicitlyDisconnected = new List<IPEndPoint>();

            _mxClientGroups.Add(MxClientGroup.Ungrouped, new MxClientGroup(this, MxClientGroup.Ungrouped));
            _mxClientGroupLock = new object();
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
        public bool Closed => _closed;

        /// <summary>
        /// Retrieves the client group for the given identifier.
        /// </summary>
        /// <param name="identifier">The identifier for the group.</param>
        /// <returns>The Mx client group.</returns>
        public MxClientGroup this[string identifier] => _mxClientGroups[identifier];

        /// <summary>
        /// Gets an enumeration of the latencies for all connected endpoints.
        /// </summary>
        /// <value>
        /// An enumeration of the latencies for all connected endpoints.
        /// </value>
        public IEnumerable<KeyValuePair<MxClientGroup, float>> Latencies
        {
            get
            {
                return
                    _mxClientGroups.Select(
                        x => new KeyValuePair<MxClientGroup, float>(x.Value, x.Value.RealtimeClients.Average(y => y.Latency))).ToArray();
            }
        }

        /// <summary>
        /// Closes the dispatcher permanently, terminating all inbound and outbound connections.
        /// </summary>
        public void Close()
        {
            _udpClient.Close();

            foreach (var group in _mxClientGroups)
            {
                Disconnect(group.Value);
            }

            _mxClientGroups.Clear();

            _closed = true;
        }

        /// <summary>
        /// Places the specified Mx client in the specified group.
        /// </summary>
        /// <param name="client">The Mx client.</param>
        /// <param name="identifier">The group identifier.</param>
        public MxClientGroup PlaceInGroup(MxClient client, string identifier)
        {
            if (client.Group.Identifier == identifier)
            {
                return client.Group;
            }

            if (!_mxClientGroups.ContainsKey(identifier))
            {
                _mxClientGroups[identifier] = new MxClientGroup(this, identifier);
            }

            var reliability = client.Group.ReliableClients.First(x => x.Client == client);

            client.Group.RealtimeClients.Remove(client);
            client.Group.ReliableClients.Remove(reliability);
            client.Group = _mxClientGroups[identifier];
            client.Group.RealtimeClients.Add(client);
            client.Group.ReliableClients.Add(reliability);

            return client.Group;
        }

        private MxClient ThreadSafeGetOrCreateClient(IPEndPoint endpoint)
        {
            MxClient client;

            lock (_mxClientGroupLock)
            {
                client =
                    _mxClientGroups
                        .Select(x => x.Value.RealtimeClients.FirstOrDefault(y => y.Endpoint.ToString() == endpoint.ToString()))
                        .FirstOrDefault(x => x != null);
                if (client != null)
                {
                    return client;
                }

                if (_explicitlyDisconnected.Contains(endpoint))
                {
                    return null;
                }

                client = new MxClient(
                    this,
                    _mxClientGroups[MxClientGroup.Ungrouped],
                    endpoint,
                    _udpClient);
                _mxClientGroups[MxClientGroup.Ungrouped].RealtimeClients.Add(client);
                RegisterForEvents(client);

                var reliability = new MxReliability(client);
                _mxClientGroups[MxClientGroup.Ungrouped].ReliableClients.Add(reliability);
                RegisterForEvents(reliability);
            }

            OnClientConnected(client);

            return client;
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
        public MxClient Connect(IPEndPoint endpoint)
        {
            AssertNotClosed();

            lock (_mxClientGroupLock)
            {
                if (_explicitlyDisconnected.Contains(endpoint))
                {
                    _explicitlyDisconnected.Remove(endpoint);
                }
            }

            var client = ThreadSafeGetOrCreateClient(endpoint);
            if (client == null)
            {
                throw new InvalidOperationException("Client was requested disconnect before connection was created.");
            }
            return client;
        }

        /// <summary>
        /// Disconnects the entire Mx client group, disconnecting
        /// all clients inside it.
        /// </summary>
        /// <param name="clientGroup">The client group.</param>
        public void Disconnect(MxClientGroup clientGroup)
        {
            foreach (var client in clientGroup.RealtimeClients.ToArray())
            {
                Disconnect(client);
            }
        }

        /// <summary>
        /// Disconnects the specified Mx client.  This removes it from
        /// the group that owns it, and prevents it from reconnecting to
        /// this dispatcher implicitly.
        /// </summary>
        /// <param name="client">The client.</param>
        public void Disconnect(MxClient client)
        {
            AssertNotClosed();

            lock (_mxClientGroupLock)
            {
                var reliability = client.Group.ReliableClients.First(x => x.Client == client);

                var group = client.Group;
                group.RealtimeClients.Remove(client);
                group.ReliableClients.Remove(reliability);

                UnregisterFromEvents(client);
                UnregisterFromEvents(reliability);

                _explicitlyDisconnected.Add(client.Endpoint);
            }

            OnClientDisconnected(client);
        }
        
        public int GetBytesLastReceivedAndReset()
        {
            var value = _bytesLastReceived;
            _bytesLastReceived = 0;
            return value;
        }
        
        public int GetBytesLastSentAndReset()
        {
            var value = _bytesLastSent;
            _bytesLastSent = 0;
            return value;
        }

        /// <summary>
        /// Queue a packet for sending to the specified endpoint.
        /// </summary>
        /// <param name="group">
        /// The group to send the message to.
        /// </param>
        /// <param name="packet">
        /// The associated data to send.
        /// </param>
        /// <param name="reliable">
        /// Whether or not this message should be sent reliably and intact.  This also
        /// permits messages larger than 512 bytes to be sent.
        /// </param>
        public void Send(MxClientGroup group, byte[] packet, bool reliable = false)
        {
            if (group.Identifier == MxClientGroup.Ungrouped)
            {
                throw new InvalidOperationException(
                    "You must group clients before sending packets to them.  Either " +
                    "call PlaceInGroup immediately after Connect, or call PlaceInGroup " +
                    "in the ClientConnected handler.");
            }

            foreach (var client in group.RealtimeClients)
            {
                Send(client, packet, reliable);
            }
        }

        /// <summary>
        /// Queue a packet for sending to the specified client.
        /// </summary>
        /// <param name="client">
        /// The client to send the message to.
        /// </param>
        /// <param name="packet">
        /// The associated data to send.
        /// </param>
        /// <param name="reliable">
        /// Whether or not this message should be sent reliably and intact.  This also
        /// permits messages larger than 512 bytes to be sent.
        /// </param>
        public void Send(MxClient client, byte[] packet, bool reliable = false)
        {
            AssertNotClosed();

            if (!reliable)
            { 
                client.EnqueueSend(packet, MxMessage.RealtimeProtocol);
            }
            else
            {
                var reliability = client.Group.ReliableClients.First(x => x.Client == client);
                reliability.Send(packet);
            }
        }

        /// <summary>
        /// Updates the dispatcher, receiving messages and connecting clients as appropriate.
        /// </summary>
        public void Update()
        {
            AssertNotClosed();

            // Receive as many messages from the connection as we possibly
            // can, and dispatch them to the correct MxClient.
            while (true)
            {
                var receive = new IPEndPoint(IPAddress.Loopback, IPEndPoint.MinPort);
                var packet = ReceiveNonBlocking(_udpClient, ref receive);
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
                    _udpClient.Send(new byte[] { 0x56, 0x78 }, 2, receive);
                    continue;
                }

                var client = ThreadSafeGetOrCreateClient(receive);
                if (client == null)
                {
                    // Client has been explicitly disconnected previously; ignore.
                    continue;
                }

                client.EnqueueReceive(packet);
            }

            // Update all of the client groups.
            foreach (var group in _mxClientGroups.ToArray())
            {
                // Update all of the clients.
                foreach (var client in group.Value.RealtimeClients.ToArray())
                {
                    client.Update();
                }

                // Update all of the reliabilities if needed.
                foreach (var reliability in group.Value.ReliableClients.ToArray())
                {
                    reliability.Update();
                }
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
            var handler = ClientConnected;
            handler?.Invoke(this, new MxClientEventArgs { Client = client });
        }

        /// <summary>
        /// Raise the ClientDisconnectWarning event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnClientDisconnectWarning(MxDisconnectEventArgs e)
        {
            var handler = ClientDisconnectWarning;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raise the ClientDisconnected event.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        protected virtual void OnClientDisconnected(MxClient client)
        {
            var handler = ClientDisconnected;
            handler?.Invoke(this, new MxClientEventArgs { Client = client });
        }

        /// <summary>
        /// Raise the MessageAcknowledged event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnMessageAcknowledged(MxMessageEventArgs e)
        {
            var handler = MessageAcknowledged;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raise the MessageLost event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnMessageLost(MxMessageEventArgs e)
        {
            var handler = MessageLost;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raise the MessageReceived event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnMessageReceived(MxMessageEventArgs e)
        {
            _bytesLastReceived += e.Payload.Length;

            var handler = MessageReceived;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raise the MessageSent event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnMessageSent(MxMessageEventArgs e)
        {
            _bytesLastSent += e.Payload.Length;

            var handler = MessageSent;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raise the ReliableReceivedProgress event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnReliableReceivedProgress(MxReliabilityTransmitEventArgs e)
        {
            var handler = ReliableReceivedProgress;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raise the ReliableSendProgress event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnReliableSendProgress(MxReliabilityTransmitEventArgs e)
        {
            var handler = ReliableSendProgress;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Assert that the dispatcher has not been closed.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown in the dispatcher has already been closed.
        /// </exception>
        private void AssertNotClosed()
        {
            if (_closed)
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
            OnClientDisconnectWarning(e);
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
                OnMessageAcknowledged(e);
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
                OnMessageLost(e);
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
                OnMessageReceived(e);
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
                OnMessageSent(e);
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
            OnReliableReceivedProgress(e);
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
            OnReliableSendProgress(e);
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
            OnMessageAcknowledged(e);
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
            OnMessageReceived(e);
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
            client.MessageSent += OnClientMessageSent;
            client.MessageReceived += OnClientMessageReceived;
            client.MessageLost += OnClientMessageLost;
            client.MessageAcknowledged += OnClientMessageAcknowledged;
            client.DisconnectWarning += OnClientDisconnectWarning;
        }

        /// <summary>
        /// Register for a reliability's events.
        /// </summary>
        /// <param name="reliability">
        /// The reliability.
        /// </param>
        private void RegisterForEvents(MxReliability reliability)
        {
            reliability.MessageAcknowledged += OnReliabilityMessageAcknowledged;
            reliability.MessageReceived += OnReliabilityMessageReceived;
            reliability.FragmentReceived += OnReliabilityFragmentReceived;
            reliability.FragmentSent += OnReliabilityFragmentSent;
        }

        /// <summary>
        /// Unregister from a client's events.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        private void UnregisterFromEvents(MxClient client)
        {
            client.MessageSent -= OnClientMessageSent;
            client.MessageReceived -= OnClientMessageReceived;
            client.MessageLost -= OnClientMessageLost;
            client.MessageAcknowledged -= OnClientMessageAcknowledged;
            client.DisconnectWarning -= OnClientDisconnectWarning;
        }

        /// <summary>
        /// Unregister from a reliability's events.
        /// </summary>
        /// <param name="reliability">
        /// The reliability.
        /// </param>
        private void UnregisterFromEvents(MxReliability reliability)
        {
            reliability.MessageAcknowledged -= OnReliabilityMessageAcknowledged;
            reliability.MessageAcknowledged -= OnReliabilityMessageReceived;
            reliability.FragmentReceived -= OnReliabilityFragmentReceived;
            reliability.FragmentSent -= OnReliabilityFragmentSent;
        }

        public IEnumerable<MxClientGroup> AllClientGroups
        {
            get
            {
                return _mxClientGroups.Values;
            }
        }

        public IEnumerable<MxClientGroup> ValidClientGroups
        {
            get
            {
                return _mxClientGroups.Where(x => x.Key != MxClientGroup.Ungrouped).Select(x => x.Value);
            }
        }
    }
}