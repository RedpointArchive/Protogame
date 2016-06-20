namespace Protogame
{
    /// <summary>
    /// Represents a fragment's status.
    /// </summary>
    /// <module>Network</module>
    public enum FragmentStatus
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
}