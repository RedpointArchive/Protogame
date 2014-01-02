namespace Protogame
{
    using System;

    /// <summary>
    /// The mx message event args.
    /// </summary>
    public class MxReliabilityTransmitEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public MxClient Client { get; set; }

        public bool IsSending { get; set; }

        public int CurrentFragments { get; set; }

        public int TotalFragments { get; set; }

        public int TotalSize { get; set; }
    }
}