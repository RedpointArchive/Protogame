namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The class that provides reliability and fragmentation infrastructure for Mx clients.
    /// <para>
    /// This class is used by <see cref="MxDispatcher"/> to interface with an <see cref="MxClient"/> in a reliable manner.
    /// When data is sent reliably, this class is used to fragment and reconstruct sets of data, ensuring that each
    /// fragment is either acknowledged by the receiving machine, or sent again.
    /// </para>
    /// </summary>
    public class MxReliability
    {
        /// <summary>
        /// The safe fragment size.
        /// </summary>
        /// <remarks>
        /// This should be converted into a variable which is used to determine the packet size, and should
        /// be automatically reduced on packet loss.
        /// </remarks>
        public const int SafeFragmentSize = 512;

        /// <summary>
        /// The associated <see cref="MxClient"/> that data is sent and received through.
        /// </summary>
        private readonly MxClient m_Client;

        /// <summary>
        /// A list of unordered, but received fragments.  Fragments are placed into this list
        /// when they are received before a header.  This can occur if the header fragment is
        /// lost, but some of the content fragments are received.
        /// </summary>
        private readonly List<Fragment> m_CurrentUnorderedReceiveFragments;

        /// <summary>
        /// A list of queued messages that are waiting to be sent.
        /// </summary>
        private readonly Queue<byte[]> m_QueuedMessages;

        /// <summary>
        /// A list of currently received fragments.  Unlike the current send fragments, this
        /// does not include the header or footer fragments.
        /// </summary>
        private List<Fragment> m_CurrentReceiveFragments;

        /// <summary>
        /// A list of fragments that are currently being sent.  Each of the fragments is used to track
        /// whether or not it has been acknowledged by the remote client.
        /// </summary>
        private List<Fragment> m_CurrentSendFragments;

        /// <summary>
        /// The current message that is being sent.
        /// </summary>
        private byte[] m_CurrentSendMessage;

        /// <summary>
        /// This keeps the footer fragment around in case part of the message has to be resent, but
        /// we've acknowledged the footer in the mean time.  When all fragments are in a received
        /// state, we'll verify any applicable checksum.
        /// </summary>
        private Fragment m_FooterFragment;

        /// <summary>
        /// Initializes a new instance of the <see cref="MxReliability"/> class.
        /// </summary>
        /// <param name="client">
        /// The client that this reliability class is associated with.
        /// </param>
        public MxReliability(MxClient client)
        {
            this.m_Client = client;

            this.m_QueuedMessages = new Queue<byte[]>();

            this.m_CurrentReceiveFragments = null;
            this.m_CurrentUnorderedReceiveFragments = new List<Fragment>();

            this.m_Client.MessageAcknowledged += this.ClientOnMessageAcknowledged;
            this.m_Client.MessageLost += this.ClientOnMessageLost;
            this.m_Client.MessageReceived += this.ClientOnMessageReceived;
        }

        /// <summary>
        /// Raised when a message has been acknowledged by the remote endpoint.
        /// </summary>
        public event MxMessageEventHandler MessageAcknowledged;

        /// <summary>
        /// Raised when a message has been received by this client.
        /// </summary>
        public event MxMessageEventHandler MessageReceived;

        /// <summary>
        /// Raised when a fragment has been received.
        /// </summary>
        public event MxReliabilityTransmitEventHandler FragmentReceived;

        /// <summary>
        /// Raised when a fragment has been acknowledged.
        /// </summary>
        public event MxReliabilityTransmitEventHandler FragmentSent;

        /// <summary>
        /// Represents a fragment's status.
        /// </summary>
        private enum FragmentStatus
        {
            /// <summary>
            /// This indicates the fragment is currently waiting to be sent.  This can
            /// either be because it's the first time the fragment is being sent, or we 
            /// received a MessageLost event and we are resending it.
            /// </summary>
            WaitingOnSend, 

            /// <summary>
            /// This indicates the fragment has been sent, but we are waiting on either an
            /// acknowledgement or lost event to be sent relating to this fragment.
            /// </summary>
            WaitingOnAcknowledgement, 

            /// <summary>
            /// This indicates the fragment has been acknowledged by the remote client.
            /// </summary>
            Acknowledged, 

            /// <summary>
            /// This indicates that we know about this fragment (because we have received the
            /// header indicating the number of fragments to expect), but we haven't received
            /// the content fragment yet.
            /// </summary>
            WaitingOnReceive, 

            /// <summary>
            /// This indicates the fragment has been received.
            /// </summary>
            Received
        }

        /// <summary>
        /// Sends data to the associated client reliably.
        /// </summary>
        /// <param name="data">
        /// The data to be sent.
        /// </param>
        public void Send(byte[] data)
        {
            this.m_QueuedMessages.Enqueue(data);
        }

        /// <summary>
        /// Updates this reliability class, sending and receiving messages as required.
        /// </summary>
        public void Update()
        {
            this.UpdateSend();
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
        /// Raise the FragmentReceived event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnFragmentReceived(MxReliabilityTransmitEventArgs e)
        {
            var handler = this.FragmentReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raise the OnFragmentSent event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnFragmentSent(MxReliabilityTransmitEventArgs e)
        {
            var handler = this.FragmentSent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when one of the fragments has been acknowledged by the remote client.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="mxMessageEventArgs">
        /// The message event args.
        /// </param>
        private void ClientOnMessageAcknowledged(object sender, MxMessageEventArgs mxMessageEventArgs)
        {
            var data = mxMessageEventArgs.Payload;

            var fragment = this.m_CurrentSendFragments.First(x => x.Data == data);
            var index = this.m_CurrentSendFragments.IndexOf(fragment);

            this.m_CurrentSendFragments[index].Status = FragmentStatus.Acknowledged;
        }

        /// <summary>
        /// Called when one of the fragments has been detected as lost.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="mxMessageEventArgs">
        /// The message event args.
        /// </param>
        private void ClientOnMessageLost(object sender, MxMessageEventArgs mxMessageEventArgs)
        {
            var data = mxMessageEventArgs.Payload;

            var fragment = this.m_CurrentSendFragments.First(x => x.Data == data);
            var index = this.m_CurrentSendFragments.IndexOf(fragment);

            // Mark the affected fragment as "WaitingForSend" so it will be resent.
            this.m_CurrentSendFragments[index].Status = FragmentStatus.WaitingOnSend;
        }

        /// <summary>
        /// Called when we have received a fragment from the underlying client.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="mxMessageEventArgs">
        /// The message event args.
        /// </param>
        private void ClientOnMessageReceived(object sender, MxMessageEventArgs mxMessageEventArgs)
        {
            var data = mxMessageEventArgs.Payload;

            // Read the first byte and work out what type of fragment this is.
            switch (data[0])
            {
                case 0:
                    if (this.m_CurrentReceiveFragments != null)
                    {
                        throw new InvalidOperationException(
                            "Can not start another receive " + this.m_CurrentReceiveFragments.Count(x => x.Status == FragmentStatus.Received));
                    }

                    // This is the header fragment.  Bytes 1 through 4 are the binary
                    // representation of a 32-bit integer.
                    var fragmentCount = BitConverter.ToInt32(data, 1);
                    this.m_CurrentReceiveFragments = new List<Fragment>();
                    for (var i = 0; i < fragmentCount; i++)
                    {
                        this.m_CurrentReceiveFragments.Add(new Fragment(null, FragmentStatus.WaitingOnReceive));
                    }

                    // Check our unordered fragments list.  We can have a scenario where the
                    // header is lost, but the rest (or some) of the fragments come through fine.
                    // In this scenario, because we don't have a header yet, the code under "case 1"
                    // will place the fragments in the unordered receive fragments list.  Now that we
                    // have the header, we need to take them out of that list and put them back together.
                    foreach (var unordered in this.m_CurrentUnorderedReceiveFragments)
                    {
                        // The first byte is the fragment type, which will be 1 for content fragments.
                        var rawData = unordered.Data;
                        var fragmentType = rawData[0];
                        if (fragmentType != 1)
                        {
                            throw new InvalidOperationException("Fragment type is not 1 for content fragment");
                        }

                        var fragmentSequence = BitConverter.ToInt32(rawData, 1);

                        var fragmentData = new byte[rawData.Length - 5];
                        for (var i = 0; i < rawData.Length - 5; i++)
                        {
                            fragmentData[i] = rawData[i + 5];
                        }

                        if (fragmentSequence >= this.m_CurrentReceiveFragments.Count)
                        {
                            throw new InvalidOperationException(
                                "Fragment sequence number is larger than total fragments");
                        }

                        this.m_CurrentReceiveFragments[fragmentSequence].Data = fragmentData;
                        this.m_CurrentReceiveFragments[fragmentSequence].Status = FragmentStatus.Received;
                    }

                    // Clear the unordered receive list, since we have now moved all the data from it.
                    this.m_CurrentUnorderedReceiveFragments.Clear();

                    break;
                case 1:
                    // This is a content fragment.  If the current receive fragments list is not null, then we have
                    // previously received the header fragment and can directly update the status in that list.
                    if (this.m_CurrentReceiveFragments != null)
                    {
                        var fragmentSequence = BitConverter.ToInt32(data, 1);

                        var fragmentData = new byte[data.Length - 5];
                        for (var i = 0; i < data.Length - 5; i++)
                        {
                            fragmentData[i] = data[i + 5];
                        }

                        if (fragmentSequence >= this.m_CurrentReceiveFragments.Count)
                        {
                            throw new InvalidOperationException(
                                "Fragment sequence number is larger than total fragments");
                        }

                        this.m_CurrentReceiveFragments[fragmentSequence].Data = fragmentData;
                        this.m_CurrentReceiveFragments[fragmentSequence].Status = FragmentStatus.Received;
                    }
                    else
                    {
                        // If the current receive list is null, then we haven't received (or acknowledged) the
                        // header fragment yet and we need to put it in the unordered list, because we don't
                        // know exactly how many fragments there are yet.
                        this.m_CurrentUnorderedReceiveFragments.Add(new Fragment(data, FragmentStatus.Received));
                    }

                    break;
                case 2:
                    // This is the footer fragment.  We just assign it straight to the footer fragment field, and
                    // the main Update() call will check whether or not we've received an entire message yet.
                    var footerData = new byte[data.Length - 1];
                    for (var i = 0; i < data.Length - 1; i++)
                    {
                        footerData[i] = data[i + 1];
                    }

                    this.m_FooterFragment = new Fragment(footerData, FragmentStatus.Received);
                    break;
                default:
                    throw new InvalidOperationException("Unknown fragment type!");
            }

            // Perform the receive update to fire events and finalize state if we have all the fragments.
            this.UpdateReceive();
        }

        /// <summary>
        /// Handles receiving fragments and firing the MessageReceived event when reconstruction is complete.
        /// </summary>
        private void UpdateReceive()
        {
            if (this.m_CurrentReceiveFragments != null)
            {
                this.OnFragmentReceived(new MxReliabilityTransmitEventArgs
                {
                    Client = this.m_Client,
                    CurrentFragments = this.m_CurrentReceiveFragments.Count(x => x.Status == FragmentStatus.Received),
                    TotalFragments = this.m_CurrentReceiveFragments.Count,
                    IsSending = false,
                    TotalSize = this.m_CurrentReceiveFragments.Count * SafeFragmentSize // TODO: Is this accurate?
                });

                // See if all of the fragments are in a received status.
                if (this.m_CurrentReceiveFragments.All(x => x.Status == FragmentStatus.Received))
                {
                    // See if the footer fragment is present.
                    if (this.m_FooterFragment != null)
                    {
                        // Then we have received a message!  The data field in the fragments is actually
                        // the original message data (with the fragment headers already removed), so we
                        // can concatenate the arrays in the fragments and fire the message received event.
                        var data = this.m_CurrentReceiveFragments.SelectMany(x => x.Data).ToArray();
                        this.OnMessageReceived(new MxMessageEventArgs { Client = this.m_Client, Payload = data });

                        // Delete the received list.
                        this.m_CurrentReceiveFragments = null;
                    }
                }
            }
        }

        /// <summary>
        /// Handles sending out fragments based on the messages in the queue.
        /// </summary>
        private void UpdateSend()
        {
            if (this.m_CurrentSendFragments == null)
            {
                // We are currently not sending a message.  Check to see if there
                // are any queued messages and if so kick off the process.
                if (this.m_QueuedMessages.Count == 0)
                {
                    return;
                }

                var packet = this.m_QueuedMessages.Dequeue();

                // Fragment the packet up into (SafeFragmentSize - 1 - 5) byte chunks (with a boolean and 32-bit int header).
                var fragments = new List<Fragment>();
                for (var i = 0; i < packet.Length; i += SafeFragmentSize - 1 - 5)
                {
                    var length = Math.Min(SafeFragmentSize - 1 - 5, packet.Length - i);
                    var fragment = new byte[length + 5];
                    var header = BitConverter.GetBytes(fragments.Count);
                    fragment[0] = 1; // 1 == content
                    fragment[1] = header[0];
                    fragment[2] = header[1];
                    fragment[3] = header[2];
                    fragment[4] = header[3];
                    for (var idx = 0; idx < length; idx++)
                    {
                        fragment[idx + 5] = packet[i + idx];
                    }

                    fragments.Add(new Fragment(fragment, FragmentStatus.WaitingOnSend));
                }

                // Create the real list with header and footer fragment.
                // 0 == header, 2 == footer
                var headerBytes = BitConverter.GetBytes(fragments.Count);
                var headerFragment = new byte[] { 0, headerBytes[0], headerBytes[1], headerBytes[2], headerBytes[3] };
                var footerFragment = new byte[] { 2 };
                this.m_CurrentSendFragments = new List<Fragment>();
                this.m_CurrentSendFragments.Add(new Fragment(headerFragment, FragmentStatus.WaitingOnSend));
                this.m_CurrentSendFragments.AddRange(fragments);
                this.m_CurrentSendFragments.Add(new Fragment(footerFragment, FragmentStatus.WaitingOnSend));
                this.m_CurrentSendMessage = packet;
            }

            // Iterate through all of the fragments in the list, and call Send() on the client.
            // Since the client is in reliable mode, it will send exactly one fragment at a time, thus
            // reducing the loss over UDP.
            foreach (var fragment in this.m_CurrentSendFragments)
            {
                if (fragment.Status == FragmentStatus.WaitingOnSend)
                {
                    this.m_Client.EnqueueSend(fragment.Data);
                    fragment.Status = FragmentStatus.WaitingOnAcknowledgement;
                }
            }

            this.OnFragmentReceived(new MxReliabilityTransmitEventArgs
            {
                Client = this.m_Client,
                CurrentFragments = this.m_CurrentSendFragments.Count(x => x.Status == FragmentStatus.Acknowledged),
                TotalFragments = this.m_CurrentSendFragments.Count,
                IsSending = true,
                TotalSize = this.m_CurrentSendMessage.Length
            });

            // Now check to see if all fragments are in an acknowledged state.  If they are, this message
            // has been delivered successfully and we can ready ourselves for the next message.
            if (this.m_CurrentSendFragments.All(x => x.Status == FragmentStatus.Acknowledged))
            {
                this.m_CurrentSendFragments = null;
                this.OnMessageAcknowledged(
                    new MxMessageEventArgs { Client = this.m_Client, Payload = this.m_CurrentSendMessage });
            }
        }

        /// <summary>
        /// Represents a fragment of data being sent over the network.  This is used to
        /// track what fragments have been received / sent.
        /// </summary>
        private class Fragment
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Fragment"/> class.
            /// </summary>
            /// <param name="data">
            /// The raw data of the fragment.
            /// </param>
            /// <param name="status">
            /// The fragment status.
            /// </param>
            public Fragment(byte[] data, FragmentStatus status)
            {
                this.Data = data;
                this.Status = status;
            }

            /// <summary>
            /// Gets or sets the raw data of the fragment.
            /// </summary>
            /// <value>
            /// The raw data of the fragment.
            /// </value>
            public byte[] Data { get; set; }

            /// <summary>
            /// Gets or sets the status.
            /// </summary>
            /// <value>
            /// The status.
            /// </value>
            public FragmentStatus Status { get; set; }
        }
    }
}