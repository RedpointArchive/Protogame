namespace Protogame
{
    using ProtoBuf;

    /// <summary>
    /// The compiled asset.
    /// </summary>
    [ProtoContract]
    public class CompiledAsset
    {
        /// <summary>
        /// Gets or sets the loader.
        /// </summary>
        /// <value>
        /// The loader.
        /// </value>
        [ProtoMember(1)]
        public string Loader { get; set; }

        /// <summary>
        /// Gets or sets the platform data.
        /// </summary>
        /// <value>
        /// The platform data.
        /// </value>
        [ProtoMember(3)]
        public PlatformData PlatformData { get; set; }
    }
}