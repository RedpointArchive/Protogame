namespace Protogame
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents the current send state of a reliable message.
    /// </summary>
    public class MxReliabilitySendState
    {
        /// <summary>
        /// A list of fragments that are currently being sent.  Each of the fragments is used to track
        /// whether or not it has been acknowledged by the remote client.
        /// </summary>
        public List<Fragment> CurrentSendFragments { get; set; }

        /// <summary>
        /// The current message that is being sent.
        /// </summary>
        public byte[] CurrentSendMessage { get; set; }

        /// <summary>
        /// The ID of the message we are currently sending.
        /// </summary>
        public int CurrentSendMessageID { get; set; }

        public MxReliabilitySendState()
        {
            this.CurrentSendFragments = new List<Fragment>();
        }

        public bool MarkFragmentIfPresent(byte[] data, FragmentStatus newStatus)
        {
            var fragment = this.CurrentSendFragments.FirstOrDefault(x => x.Data == data);
            if (fragment == null)
            {
                return false;
            }

            var index = this.CurrentSendFragments.IndexOf(fragment);

            this.CurrentSendFragments[index].Status = newStatus;

            return true;
        }

        public bool HasPendingSends()
        {
            return this.CurrentSendFragments.Any(x => x.Status == FragmentStatus.WaitingOnSend);
        }
    }
}