// This is a copy of the 7-zip LZMA
// compression library.
namespace Protogame.Compression
{
    using System.IO;

    /// <summary>
    /// The out buffer.
    /// </summary>
    public class OutBuffer
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
        /// Initializes a new instance of the <see cref="OutBuffer"/> class.
        /// </summary>
        /// <param name="bufferSize">
        /// The buffer size.
        /// </param>
        public OutBuffer(uint bufferSize)
        {
            this.m_Buffer = new byte[bufferSize];
            this.m_BufferSize = bufferSize;
        }

        /// <summary>
        /// The close stream.
        /// </summary>
        public void CloseStream()
        {
            this.m_Stream.Close();
        }

        /// <summary>
        /// The flush data.
        /// </summary>
        public void FlushData()
        {
            if (this.m_Pos == 0)
            {
                return;
            }

            this.m_Stream.Write(this.m_Buffer, 0, (int)this.m_Pos);
            this.m_Pos = 0;
        }

        /// <summary>
        /// The flush stream.
        /// </summary>
        public void FlushStream()
        {
            this.m_Stream.Flush();
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
        public void Init()
        {
            this.m_ProcessedSize = 0;
            this.m_Pos = 0;
        }

        /// <summary>
        /// The release stream.
        /// </summary>
        public void ReleaseStream()
        {
            this.m_Stream = null;
        }

        /// <summary>
        /// The set stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public void SetStream(Stream stream)
        {
            this.m_Stream = stream;
        }

        /// <summary>
        /// The write byte.
        /// </summary>
        /// <param name="b">
        /// The b.
        /// </param>
        public void WriteByte(byte b)
        {
            this.m_Buffer[this.m_Pos++] = b;
            if (this.m_Pos >= this.m_BufferSize)
            {
                this.FlushData();
            }
        }
    }
}