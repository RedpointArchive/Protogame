﻿namespace Protogame
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents the current receive state of a reliable message.
    /// </summary>
    /// <module>Network</module>
    public class MxReliabilityReceiveState
    {
        /// <summary>
        /// A list of currently received fragments.  Unlike the current send fragments, this
        /// does not include the header fragments.
        /// </summary>
        private readonly Fragment[] m_CurrentReceiveFragments;
        
        public MxReliabilityReceiveState(int messageID, int totalPackets)
        {
            this.m_CurrentReceiveFragments = new Fragment[totalPackets];
            this.CurrentReceiveMessageID = messageID;
            this.TotalFragments = totalPackets;

            for (var i = 0; i < totalPackets; i++)
            {
                this.m_CurrentReceiveFragments[i] = new Fragment(null, FragmentStatus.WaitingOnReceive);
            }
        }

        /// <summary>
        /// The ID of the message we are currently receiving; this binds packets to the original
        /// header so that in the case of duplicated sends we can't get packets that aren't associated
        /// with the given header.
        /// </summary>
        /// <value>
        /// The current receive message id.
        /// </value>
        public int CurrentReceiveMessageID { get; private set; }

        /// <summary>
        /// The total number of fragments that this message contains.
        /// </summary>
        public int TotalFragments { get; private set; }
        
        public bool IsComplete()
        {
            return this.m_CurrentReceiveFragments.All(x => x.Status == FragmentStatus.Received);
        }
        
        public byte[] Reconstruct()
        {
            var bytes = new byte[this.m_CurrentReceiveFragments.Sum(x => x.Data.Length)];
            var idx = 0;
            for (var i = 0; i < this.m_CurrentReceiveFragments.Length; i++)
            {
                for (var a = 0; a < this.m_CurrentReceiveFragments[i].Data.Length; a++)
                {
                    bytes[idx] = this.m_CurrentReceiveFragments[i].Data[a];
                    idx++;
                }
            }
            return bytes;
        }
        
        public void SetFragment(int currentIndex, Fragment fragment)
        {
            if (currentIndex >= this.TotalFragments)
            {
                throw new InvalidOperationException("Message data is corrupt!");
            }

            this.m_CurrentReceiveFragments[currentIndex] = fragment;
        }
    }
}