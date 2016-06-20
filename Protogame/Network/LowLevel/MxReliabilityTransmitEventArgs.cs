namespace Protogame
{
    using System;

    /// <summary>
    /// The mx message event args.
    /// </summary>
    /// <module>Network</module>
    public class MxReliabilityTransmitEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public MxClient Client { get; set; }

        /// <summary>
        /// Gets or sets the current fragments.
        /// </summary>
        /// <value>
        /// The current fragments.
        /// </value>
        public int CurrentFragments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is sending.
        /// </summary>
        /// <value>
        /// The is sending.
        /// </value>
        public bool IsSending { get; set; }

        /// <summary>
        /// Gets or sets the total fragments.
        /// </summary>
        /// <value>
        /// The total fragments.
        /// </value>
        public int TotalFragments { get; set; }

        /// <summary>
        /// Gets or sets the total size.
        /// </summary>
        /// <value>
        /// The total size.
        /// </value>
        public int TotalSize { get; set; }
    }
}