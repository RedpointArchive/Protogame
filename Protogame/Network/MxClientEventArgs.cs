namespace Protogame
{
    using System;

    /// <summary>
    /// The mx client event args.
    /// </summary>
    public class MxClientEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public MxClient Client { get; set; }
    }
}