namespace Protogame
{
    using System;

    /// <summary>
    /// The flow control changed event args.
    /// </summary>
    /// <module>Network</module>
    public class FlowControlChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether flow control is currently in good send mode.
        /// </summary>
        /// <value>
        /// Whether flow control is in good send mode.
        /// </value>
        public bool IsGoodSendMode { get; set; }

        /// <summary>
        /// Gets or sets the penalty time.
        /// </summary>
        /// <value>
        /// The penalty time.
        /// </value>
        public double PenaltyTime { get; set; }
    }
}