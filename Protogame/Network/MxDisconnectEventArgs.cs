namespace Protogame
{
    using System;

    /// <summary>
    /// The mx client event args.
    /// </summary>
    public class MxDisconnectEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public MxClient Client { get; set; }

        /// <summary>
        /// Gets or sets the disconnect accumulator.
        /// </summary>
        /// <value>
        /// The disconnect accumulator.
        /// </value>
        public int DisconnectAccumulator { get; set; }

        /// <summary>
        /// Gets or sets the disconnect timeout.
        /// </summary>
        /// <value>
        /// The disconnect timeout.
        /// </value>
        public int DisconnectTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the client has disconnected.
        /// </summary>
        /// <value>
        /// Whether or not the client has disconnected.
        /// </value>
        public bool IsDisconnected { get; set; }
    }
}