namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

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
        /// The data offset.
        /// </summary>
        private const int DataOffset = 6;

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
        /// A list of active messages that are currently in the process of being sent.
        /// </summary>
        private readonly List<MxReliabilitySendState> m_ActiveMessages; 

        /// <summary>
        /// A list of currently received fragments.  Unlike the current send fragments, this
        /// does not include the header fragments.
        /// </summary>
        private List<Fragment> m_CurrentReceiveFragments;

        /// <summary>
        /// The ID of the message we are currently receiving; this binds packets to the original
        /// header so that in the case of duplicated sends we can't get packets that aren't associated
        /// with the given header.
        /// </summary>
        private byte m_CurrentReceivingMessageID;

        /// <summary>
        /// The ID of the next message to be sent.
        /// </summary>
        private byte m_CurrentSendMessageIDCounter;

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
            this.m_ActiveMessages = new List<MxReliabilitySendState>();

            this.m_CurrentReceiveFragments = null;
            this.m_CurrentUnorderedReceiveFragments = new List<Fragment>();

            this.m_Client.MessageAcknowledged += this.ClientOnMessageAcknowledged;
            this.m_Client.MessageLost += this.ClientOnMessageLost;
            this.m_Client.MessageReceived += this.ClientOnMessageReceived;
        }

        /// <summary>
        /// Raised when a fragment has been received.
        /// </summary>
        public event MxReliabilityTransmitEventHandler FragmentReceived;

        /// <summary>
        /// Raised when a fragment has been acknowledged.
        /// </summary>
        public event MxReliabilityTransmitEventHandler FragmentSent;

        /// <summary>
        /// Raised when a message has been acknowledged by the remote endpoint.
        /// </summary>
        public event MxMessageEventHandler MessageAcknowledged;

        /// <summary>
        /// Raised when a message has been received by this client.
        /// </summary>
        public event MxMessageEventHandler MessageReceived;

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

            foreach (var message in this.m_ActiveMessages)
            {
                if (message.MarkFragmentIfPresent(data, FragmentStatus.Acknowledged))
                {
                    break;
                }
            }
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

            foreach (var message in this.m_ActiveMessages)
            {
                if (message.MarkFragmentIfPresent(data, FragmentStatus.WaitingOnSend))
                {
                    break;
                }
            }
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
        private void ClientOnMessageReceived(object sender, MxMessageReceiveEventArgs mxMessageEventArgs)
        {
            var data = mxMessageEventArgs.Payload;

            // Read the first byte and work out what type of fragment this is.
            switch (data[0])
            {
                case 0:
                    if (this.m_CurrentReceiveFragments != null)
                    {
                        var message = "Can not start receiving another message.";
                        message += "  Currently there are "
                                   + this.m_CurrentReceiveFragments.Count(x => x.Status == FragmentStatus.Received)
                                   + " received.";
                        message += "  Currently there are " + this.m_CurrentReceiveFragments.Count + " in total.";

                        Console.WriteLine(message);

                        mxMessageEventArgs.DoNotAcknowledge = true;
                        return;
                    }

                    // This is the header fragment.  Bytes 1 through 4 are the binary
                    // representation of a 32-bit integer.  Byte 5 is a single byte
                    // representing the message ID.
                    var fragmentCount = BitConverter.ToInt32(data, 1);
                    var initialMessageID = data[5];
                    this.m_CurrentReceiveFragments = new List<Fragment>();
                    for (var i = 0; i < fragmentCount; i++)
                    {
                        this.m_CurrentReceiveFragments.Add(new Fragment(null, FragmentStatus.WaitingOnReceive));
                    }

                    this.m_CurrentReceivingMessageID = initialMessageID;

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
                        var unorderedMessageID = rawData[5];

                        // Skip any unordered fragments that don't belong the header.
                        if (unorderedMessageID != this.m_CurrentReceivingMessageID)
                        {
                            continue;
                        }

                        var fragmentData = new byte[rawData.Length - DataOffset];
                        for (var i = 0; i < rawData.Length - DataOffset; i++)
                        {
                            fragmentData[i] = rawData[i + DataOffset];
                        }

                        if (fragmentSequence >= this.m_CurrentReceiveFragments.Count)
                        {
                            // "Fragment sequence number is larger than total fragments"
                            mxMessageEventArgs.DoNotAcknowledge = true;
                            break;
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

                        var fragmentData = new byte[data.Length - DataOffset];
                        for (var i = 0; i < data.Length - DataOffset; i++)
                        {
                            fragmentData[i] = data[i + DataOffset];
                        }

                        if (fragmentSequence >= this.m_CurrentReceiveFragments.Count)
                        {
                            // "Fragment sequence number is larger than total fragments"
                            mxMessageEventArgs.DoNotAcknowledge = true;
                            break;
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
                case 3:

                    // This is a special, single fragment.  It is not preceded by a header.  We can raise the message
                    // received event immediately as this does not rely on
                    // reconstruction of fragments and does not impact current receiving state (so you could theoretically
                    // even send them in the middle of a transmission of a fragmented message).
                    var singleFragmentData = new byte[data.Length - DataOffset];
                    for (var i = 0; i < data.Length - DataOffset; i++)
                    {
                        singleFragmentData[i] = data[i + DataOffset];
                    }

                    this.OnMessageReceived(
                        new MxMessageEventArgs { Client = this.m_Client, Payload = singleFragmentData });
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
                this.OnFragmentReceived(
                    new MxReliabilityTransmitEventArgs
                    {
                        Client = this.m_Client, 
                        CurrentFragments =
                            this.m_CurrentReceiveFragments.Count(x => x.Status == FragmentStatus.Received), 
                        TotalFragments = this.m_CurrentReceiveFragments.Count, 
                        IsSending = false, 
                        TotalSize = this.m_CurrentReceiveFragments.Count * SafeFragmentSize // TODO: Is this accurate?
                    });

                // See if all of the fragments are in a received status.
                if (this.m_CurrentReceiveFragments.All(x => x.Status == FragmentStatus.Received))
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

        /// <summary>
        /// Handles sending out fragments based on the messages in the queue.
        /// </summary>
        private void UpdateSend()
        {
            // Update existing active messages before doing anything else; if we
            // send something for an existing active message then we don't go
            // any further.
            if (this.UpdateSendActiveMessages())
            {
                return;
            }

            // Re-evaluate the active messages to find and finalize any that
            // have been full acknowledged.
            this.CheckForFullAcknowledgement();

            // If we get to here, then there were no active message that had
            // sends pending, so rather than wasting a cycle doing nothing, we
            // start sending out the next message by bringing a queued message
            // into the active list.

            // Make sure there are messages in the queue; if there aren't any
            // then we have nothing left to do.
            if (this.m_QueuedMessages.Count == 0)
            {
                return;
            }

            // Dequeue the next message.
            var packet = this.m_QueuedMessages.Dequeue();
            
            // Create the new send state that we're going to use to track this message.
            var sendState = new MxReliabilitySendState();

            // Assign the send state a unique send message ID and then increment our counter.
            sendState.CurrentSendMessageID = this.m_CurrentSendMessageIDCounter++;

            // Assign the send state the packet data.
            sendState.CurrentSendMessage = packet;

            // Fragment the packet up into (SafeFragmentSize - DataOffset) byte chunks.
            var fragments = new List<Fragment>();
            for (var i = 0; i < packet.Length; i += SafeFragmentSize - DataOffset)
            {
                var length = Math.Min(SafeFragmentSize - DataOffset, packet.Length - i);
                var fragment = new byte[length + DataOffset];
                var header = BitConverter.GetBytes(fragments.Count);
                fragment[0] = 1; // 1 == content
                fragment[1] = header[0];
                fragment[2] = header[1];
                fragment[3] = header[2];
                fragment[4] = header[3];
                fragment[5] = sendState.CurrentSendMessageID;
                for (var idx = 0; idx < length; idx++)
                {
                    fragment[idx + DataOffset] = packet[i + idx];
                }

                fragments.Add(new Fragment(fragment, FragmentStatus.WaitingOnSend));
            }

            // If there is only one packet to send, make an optimization and send the packet
            // with a prefix of "3" (instead of "1"), and then skip the header.
            if (fragments.Count == 1)
            {
                // Change the first byte.
                fragments[0].Data[0] = 3;

                // Create the list with just one fragment to send.
                sendState.CurrentSendFragments = new List<Fragment>();
                sendState.CurrentSendFragments.Add(fragments[0]);
            }
            else
            {
                // Create the real list with header fragment.
                // 0 == header
                var headerBytes = BitConverter.GetBytes(fragments.Count);
                var headerFragment = new byte[]
                    {
                       0, headerBytes[0], headerBytes[1], headerBytes[2], headerBytes[3], sendState.CurrentSendMessageID 
                    };
                sendState.CurrentSendFragments = new List<Fragment>();
                sendState.CurrentSendFragments.Add(new Fragment(headerFragment, FragmentStatus.WaitingOnSend));
                sendState.CurrentSendFragments.AddRange(fragments);
            }

            // Add the new send state to the end of the list of active messages.
            this.m_ActiveMessages.Add(sendState);

            // Call UpdateSendActiveMessages again so that the new message gets queued
            // this step.
            this.UpdateSendActiveMessages();

            // Fire the progress event which is now an aggregate of all messages
            // that are waiting to be sent.
            this.OnFragmentReceived(
                new MxReliabilityTransmitEventArgs
                {
                    Client = this.m_Client, 
                    CurrentFragments = this.m_ActiveMessages.SelectMany(x => x.CurrentSendFragments).Count(x => x.Status == FragmentStatus.Acknowledged), 
                    TotalFragments = this.m_ActiveMessages.SelectMany(x => x.CurrentSendFragments).Count(), 
                    IsSending = true,
                    TotalSize = this.m_ActiveMessages.Sum(x => x.CurrentSendMessage.Length)
                });
        }

        /// <summary>
        /// Attempt to queue packets for the current active messages that need sending and
        /// returns whether it queued anything.  If it didn't, then we return false and
        /// the caller (UpdateSend) can pick up another active message.
        /// </summary>
        /// <returns>Whether any messages were queued into the client.</returns>
        private bool UpdateSendActiveMessages()
        {
            foreach (var message in this.m_ActiveMessages)
            {
                if (message.HasPendingSends())
                {
                    // Iterate through all of the fragments in the list, and call Send() on the client.
                    // Since the client is in reliable mode, it will send exactly one fragment at a time, thus
                    // reducing the loss over UDP.
                    foreach (var fragment in message.CurrentSendFragments)
                    {
                        if (fragment.Status == FragmentStatus.WaitingOnSend)
                        {
                            this.m_Client.EnqueueSend(fragment.Data);
                            fragment.Status = FragmentStatus.WaitingOnAcknowledgement;
                        }
                    }

                    // Re-evaluate the active messages to find and finalize any that
                    // have been full acknowledged.
                    this.CheckForFullAcknowledgement();

                    // We only deal with one active message per update.
                    return true;
                }
            }

            // We didn't send any message this update.
            return false;
        }

        /// <summary>
        /// Checks for and finishes any active messages that have now been completely acknowledged
        /// by the receiver.
        /// </summary>
        private void CheckForFullAcknowledgement()
        {
            foreach (var message in this.m_ActiveMessages.ToList())
            {
                // Now check to see if all fragments are in an acknowledged state.  If they are, this message
                // has been delivered successfully and we can ready ourselves for the next message.
                if (message.CurrentSendFragments.All(x => x.Status == FragmentStatus.Acknowledged))
                {
                    this.OnMessageAcknowledged(
                        new MxMessageEventArgs { Client = this.m_Client, Payload = message.CurrentSendMessage });

                    this.m_ActiveMessages.Remove(message);
                }
            }
        }
    }
}