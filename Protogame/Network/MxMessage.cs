namespace Protogame
{
    using System;
    using ProtoBuf;

    /// <summary>
    /// The message class that represents a message being transferred over the Mx protocol.
    /// </summary>
    [ProtoContract]
    public class MxMessage
    {
        /// <summary>
        /// Gets or sets the latest sequence number this packet is acknowledging.
        /// </summary>
        /// <value>
        /// The latest sequence number this packet is acknowledging.
        /// </value>
        [ProtoMember(3)]
        public uint Ack { get; set; }

        /// <summary>
        /// Gets or sets the ack bitfield, which represents of the last 32 acks, which have been acknowledged.
        /// </summary>
        /// <value>
        /// The ack bitfield, which represents of the last 32 acks, which have been acknowledged.
        /// </value>
        [ProtoMember(4)]
        public uint AckBitfield { get; set; }

        /// <summary>
        /// Gets or sets the payloads for this message.
        /// </summary>
        /// <value>
        /// The payloads associated with this message.
        /// </value>
        [ProtoMember(6)]
        public MxPayload[] Payloads { get; set; }

        /// <summary>
        /// Gets or sets the protocol ID.
        /// </summary>
        /// <value>
        /// The protocol ID.
        /// </value>
        [ProtoMember(1)]
        public uint ProtocolID { get; set; }

        /// <summary>
        /// Gets or sets the sequence number.
        /// </summary>
        /// <value>
        /// The sequence number.
        /// </value>
        [ProtoMember(2)]
        public uint Sequence { get; set; }

        /// <summary>
        /// Returns whether this message did acknowledge the specified sequence number.
        /// </summary>
        /// <param name="sequence">
        /// The sequence number to check.
        /// </param>
        /// <returns>
        /// Whether this message acknowledges the specified sequence number.
        /// </returns>
        public bool DidAck(uint sequence)
        {
            if (!this.HasAck(sequence))
            {
                return false;
            }

            var diff = MxUtility.GetSequenceNumberDifference(sequence, this.Ack);

            return ((this.AckBitfield & (0x1u << (int)diff)) >> (int)diff) == 0x1;
        }

        /// <summary>
        /// Converts the bitfield into an array of <see cref="bool"/>.
        /// </summary>
        /// <returns>
        /// The array of <see cref="bool"/> representing the acknowledgement status.
        /// </returns>
        public bool[] GetAckBitfield()
        {
            var bitfield = new bool[MxUtility.UIntBitsize];
            for (var i = 0; i < bitfield.Length; i++)
            {
                if (((this.AckBitfield & (0x1u << i)) >> i) == 0x1)
                {
                    bitfield[i] = true;
                }
            }

            return bitfield;
        }

        /// <summary>
        /// Returns whether or not this message can acknowledge the specified sequence number.
        /// This will return false if the specified sequence number is more than 32 messages ago.
        /// </summary>
        /// <param name="sequence">
        /// The sequence number to check.
        /// </param>
        /// <returns>
        /// Whether this message can acknowledge the specified sequence number.
        /// </returns>
        public bool HasAck(uint sequence)
        {
            return MxUtility.GetSequenceNumberDifference(sequence, this.Ack) < MxUtility.UIntBitsize;
        }

        /// <summary>
        /// Sets the bitfield based on an input array of <see cref="bool"/>.
        /// </summary>
        /// <param name="received">
        /// The array of <see cref="bool"/> representing the acknowledgement status.
        /// </param>
        public void SetAckBitfield(bool[] received)
        {
            this.AckBitfield = 0;
            for (var i = 0; i < Math.Min(received.Length, MxUtility.UIntBitsize); i++)
            {
                if (received[i])
                {
                    this.AckBitfield = this.AckBitfield | (0x1u << i);
                }
            }
        }
    }
}