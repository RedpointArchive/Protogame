namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// A client on the Mx protocol.
    /// </summary>
    public class MxClient
    {
        /// <summary>
        /// The number of packets that can be missed in an Update() call
        /// before this client is considered to be disconnected from the target endpoint.
        /// </summary>
        private readonly int m_DisconnectLimit;

        /// <summary>
        /// The limit at which point the DisconnectWarning events will be raised.
        /// </summary>
        private readonly int m_DisconnectWarningLimit;

        /// <summary>
        /// The dispatcher associated with this client.
        /// </summary>
        private readonly MxDispatcher m_Dispatcher;

        /// <summary>
        /// The dual IP endpoint that we belong to.
        /// </summary>
        private readonly DualIPEndPoint m_DualEndPoint;

        /// <summary>
        /// Whether or not this Mx client is a reliable client.
        /// </summary>
        private readonly bool m_IsReliable;

        /// <summary>
        /// The packets that are currently waiting to be sent by this client.
        /// </summary>
        private readonly Queue<byte[]> m_PendingSendPackets;

        /// <summary>
        /// The round trip time queue, which is used to calculate a moving average
        /// of the round trip time as packets arrive.
        /// </summary>
        private readonly List<ulong> m_RTTQueue;

        /// <summary>
        /// The round trip time threshold, which determines the point at which flow control
        /// will switch to "bad" mode.
        /// </summary>
        private readonly double m_RTTThreshold;

        /// <summary>
        /// The receive queue that caches what packets this client has acknowledges, such
        /// that the queue can be sent on outgoing packets.
        /// </summary>
        private readonly List<bool> m_ReceiveQueue;

        /// <summary>
        /// The packets that were received by the dispatcher and enqueued for
        /// handling by this client.
        /// </summary>
        private readonly Queue<byte[]> m_ReceivedPackets;

        /// <summary>
        /// The same as m_SendQueue, except that it stores the payload as the value.  This
        /// is used to construct MessageLost events as required.
        /// </summary>
        private readonly Dictionary<uint, byte[][]> m_SendMessageQueue;

        /// <summary>
        /// The send queue that has messages added to it when we send a message; it is a
        /// key-value pair of the sequence number of a message and the time it was sent.
        /// <para>
        /// Messages that are in this queue without explicit acknowledgement from the
        /// remote machine are assumed to have timed out after 1 second.
        /// </para>
        /// </summary>
        private readonly Dictionary<uint, ulong> m_SendQueue;

        /// <summary>
        /// The shared UDP client, provided by the dispatcher, that is used to send outgoing messages.
        /// </summary>
        private readonly UdpClient m_SharedUdpClient;

        /// <summary>
        /// The target endpoint that we are connected to.
        /// </summary>
        private readonly IPEndPoint m_TargetEndPoint;

        /// <summary>
        /// The delta time since the last Update() call.
        /// </summary>
        private double m_DeltaTime;

        /// <summary>
        /// The disconnect accumulator; when this reaches the disconnect limit, this client is considered
        /// to be disconnected from the target end point.
        /// </summary>
        private int m_DisconnectAccumulator;

        /// <summary>
        /// The good conditions time measurement for flow control.
        /// </summary>
        private double m_FCGoodConditionsTime;

        /// <summary>
        /// Whether or not the flow control system is currently in good mode.
        /// </summary>
        private bool m_FCIsGoodSendMode;

        /// <summary>
        /// The penalty reduction accumulator for flow control.
        /// </summary>
        private double m_FCPenaltyReductionAccumulator;

        /// <summary>
        /// The penalty time for flow control.
        /// </summary>
        private double m_FCPenaltyTime;

        /// <summary>
        /// The time that the Update() method was last called.
        /// </summary>
        private DateTime m_LastCall;

        /// <summary>
        /// The local sequence number for outgoing packets.
        /// </summary>
        private uint m_LocalSequenceNumber;

        /// <summary>
        /// The remote sequence number for incoming packets.
        /// </summary>
        private uint m_RemoteSequenceNumber;

        /// <summary>
        /// The send accumulator, which is used to determine how many packets we need to
        /// send in a given Update() call (such that on average we send out with a constant
        /// frequency).
        /// </summary>
        private double m_SendAccumulator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MxClient"/> class.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher that is creating this client.
        /// </param>
        /// <param name="target">
        /// The target endpoint for the Mx client.
        /// </param>
        /// <param name="dualEndpoint">
        /// The dual IP endpoint for this Mx client.
        /// </param>
        /// <param name="sharedUdpClient">
        /// The shared UDP client with which to send messages.
        /// </param>
        /// <param name="reliable">
        /// Whether or not this is a reliable Mx client.
        /// </param>
        public MxClient(
            MxDispatcher dispatcher, 
            IPEndPoint target, 
            DualIPEndPoint dualEndpoint, 
            UdpClient sharedUdpClient, 
            bool reliable)
        {
            this.m_Dispatcher = dispatcher;
            this.m_TargetEndPoint = target;
            this.m_DualEndPoint = dualEndpoint;
            this.m_SharedUdpClient = sharedUdpClient;
            this.m_ReceivedPackets = new Queue<byte[]>();
            this.m_PendingSendPackets = new Queue<byte[]>();
            this.m_LastCall = DateTime.Now;
            this.m_DeltaTime = 1000.0 / 30.0;
            this.m_IsReliable = reliable;

            // Initialize connection information.
            this.m_DisconnectAccumulator = 0;
            this.m_DisconnectLimit = 900;
            this.m_DisconnectWarningLimit = 30;
            this.m_ReceiveQueue = new List<bool>();
            for (var i = 0; i < 32; i++)
            {
                this.m_ReceiveQueue.Add(false);
            }

            this.m_RTTQueue = new List<ulong>();
            this.m_SendQueue = new Dictionary<uint, ulong>();
            this.m_SendMessageQueue = new Dictionary<uint, byte[][]>();
            this.m_LocalSequenceNumber = 0;
            this.m_RemoteSequenceNumber = uint.MaxValue;
            this.m_SendAccumulator = 0;
            this.m_RTTThreshold = 250.0;
            this.m_FCIsGoodSendMode = false;
            this.m_FCPenaltyTime = 4.0;
            this.m_FCGoodConditionsTime = 0;
            this.m_FCPenaltyReductionAccumulator = 0;
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
        public event MxMessageEventHandler MessageReceived;

        /// <summary>
        /// Raised when a message has been sent by this client.
        /// </summary>
        public event MxMessageEventHandler MessageSent;

        /// <summary>
        /// Gets the dual endpoint that this client belongs to.
        /// </summary>
        /// <value>
        /// The dual endpoint that this client belongs to.
        /// </value>
        public DualIPEndPoint DualEndpoint
        {
            get
            {
                return new DualIPEndPoint(this.m_DualEndPoint.RealtimeEndPoint, this.m_DualEndPoint.ReliableEndPoint);
            }
        }

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
                return new IPEndPoint(this.m_TargetEndPoint.Address, this.m_TargetEndPoint.Port);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this client is reliable.
        /// </summary>
        /// <value>
        /// Whether this client is reliable.
        /// </value>
        public bool IsReliable
        {
            get
            {
                return this.m_IsReliable;
            }
        }

        /// <summary>
        /// The amount of network latency (lag) in milliseconds.
        /// </summary>
        /// <value>
        /// The network latency.
        /// </value>
        public float Latency { get; private set; }

        /// <summary>
        /// Enqueues a byte array to be handled in the receiving logic when Update() is called.
        /// </summary>
        /// <param name="packet">
        /// The packet's byte data.
        /// </param>
        public void EnqueueReceive(byte[] packet)
        {
            this.m_ReceivedPackets.Enqueue(packet);
        }

        /// <summary>
        /// Enqueues a byte array to be sent to the target endpoint when Update() is called.
        /// </summary>
        /// <param name="packet">
        /// The packet's byte data.
        /// </param>
        public void EnqueueSend(byte[] packet)
        {
            this.m_PendingSendPackets.Enqueue(packet);
        }

        /// <summary>
        /// Updates the state of the Mx client, sending outgoing packets and receiving incoming packets.
        /// </summary>
        public void Update()
        {
            this.m_DeltaTime = (DateTime.Now - this.m_LastCall).TotalMilliseconds;
            this.m_LastCall = DateTime.Now;

            if (this.m_DisconnectAccumulator > this.m_DisconnectWarningLimit)
            {
                this.OnDisconnectWarning(
                    new MxDisconnectEventArgs
                    {
                        Client = this, 
                        DisconnectAccumulator = this.m_DisconnectAccumulator, 
                        DisconnectTimeout = this.m_DisconnectLimit, 
                        IsDisconnected = this.m_DisconnectAccumulator > this.m_DisconnectLimit
                    });
            }

            if (this.m_DisconnectAccumulator > this.m_DisconnectLimit)
            {
                var dualEndPoint = this.m_Dispatcher.ResolveDualIPEndPoint(this.m_TargetEndPoint, this.m_IsReliable);
                this.m_Dispatcher.Disconnect(dualEndPoint);
                return;
            }

            this.UpdateFlowControl();

            foreach (var kv in this.m_SendQueue.ToArray())
            {
                var idx = kv.Key;

                if (this.GetUnixTimestamp() - kv.Value > 1000)
                {
                    this.HandleLostMessage(idx);
                }
            }

            this.PerformSend();

            this.PerformReceive();
        }

        /// <summary>
        /// Raise the DisconnectWarning event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnDisconnectWarning(MxDisconnectEventArgs e)
        {
            MxDisconnectEventHandler handler = this.DisconnectWarning;
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
        /// Returns the current calculated round trip time, based on the last 32 messages received.
        /// </summary>
        /// <returns>
        /// The average round trip time.
        /// </returns>
        private double GetRTT()
        {
            var totalRtt = this.m_RTTQueue.Aggregate(0Lu, (current, val) => current + val);
            var avgRtt = 0.0;

            if (this.m_RTTQueue.Count > 0)
            {
                avgRtt = totalRtt / (double)this.m_RTTQueue.Count;
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
            return this.m_FCIsGoodSendMode ? 1000.0 / 20 : 1000.0 / 10;
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
            var payloads = this.m_SendMessageQueue[idx];
            this.m_SendQueue.Remove(idx);
            this.m_SendMessageQueue.Remove(idx);

            foreach (var payload in payloads)
            {
                this.OnMessageLost(new MxMessageEventArgs { Client = this, Payload = payload });
            }
        }

        /// <summary>
        /// Raise the FlowControlChanged event.
        /// </summary>
        private void OnFlowControlChanged()
        {
            var handler = this.FlowControlChanged;
            if (handler != null)
            {
                handler(
                    this, 
                    new FlowControlChangedEventArgs
                    {
                        IsGoodSendMode = this.m_FCIsGoodSendMode, 
                        PenaltyTime = this.m_FCPenaltyTime
                    });
            }
        }

        /// <summary>
        /// Handles the packets currently queued in the receive queue.
        /// </summary>
        private void PerformReceive()
        {
            if (this.m_ReceivedPackets.Count == 0)
            {
                this.m_DisconnectAccumulator++;
                return;
            }

            foreach (var packet in this.m_ReceivedPackets)
            {
                this.m_DisconnectAccumulator = 0;

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
                        this.m_RemoteSequenceNumber);

                    if (difference > 0)
                    {
                        // Calculate the difference between the old
                        // sequence number and the new sequence number
                        // we need to push "false" into the queue for
                        // the missing messages.
                        for (var i = 0; i < difference - 1; i++)
                        {
                            this.PushIntoQueue(this.m_ReceiveQueue, false);
                        }

                        this.PushIntoQueue(this.m_ReceiveQueue, true);

                        // Check based on items in the queue.
                        foreach (var kv in this.m_SendQueue.ToArray())
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
                                var sendTimestamp = this.m_SendQueue[idx];
                                var rtt = this.GetUnixTimestamp() - sendTimestamp;
                                this.PushIntoQueue(this.m_RTTQueue, rtt);

                                var payloads = this.m_SendMessageQueue[idx];
                                this.m_SendQueue.Remove(idx);
                                this.m_SendMessageQueue.Remove(idx);

                                this.Latency = rtt;

                                foreach (var payload in payloads)
                                {
                                    this.OnMessageAcknowledged(
                                        new MxMessageEventArgs { Client = this, Payload = payload });
                                }
                            }
                            else
                            {
                                this.HandleLostMessage(idx);
                            }
                        }

                        foreach (var kv in this.m_SendQueue.ToArray())
                        {
                            var idx = kv.Key;

                            if (MxUtility.GetSequenceNumberDifference(message.Ack - MxUtility.UIntBitsize, idx) > 0)
                            {
                                this.HandleLostMessage(idx);
                            }
                        }

                        this.m_RemoteSequenceNumber = message.Sequence;

                        foreach (var payload in message.Payloads)
                        {
                            this.OnMessageReceived(new MxMessageEventArgs { Client = this, Payload = payload.Data });
                        }
                    }
                }
            }

            this.m_ReceivedPackets.Clear();
        }

        /// <summary>
        /// Handles sending packets to the target endpoint.
        /// </summary>
        private void PerformSend()
        {
            this.m_SendAccumulator += this.m_DeltaTime;

            while (this.m_SendAccumulator >= this.GetSendTime())
            {
                byte[][] packets;
                if (this.m_IsReliable)
                {
                    // In reliable mode, we know the sender is MxReliability and that it's optimized it's
                    // send calls for ~512 bytes.  Thus we just take one packet and use that.
                    packets = this.m_PendingSendPackets.Count > 0
                                  ? new[] { this.m_PendingSendPackets.Peek() }
                                  : new byte[0][];
                }
                else
                {
                    // In real time mode, we use all of the currently queued packets and hope that the resulting
                    // size is not larger than 512 bytes (or is otherwise fragmented and dropped along the way).
                    packets = this.m_PendingSendPackets.ToArray();
                }

                using (var memory = new MemoryStream())
                {
                    var message = new MxMessage
                    {
                        Payloads = packets.Select(x => new MxPayload { Data = x }).ToArray(),
                        Sequence = this.m_LocalSequenceNumber,
                        Ack = this.m_RemoteSequenceNumber
                    };
                    message.SetAckBitfield(this.m_ReceiveQueue.ToArray());

                    var serializer = new MxMessageSerializer();
                    serializer.Serialize(memory, message);
                    var len = (int)memory.Position;
                    memory.Seek(0, SeekOrigin.Begin);
                    var bytes = new byte[len];
                    memory.Read(bytes, 0, len);

                    if (len > 512 && !this.m_IsReliable)
                    {
                        // TODO: Probably fire an event here to warn that the queued messages exceeds the safe packet size.
                    }

                    try
                    {
                        try
                        {
                            this.m_SharedUdpClient.Send(bytes, bytes.Length, this.m_TargetEndPoint);
                            this.m_SendQueue.Add(this.m_LocalSequenceNumber, this.GetUnixTimestamp());
                            this.m_SendMessageQueue.Add(this.m_LocalSequenceNumber, packets);
                            this.m_LocalSequenceNumber++;

                            if (this.m_IsReliable)
                            {
                                // Only dequeue the pending send packet once we know that it's at least
                                // left this machine successfully (otherwise there'd be no message lost
                                // event if they got consumed by a SocketException).
                                if (this.m_PendingSendPackets.Count > 0)
                                {
                                    this.m_PendingSendPackets.Dequeue();
                                }
                            }
                            else
                            {
                                // Only clear the pending send packets once we know that they've at least
                                // left this machine successfully (otherwise there'd be no message lost
                                // event if they got consumed by a SocketException).
                                this.m_PendingSendPackets.Clear();
                            }

                            // Raise the OnMessageSent event.
                            foreach (var packet in packets)
                            {
                                this.OnMessageSent(new MxMessageEventArgs { Client = this, Payload = packet });
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

                this.m_SendAccumulator -= this.m_DeltaTime;
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
            if (this.m_FCIsGoodSendMode)
            {
                if (this.GetRTT() > this.m_RTTThreshold)
                {
                    this.m_FCIsGoodSendMode = false;
                    if (this.m_FCGoodConditionsTime < 10 && this.m_FCPenaltyTime < 60)
                    {
                        this.m_FCPenaltyTime *= 2;
                        if (this.m_FCPenaltyTime > 60)
                        {
                            this.m_FCPenaltyTime = 60;
                        }
                    }

                    this.m_FCGoodConditionsTime = 0;
                    this.m_FCPenaltyReductionAccumulator = 0;
                    this.OnFlowControlChanged();
                }
                else
                {
                    this.m_FCGoodConditionsTime += this.m_DeltaTime;
                    this.m_FCPenaltyReductionAccumulator += this.m_DeltaTime;
                    if (this.m_FCPenaltyReductionAccumulator > 10 && this.m_FCPenaltyTime > 1)
                    {
                        this.m_FCPenaltyTime /= 2;
                        if (this.m_FCPenaltyTime < 1)
                        {
                            this.m_FCPenaltyTime = 1;
                        }

                        this.OnFlowControlChanged();
                        this.m_FCPenaltyReductionAccumulator = 0;
                    }
                }

                return;
            }

            if (this.GetRTT() < this.m_RTTThreshold)
            {
                this.m_FCGoodConditionsTime += this.m_DeltaTime;
            }
            else
            {
                this.m_FCGoodConditionsTime = 0;
            }

            if (this.m_FCGoodConditionsTime > this.m_FCPenaltyTime)
            {
                this.m_FCGoodConditionsTime = 0;
                this.m_FCPenaltyReductionAccumulator = 0;
                this.m_FCIsGoodSendMode = true;
                this.OnFlowControlChanged();
            }
        }
    }
}