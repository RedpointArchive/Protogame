namespace Protogame
{
    using ProtoBuf;

    /// <summary>
    /// The platform data.
    /// </summary>
    [ProtoContract]
    public class PlatformData
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [ProtoMember(2)]
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the platform.
        /// </summary>
        /// <value>
        /// The platform.
        /// </value>
        [ProtoMember(1)]
        public TargetPlatform Platform { get; set; }
    }
}