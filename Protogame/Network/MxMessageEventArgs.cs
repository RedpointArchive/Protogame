namespace Protogame
{
    using System;

    /// <summary>
    /// The mx message event args.
    /// </summary>
    public class MxMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public MxClient Client { get; set; }

        /// <summary>
        /// Gets or sets the payload of the message.
        /// </summary>
        /// <value>
        /// The payload of the message.
        /// </value>
        public byte[] Payload { get; set; }

        /// <summary>
        /// Gets or sets the protocol ID of the message.
        /// </summary>
        /// <value>
        /// The protocol ID of the message.
        /// </value>
        public uint ProtocolID { get; set; }
    }
}