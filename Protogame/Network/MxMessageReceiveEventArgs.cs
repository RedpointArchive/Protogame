namespace Protogame
{
    /// <summary>
    /// The mx message event args.
    /// </summary>
    public class MxMessageReceiveEventArgs : MxMessageEventArgs
    {
        /// <summary>
        /// Gets or sets whether the client should not acknowledge this message.
        /// </summary>
        /// <value>
        /// Set to true if the client should not acknowledge this message
        /// has been received.  This can be used when the packet is
        /// invalid or the receiver has no way of determining what to do with
        /// it.
        /// </value>
        public bool DoNotAcknowledge { get; set; }
    }
}