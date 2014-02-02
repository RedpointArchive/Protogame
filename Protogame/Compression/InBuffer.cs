// This is a copy of the 7-zip LZMA
// compression library.
namespace Protogame.Compression
{
    using System.IO;

    /// <summary>
    /// The in buffer.
    /// </summary>
    public class InBuffer
    {
        /// <summary>
        /// The m_ buffer.
        /// </summary>
        private readonly byte[] m_Buffer;

        /// <summary>
        /// The m_ buffer size.
        /// </summary>
        private readonly uint m_BufferSize;

        /// <summary>
        /// The m_ limit.
        /// </summary>
        private uint m_Limit;

        /// <summary>
        /// The m_ pos.
        /// </summary>
        private uint m_Pos;

        /// <summary>
        /// The m_ processed size.
        /// </summary>
        private ulong m_ProcessedSize;

        /// <summary>
        /// The m_ stream.
        /// </summary>
        private Stream m_Stream;

        /// <summary>
        /// The m_ stream was exhausted.
        /// </summary>
        private bool m_StreamWasExhausted;

        /// <summary>
        /// Initializes a new instance of the <see cref="InBuffer"/> class.
        /// </summary>
        /// <param name="bufferSize">
        /// The buffer size.
        /// </param>
        public InBuffer(uint bufferSize)
        {
            this.m_Buffer = new byte[bufferSize];
            this.m_BufferSize = bufferSize;
        }

        /// <summary>
        /// The get processed size.
        /// </summary>
        /// <returns>
        /// The <see cref="ulong"/>.
        /// </returns>
        public ulong GetProcessedSize()
        {
            return this.m_ProcessedSize + this.m_Pos;
        }

        /// <summary>
        /// The init.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public void Init(Stream stream)
        {
            this.m_Stream = stream;
            this.m_ProcessedSize = 0;
            this.m_Limit = 0;
            this.m_Pos = 0;
            this.m_StreamWasExhausted = false;
        }

        /// <summary>
        /// The read block.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ReadBlock()
        {
            if (this.m_StreamWasExhausted)
            {
                return false;
            }

            this.m_ProcessedSize += this.m_Pos;
            int aNumProcessedBytes = this.m_Stream.Read(this.m_Buffer, 0, (int)this.m_BufferSize);
            this.m_Pos = 0;
            this.m_Limit = (uint)aNumProcessedBytes;
            this.m_StreamWasExhausted = aNumProcessedBytes == 0;
            return !this.m_StreamWasExhausted;
        }

        /// <summary>
        /// The read byte.
        /// </summary>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ReadByte(byte b)
        {
            // check it
            if (this.m_Pos >= this.m_Limit)
            {
                if (!this.ReadBlock())
                {
                    return false;
                }
            }

            b = this.m_Buffer[this.m_Pos++];
            return true;
        }

        /// <summary>
        /// The read byte.
        /// </summary>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public byte ReadByte()
        {
            // return (byte)m_Stream.ReadByte();
            if (this.m_Pos >= this.m_Limit)
            {
                if (!this.ReadBlock())
                {
                    return 0xFF;
                }
            }

            return this.m_Buffer[this.m_Pos++];
        }

        /// <summary>
        /// The release stream.
        /// </summary>
        public void ReleaseStream()
        {
            // m_Stream.Close(); 
            this.m_Stream = null;
        }
    }
}