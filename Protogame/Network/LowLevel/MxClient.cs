using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Protogame
{
    /// <summary>
    /// A client on the Mx protocol.
    /// </summary>
    /// <module>Network</module>
    public class MxClient
    {
        /// <summary>
        /// The number of packets that can be missed in an Update() call
        /// before this client is considered to be disconnected from the target endpoint.
        /// </summary>
        private int _disconnectLimit;

        /// <summary>
        /// The limit at which point the DisconnectWarning events will be raised.
        /// </summary>
        private int _disconnectWarningLimit;

        /// <summary>
        /// The dispatcher associated with this client.
        /// </summary>
        private readonly MxDispatcher _dispatcher;

        /// <summary>
        /// The packets real time that are currently waiting to be sent by this client.
        /// </summary>
        private readonly Queue<byte[]> _pendingRealtimeSendPackets;

        /// <summary>
        /// The packets reliable that are currently waiting to be sent by this client.
        /// </summary>
        private readonly Queue<byte[]> _pendingReliableSendPackets;

        /// <summary>
        /// The round trip time queue, which is used to calculate a moving average
        /// of the round trip time as packets arrive.
        /// </summary>
        private readonly List<ulong> _rttQueue;

        /// <summary>
        /// The round trip time threshold, which determines the point at which flow control
        /// will switch to "bad" mode.
        /// </summary>
        private readonly double _rttThreshold;

        /// <summary>
        /// The receive queue that caches what packets this client has acknowledges, such
        /// that the queue can be sent on outgoing packets.
        /// </summary>
        private readonly List<bool> _receiveQueue;

        /// <summary>
        /// The packets that were received by the dispatcher and enqueued for
        /// handling by this client.
        /// </summary>
        private readonly Queue<byte[]> _receivedPackets;

        /// <summary>
        /// The same as _sendQueue, except that it stores the protocol and payload as the value.  This
        /// is used to construct MessageLost events as required.
        /// </summary>
        private readonly Dictionary<uint, KeyValuePair<uint, byte[][]>> _sendMessageQueue;

        /// <summary>
        /// The send queue that has messages added to it when we send a message; it is a
        /// key-value pair of the sequence number of a message and the time it was sent.
        /// <para>
        /// Messages that are in this queue without explicit acknowledgement from the
        /// remote machine are assumed to have timed out after 1 second.
        /// </para>
        /// </summary>
        private readonly Dictionary<uint, ulong> _sendQueue;

        /// <summary>
        /// The shared UDP client, provided by the dispatcher, that is used to send outgoing messages.
        /// </summary>
        private readonly UdpClient _sharedUdpClient;

        /// <summary>
        /// The target endpoint that we are connected to.
        /// </summary>
        private readonly IPEndPoint _targetEndPoint;

        /// <summary>
        /// The delta time since the last Update() call.
        /// </summary>
        private double _deltaTime;

        /// <summary>
        /// The disconnect accumulator; when this reaches the disconnect limit, this client is considered
        /// to be disconnected from the target end point.
        /// </summary>
        private int _disconnectAccumulator;

        /// <summary>
        /// The good conditions time measurement for flow control.
        /// </summary>
        private double _fcGoodConditionsTime;

        /// <summary>
        /// Whether or not the flow control system is currently in good mode.
        /// </summary>
        private bool _fcIsGoodSendMode;

        /// <summary>
        /// The penalty reduction accumulator for flow control.
        /// </summary>
        private double _fcPenaltyReductionAccumulator;

        /// <summary>
        /// The penalty time for flow control.
        /// </summary>
        private double _fcPenaltyTime;

        /// <summary>
        /// The time that the Update() method was last called.
        /// </summary>
        private DateTime _lastCall;

        /// <summary>
        /// The local sequence number for outgoing packets.
        /// </summary>
        private uint _localSequenceNumber;

        /// <summary>
        /// The remote sequence number for incoming packets.
        /// </summary>
        private uint _remoteSequenceNumber;

        /// <summary>
        /// The send accumulator, which is used to determine how many packets we need to
        /// send in a given Update() call (such that on average we send out with a constant
        /// frequency).
        /// </summary>
        private double _sendAccumulator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MxClient"/> class.
        /// </summary>
        /// <param name="dispatcher">
        ///     The dispatcher that is creating this client.
        /// </param>
        /// <param name="mxClientGroup"></param>
        /// <param name="target">
        ///     The target endpoint for the Mx client.
        /// </param>
        /// <param name="sharedUdpClient">
        ///     The shared UDP client with which to send messages.
        /// </param>
        public MxClient(MxDispatcher dispatcher, MxClientGroup mxClientGroup, IPEndPoint target, UdpClient sharedUdpClient)
        {
            _dispatcher = dispatcher;
            Group = mxClientGroup;
            _targetEndPoint = target;
            _sharedUdpClient = sharedUdpClient;
            _receivedPackets = new Queue<byte[]>();
            _pendingRealtimeSendPackets = new Queue<byte[]>();
            _pendingReliableSendPackets = new Queue<byte[]>();
            _lastCall = DateTime.Now;
            _deltaTime = 1000.0 / 30.0;

            // Initialize connection information.
            _disconnectAccumulator = 0;
            _disconnectLimit = 900;
            _disconnectWarningLimit = 30;
            _receiveQueue = new List<bool>();
            for (var i = 0; i < 32; i++)
            {
                _receiveQueue.Add(false);
            }

            _rttQueue = new List<ulong>();
            _sendQueue = new Dictionary<uint, ulong>();
            _sendMessageQueue = new Dictionary<uint, KeyValuePair<uint, byte[][]>>();
            _localSequenceNumber = 0;
            _remoteSequenceNumber = uint.MaxValue;
            _sendAccumulator = 0;
            _rttThreshold = 250.0;
            _fcIsGoodSendMode = false;
            _fcPenaltyTime = 4.0;
            _fcGoodConditionsTime = 0;
            _fcPenaltyReductionAccumulator = 0;

            HasReceivedPacket = false;
        }

        /// <summary>
        /// Raised when the client has been disconnected for longer than one second.
        /// </summary>
        public event MxDisconnectEventHandler DisconnectWarning;

        /// <summary>
        /// Raised when the flow control settings have changed.
        /// </summary>
        public event FlowControlChangedEventHandler FlowControlChanged;

        /// <summary>
        /// Raised when a message has been acknowledged by the remote endpoint.
        /// </summary>
        public event MxMessageEventHandler MessageAcknowledged;

        /// <summary>
        /// Raised when a message was not acknowledged by the remote endpoint.
        /// </summary>
        public event MxMessageEventHandler MessageLost;

        /// <summary>
        /// Raised when a message has been received by this client.
        /// </summary>
        public event MxMessageReceiveEventHandler MessageReceived;

        /// <summary>
        /// Raised when a message has been sent by this client.
        /// </summary>
        public event MxMessageEventHandler MessageSent;

        /// <summary>
        /// Gets the endpoint that this client is responsible for.
        /// </summary>
        /// <value>
        /// The endpoint that this client is responsible for.
        /// </value>
        public IPEndPoint Endpoint
        {
            get
            {
                return new IPEndPoint(_targetEndPoint.Address, _targetEndPoint.Port);
            }
        }

        /// <summary>
        /// Gets the amount of network latency (lag) in milliseconds.
        /// </summary>
        /// <value>
        /// The network latency.
        /// </value>
        public float Latency { get; private set; }

        /// <summary>
        /// The group that this Mx client is in.  You should not set this directly; instead
        /// call <see cref="MxDispatcher.PlaceInGroup"/>.
        /// </summary>
        public MxClientGroup Group { get; set; }

        /// <summary>
        /// The disconnection timeout limit.
        /// </summary>
        public int DisconnectLimit
        {
            get { return _disconnectLimit; }
            set { _disconnectLimit = value; }
        }

        /// <summary>
        /// The disconnection timeout warning limit.
        /// </summary>
        public int DisconnectWarningLimit
        {
            get { return _disconnectWarningLimit; }
            set { _disconnectWarningLimit = value; }
        }

        /// <summary>
        /// The disconnection accumulator.
        /// </summary>
        public int DisconnectAccumulator => _disconnectAccumulator;

        /// <summary>
        /// Whether or not this client has ever received a packet.  When a group is isolating
        /// connections via <see cref="MxClientGroup.Isolate"/>, it checks this value to see
        /// whether or not the client has had successful communications as opposed to simply
        /// not having timed out yet.
        /// </summary>
        public bool HasReceivedPacket { get; private set; }

        /// <summary>
        /// Enqueues a byte array to be handled in the receiving logic when Update() is called.
        /// </summary>
        /// <param name="packet">
        /// The packet's byte data.
        /// </param>
        public void EnqueueReceive(byte[] packet)
        {
            HasReceivedPacket = true;
            _receivedPackets.Enqueue(packet);
        }

        /// <summary>
        /// Enqueues a byte array to be sent to the target endpoint when Update() is called.
        /// </summary>
        /// <param name="packet">
        /// The packet's byte data.
        /// </param>
        /// <param name="protocol">
        /// The packet's protocol type.
        /// </param>
        public void EnqueueSend(byte[] packet, uint protocol)
        {
            if (protocol == MxMessage.RealtimeProtocol)
            {
                _pendingRealtimeSendPackets.Enqueue(packet);
            }
            else if (protocol == MxMessage.ReliableProtocol)
            {
                _pendingReliableSendPackets.Enqueue(packet);
            }
            else
            {
                throw new InvalidOperationException("Protocol not supported.");
            }
        }

        /// <summary>
        /// Updates the state of the Mx client, sending outgoing packets and receiving incoming packets.
        /// </summary>
        public void Update()
        {
            _deltaTime = (DateTime.Now - _lastCall).TotalMilliseconds;
            _lastCall = DateTime.Now;

            if (_disconnectAccumulator > _disconnectWarningLimit)
            {
                OnDisconnectWarning(
                    new MxDisconnectEventArgs
                    {
                        Client = this, 
                        DisconnectAccumulator = _disconnectAccumulator, 
                        DisconnectTimeout = _disconnectLimit, 
                        IsDisconnected = _disconnectAccumulator > _disconnectLimit
                    });
            }

            if (_disconnectAccumulator > _disconnectLimit)
            {
                _dispatcher.Disconnect(this);
                return;
            }

            UpdateFlowControl();

            foreach (var kv in _sendQueue.ToArray())
            {
                var idx = kv.Key;

                if (GetUnixTimestamp() - kv.Value > 1000)
                {
                    HandleLostMessage(idx);
                }
            }

            PerformSend();

            PerformReceive();
        }

        /// <summary>
        /// Raise the DisconnectWarning event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnDisconnectWarning(MxDisconnectEventArgs e)
        {
            var handler = DisconnectWarning;
            if (handler != null)
            {
                handler(this, e);
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
            var handler = MessageAcknowledged;
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
            var handler = MessageLost;
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
        protected virtual void OnMessageReceived(MxMessageReceiveEventArgs e)
        {
            var handler = MessageReceived;
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
            var handler = MessageSent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Returns the current calculated round trip time, based on the last 32 messages received.
        /// </summary>
        /// <returns>
        /// The average round trip time.
        /// </returns>
        private double GetRTT()
        {
            var totalRtt = _rttQueue.Aggregate(0Lu, (current, val) => current + val);
            var avgRtt = 0.0;

            if (_rttQueue.Count > 0)
            {
                avgRtt = totalRtt / (double)_rttQueue.Count;
            }

            return avgRtt;
        }

        /// <summary>
        /// Returns the rate at which we should send messages according to flow control.
        /// </summary>
        /// <returns>
        /// The number of messages to send per second.
        /// </returns>
        private double GetSendTime()
        {
            return _fcIsGoodSendMode ? 1000.0 / 20 : 1000.0 / 10;
        }

        /// <summary>
        /// Returns the current UNIX timestamp.
        /// </summary>
        /// <returns>
        /// The current UNIX timestamp.
        /// </returns>
        private ulong GetUnixTimestamp()
        {
            return (uint)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        }

        /// <summary>
        /// Handles a lost message, removing it from the queues and firing
        /// events as appropriate.
        /// </summary>
        /// <param name="idx">
        /// The sequence number of the message that was lost.
        /// </param>
        private void HandleLostMessage(uint idx)
        {
            var payloads = _sendMessageQueue[idx];
            _sendQueue.Remove(idx);
            _sendMessageQueue.Remove(idx);

            foreach (var payload in payloads.Value)
            {
                OnMessageLost(new MxMessageEventArgs { Client = this, Payload = payload, ProtocolID = payloads.Key });
            }
        }

        /// <summary>
        /// Raise the FlowControlChanged event.
        /// </summary>
        private void OnFlowControlChanged()
        {
            var handler = FlowControlChanged;
            if (handler != null)
            {
                handler(
                    this, 
                    new FlowControlChangedEventArgs
                    {
                        IsGoodSendMode = _fcIsGoodSendMode, 
                        PenaltyTime = _fcPenaltyTime
                    });
            }
        }

        /// <summary>
        /// Handles the packets currently queued in the receive queue.
        /// </summary>
        private void PerformReceive()
        {
            if (_receivedPackets.Count == 0)
            {
                _disconnectAccumulator++;
                return;
            }

            foreach (var packet in _receivedPackets)
            {
                _disconnectAccumulator = 0;

                using (var memory = new MemoryStream(packet))
                {
                    var serializer = new MxMessageSerializer();
                    var message = (MxMessage)serializer.Deserialize(memory, null, typeof(MxMessage));

                    if (message.Payloads == null)
                    {
                        message.Payloads = new MxPayload[0];
                    }

                    foreach (var payload in message.Payloads.Where(payload => payload.Data == null))
                    {
                        payload.Data = new byte[0];
                    }

                    var difference = MxUtility.GetSequenceNumberDifference(
                        message.Sequence, 
                        _remoteSequenceNumber);

                    if (difference > 0)
                    {
                        // Calculate the difference between the old
                        // sequence number and the new sequence number
                        // we need to push "false" into the queue for
                        // the missing messages.
                        for (var i = 0; i < difference - 1; i++)
                        {
                            PushIntoQueue(_receiveQueue, false);
                        }
                        
                        // We push the "true" value for this message after
                        // firing the OnReceived event (so we can not acknowledge
                        // it if the event callbacks do not want us to).

                        // Check based on items in the queue.
                        foreach (var kv in _sendQueue.ToArray())
                        {
                            var idx = kv.Key;

                            if (!message.HasAck(idx))
                            {
                                // We aren't acking this message yet.
                                continue;
                            }

                            if (message.DidAck(idx))
                            {
                                // We already checked for existance of the key above.
                                var sendTimestamp = _sendQueue[idx];
                                var rtt = GetUnixTimestamp() - sendTimestamp;
                                PushIntoQueue(_rttQueue, rtt);

                                var payloads = _sendMessageQueue[idx];
                                _sendQueue.Remove(idx);
                                _sendMessageQueue.Remove(idx);

                                Latency = rtt;

                                foreach (var payload in payloads.Value)
                                {
                                    OnMessageAcknowledged(
                                        new MxMessageEventArgs { Client = this, Payload = payload, ProtocolID = payloads.Key });
                                }
                            }
                            else
                            {
                                HandleLostMessage(idx);
                            }
                        }

                        foreach (var kv in _sendQueue.ToArray())
                        {
                            var idx = kv.Key;

                            if (MxUtility.GetSequenceNumberDifference(message.Ack - MxUtility.UIntBitsize, idx) > 0)
                            {
                                HandleLostMessage(idx);
                            }
                        }

                        _remoteSequenceNumber = message.Sequence;

                        var doNotAcknowledge = false;

                        foreach (var payload in message.Payloads)
                        {
                            var eventArgs = new MxMessageReceiveEventArgs { Client = this, Payload = payload.Data, DoNotAcknowledge = doNotAcknowledge, ProtocolID = message.ProtocolID };
                            OnMessageReceived(eventArgs);
                            doNotAcknowledge = eventArgs.DoNotAcknowledge;
                        }

                        if (!doNotAcknowledge)
                        {
                            PushIntoQueue(_receiveQueue, true);
                        }
                    }
                }
            }

            _receivedPackets.Clear();
        }

        /// <summary>
        /// Handles sending packets to the target endpoint.
        /// </summary>
        private void PerformSend()
        {
            _sendAccumulator += _deltaTime;

            while (_sendAccumulator >= GetSendTime())
            {
                var queues = new[]
                {
                    new KeyValuePair<uint, Queue<byte[]>>(MxMessage.RealtimeProtocol, _pendingRealtimeSendPackets),
                    new KeyValuePair<uint, Queue<byte[]>>(MxMessage.ReliableProtocol, _pendingReliableSendPackets)
                };

                foreach (var item in queues)
                {
                    var protocol = item.Key;
                    var queue = item.Value;

                    byte[][] packets;
                    if (protocol == MxMessage.ReliableProtocol)
                    {
                        // In reliable mode, we know the sender is MxReliability and that it's optimized it's
                        // send calls for ~512 bytes.  Thus we just take one packet and use that.
                        packets = queue.Count > 0
                                      ? new[] { queue.Peek() }
                                      : new byte[0][];
                    }
                    else
                    {
                        // In real time mode, we use all of the currently queued packets and hope that the resulting
                        // size is not larger than 512 bytes (or is otherwise fragmented and dropped along the way).
                        packets = queue.ToArray();
                    }

                    using (var memory = new MemoryStream())
                    {
                        var message = new MxMessage
                        {
                            ProtocolID = protocol,
                            Payloads = packets.Select(x => new MxPayload { Data = x }).ToArray(),
                            Sequence = _localSequenceNumber,
                            Ack = _remoteSequenceNumber
                        };
                        message.SetAckBitfield(_receiveQueue.ToArray());

                        var serializer = new MxMessageSerializer();
                        serializer.Serialize(memory, message);
                        var len = (int)memory.Position;
                        memory.Seek(0, SeekOrigin.Begin);
                        var bytes = new byte[len];
                        memory.Read(bytes, 0, len);

                        if (len > 512 && protocol != MxMessage.ReliableProtocol)
                        {
                            // TODO: Probably fire an event here to warn that the queued messages exceeds the safe packet size.
                        }

                        try
                        {
                            try
                            {
                                _sharedUdpClient.Send(bytes, bytes.Length, _targetEndPoint);
                                _sendQueue.Add(_localSequenceNumber, GetUnixTimestamp());
                                _sendMessageQueue.Add(_localSequenceNumber, new KeyValuePair<uint, byte[][]>(protocol, packets));
                                _localSequenceNumber++;

                                if (protocol == MxMessage.ReliableProtocol)
                                {
                                    // Only dequeue the pending send packet once we know that it's at least
                                    // left this machine successfully (otherwise there'd be no message lost
                                    // event if they got consumed by a SocketException).
                                    if (queue.Count > 0)
                                    {
                                        queue.Dequeue();
                                    }
                                }
                                else
                                {
                                    // Only clear the pending send packets once we know that they've at least
                                    // left this machine successfully (otherwise there'd be no message lost
                                    // event if they got consumed by a SocketException).
                                    queue.Clear();
                                }

                                // Raise the OnMessageSent event.
                                foreach (var packet in packets)
                                {
                                    OnMessageSent(new MxMessageEventArgs { Client = this, Payload = packet });
                                }
                            }
                            catch (SocketException)
                            {
                                // We don't care.
                            }
                        }
                        catch (ObjectDisposedException)
                        {
                            // We don't care.
                        }
                    }
                }

                _sendAccumulator -= _deltaTime;
            }
        }

        /// <summary>
        /// Push the specified value into the list as if it was a queue, automatically discarding
        /// old entries if adding this one would push the queue count over 32.
        /// </summary>
        /// <param name="receiveQueue">
        /// The queue to push the item into.
        /// </param>
        /// <param name="b">
        /// The value to push into the queue.
        /// </param>
        /// <typeparam name="T">
        /// The type of list and value.
        /// </typeparam>
        private void PushIntoQueue<T>(List<T> receiveQueue, T b)
        {
            if (receiveQueue.Count >= 32)
            {
                receiveQueue.RemoveAt(0);
            }

            receiveQueue.Add(b);
        }

        /// <summary>
        /// Updates the flow control calculations based on recently received packets.
        /// </summary>
        private void UpdateFlowControl()
        {
            if (_fcIsGoodSendMode)
            {
                if (GetRTT() > _rttThreshold)
                {
                    _fcIsGoodSendMode = false;
                    if (_fcGoodConditionsTime < 10 && _fcPenaltyTime < 60)
                    {
                        _fcPenaltyTime *= 2;
                        if (_fcPenaltyTime > 60)
                        {
                            _fcPenaltyTime = 60;
                        }
                    }

                    _fcGoodConditionsTime = 0;
                    _fcPenaltyReductionAccumulator = 0;
                    OnFlowControlChanged();
                }
                else
                {
                    _fcGoodConditionsTime += _deltaTime;
                    _fcPenaltyReductionAccumulator += _deltaTime;
                    if (_fcPenaltyReductionAccumulator > 10 && _fcPenaltyTime > 1)
                    {
                        _fcPenaltyTime /= 2;
                        if (_fcPenaltyTime < 1)
                        {
                            _fcPenaltyTime = 1;
                        }

                        OnFlowControlChanged();
                        _fcPenaltyReductionAccumulator = 0;
                    }
                }

                return;
            }

            if (GetRTT() < _rttThreshold)
            {
                _fcGoodConditionsTime += _deltaTime;
            }
            else
            {
                _fcGoodConditionsTime = 0;
            }

            if (_fcGoodConditionsTime > _fcPenaltyTime)
            {
                _fcGoodConditionsTime = 0;
                _fcPenaltyReductionAccumulator = 0;
                _fcIsGoodSendMode = true;
                OnFlowControlChanged();
            }
        }
    }
}