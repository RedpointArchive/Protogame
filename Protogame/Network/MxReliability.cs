namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
        /// The data offset.
        /// </summary>
        private const int DataOffset = 8;

        /// <summary>
        /// The associated <see cref="MxClient"/> that data is sent and received through.
        /// </summary>
        private readonly MxClient m_Client;

        /// <summary>
        /// A list of queued messages that are waiting to be sent.
        /// </summary>
        private readonly Queue<byte[]> m_QueuedMessages;

        /// <summary>
        /// A list of active messages that are currently in the process of being sent.
        /// </summary>
        private readonly List<MxReliabilitySendState> m_ActiveMessages;

        private readonly Dictionary<int, MxReliabilityReceiveState> m_ActiveReceiveMessages; 

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

            this.m_ActiveReceiveMessages = new Dictionary<int, MxReliabilityReceiveState>();

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
            if (mxMessageEventArgs.ProtocolID != MxMessage.ReliableProtocol)
            {
                return;
            }

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
            if (mxMessageEventArgs.ProtocolID != MxMessage.ReliableProtocol)
            {
                return;
            }

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
            if (mxMessageEventArgs.ProtocolID != MxMessage.ReliableProtocol)
            {
                return;
            }

            var data = mxMessageEventArgs.Payload;

            // Format of a packet is:
            // Byte 0 - 3: message ID
            // Byte 4 - 5: length
            // Byte 6: current index
            // Byte 7: total packets
            // Remaining bytes are the fragment data
            var messageID = BitConverter.ToInt32(data, 0);
            var length = BitConverter.ToUInt16(data, 4);
            var currentIndex = (int)data[6];
            var totalPackets = (int)data[7];

            if (!this.m_ActiveReceiveMessages.ContainsKey(messageID))
            {
                // Add a new entry in the active receive messages dictionary.
                this.m_ActiveReceiveMessages.Add(
                    messageID,
                    new MxReliabilityReceiveState(messageID, totalPackets));
            }

            // Check for invalid payload.
            if ((length - 1) + DataOffset >= data.Length)
            {
                var errorMessage = 
                    "The payload data received had an invalid length for the " +
                    "data that was present. ";
                errorMessage += "\r\n";
                errorMessage += "\r\n";
                errorMessage += "Client Endpoint: " + mxMessageEventArgs.Client.Endpoint + "\r\n";
                errorMessage += "Message ID: " + messageID + "\r\n";
                errorMessage += "Message Protocol: " + mxMessageEventArgs.ProtocolID + "\r\n";
                errorMessage += "Length: " + length + "\r\n";
                errorMessage += "Current Index: " + currentIndex + "\r\n";
                errorMessage += "Total Packets: " + totalPackets + "\r\n";
                errorMessage += "Payload Length: " + data.Length + "\r\n";
                errorMessage += "Payload Length minus Offset: " + (data.Length - DataOffset) + "\r\n";

                var exception = new InvalidDataException(errorMessage);
                exception.Data.Add("Payload", data);

                throw exception;
            }

            // Extract the fragment data.
            var fragmentData = new byte[length];
            for (var i = 0; i < length; i++)
            {
                fragmentData[i] = data[i + DataOffset];
            }

            // Set the fragment data into the received message.
            this.m_ActiveReceiveMessages[messageID].SetFragment(currentIndex, new Fragment(fragmentData, FragmentStatus.Received));

            // Perform the receive update to fire events and finalize state if we have all the fragments.
            this.UpdateReceive();
        }

        /// <summary>
        /// Handles receiving fragments and firing the MessageReceived event when reconstruction is complete.
        /// </summary>
        private void UpdateReceive()
        {
            var pendingRemove = new List<int>();

            foreach (var activeReceive in this.m_ActiveReceiveMessages)
            {
                if (activeReceive.Value.IsComplete())
                {
                    this.OnMessageReceived(
                        new MxMessageEventArgs
                        {
                            Client = this.m_Client, 
                            Payload = activeReceive.Value.Reconstruct(),
                            ProtocolID = MxMessage.ReliableProtocol
                        });

                    pendingRemove.Add(activeReceive.Key);
                }
            }

            foreach (var i in pendingRemove)
            {
                this.m_ActiveReceiveMessages.Remove(i);
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
            var total = 0;
            for (var i = 0; i < packet.Length; i += SafeFragmentSize - DataOffset)
            {
                total += 1;
            }

            if (total >= byte.MaxValue)
            {
                throw new OverflowException(
                    "The input data was " + packet.Length
                    + " bytes in length which is larger than the maximum " + ((SafeFragmentSize - DataOffset) * (byte.MaxValue - 1))
                    + " bytes.");
            }

            var inc = 0;
            for (var i = 0; i < packet.Length; i += SafeFragmentSize - DataOffset)
            {
                var length = Math.Min(SafeFragmentSize - DataOffset, packet.Length - i);
                var fragment = new byte[length + DataOffset];
                var messageIDBytes = BitConverter.GetBytes(sendState.CurrentSendMessageID);
                var lengthBytes = BitConverter.GetBytes((UInt16)length);

                fragment[0] = messageIDBytes[0];
                fragment[1] = messageIDBytes[1];
                fragment[2] = messageIDBytes[2];
                fragment[3] = messageIDBytes[3];
                fragment[4] = lengthBytes[0];
                fragment[5] = lengthBytes[1];
                fragment[6] = (byte)inc;
                fragment[7] = (byte)total;
                for (var idx = 0; idx < length; idx++)
                {
                    fragment[idx + DataOffset] = packet[i + idx];
                }

                fragments.Add(new Fragment(fragment, FragmentStatus.WaitingOnSend));
                inc++;
            }

            sendState.CurrentSendFragments = fragments;

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
                            this.m_Client.EnqueueSend(fragment.Data, MxMessage.ReliableProtocol);
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
                        new MxMessageEventArgs
                        {
                            Client = this.m_Client, 
                            Payload = message.CurrentSendMessage,
                            ProtocolID = MxMessage.ReliableProtocol
                        });

                    this.m_ActiveMessages.Remove(message);
                }
            }
        }
    }
}