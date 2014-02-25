namespace Protogame
{
    /// <summary>
    /// Represents a fragment of data being sent over the network.  This is used to
    /// track what fragments have been received / sent.
    /// </summary>
    public class Fragment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Fragment"/> class.
        /// </summary>
        /// <param name="data">
        /// The raw data of the fragment.
        /// </param>
        /// <param name="status">
        /// The fragment status.
        /// </param>
        public Fragment(byte[] data, FragmentStatus status)
        {
            this.Data = data;
            this.Status = status;
        }

        /// <summary>
        /// Gets or sets the raw data of the fragment.
        /// </summary>
        /// <value>
        /// The raw data of the fragment.
        /// </value>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public FragmentStatus Status { get; set; }
    }
}