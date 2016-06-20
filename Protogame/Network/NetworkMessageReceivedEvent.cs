namespace Protogame
{
    public class NetworkMessageReceivedEvent : NetworkEvent
    {
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