namespace Protogame
{
    using ProtoBuf;

    /// <summary>
    /// Encapsulates an Mx payload.  We have a seperate class since we need
    /// an array of byte arrays, which can't be represented directly in Protobuf.
    /// </summary>
    [ProtoContract]
    public class MxPayload
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data of the payload.
        /// </value>
        [ProtoMember(1)]
        public byte[] Data { get; set; }
    }
}