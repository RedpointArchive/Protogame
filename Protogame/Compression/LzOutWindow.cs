// This is a copy of the 7-zip LZMA
// compression library.
namespace Protogame.Compression
{
    using System.IO;

    /// <summary>
    /// The out window.
    /// </summary>
    public class OutWindow
    {
        /// <summary>
        /// The train size.
        /// </summary>
        public uint TrainSize = 0;

        /// <summary>
        /// The _buffer.
        /// </summary>
        private byte[] _buffer;

        /// <summary>
        /// The _pos.
        /// </summary>
        private uint _pos;

        /// <summary>
        /// The _stream.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// The _stream pos.
        /// </summary>
        private uint _streamPos;

        /// <summary>
        /// The _window size.
        /// </summary>
        private uint _windowSize;

        /// <summary>
        /// The copy block.
        /// </summary>
        /// <param name="distance">
        /// The distance.
        /// </param>
        /// <param name="len">
        /// The len.
        /// </param>
        public void CopyBlock(uint distance, uint len)
        {
            uint pos = this._pos - distance - 1;
            if (pos >= this._windowSize)
            {
                pos += this._windowSize;
            }

            for (; len > 0; len--)
            {
                if (pos >= this._windowSize)
                {
                    pos = 0;
                }

                this._buffer[this._pos++] = this._buffer[pos++];
                if (this._pos >= this._windowSize)
                {
                    this.Flush();
                }
            }
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="windowSize">
        /// The window size.
        /// </param>
        public void Create(uint windowSize)
        {
            if (this._windowSize != windowSize)
            {
                // System.GC.Collect();
                this._buffer = new byte[windowSize];
            }

            this._windowSize = windowSize;
            this._pos = 0;
            this._streamPos = 0;
        }

        /// <summary>
        /// The flush.
        /// </summary>
        public void Flush()
        {
            uint size = this._pos - this._streamPos;
            if (size == 0)
            {
                return;
            }

            this._stream.Write(this._buffer, (int)this._streamPos, (int)size);
            if (this._pos >= this._windowSize)
            {
                this._pos = 0;
            }

            this._streamPos = this._pos;
        }

        /// <summary>
        /// The get byte.
        /// </summary>
        /// <param name="distance">
        /// The distance.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public byte GetByte(uint distance)
        {
            uint pos = this._pos - distance - 1;
            if (pos >= this._windowSize)
            {
                pos += this._windowSize;
            }

            return this._buffer[pos];
        }

        /// <summary>
        /// The init.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="solid">
        /// The solid.
        /// </param>
        public void Init(Stream stream, bool solid)
        {
            this.ReleaseStream();
            this._stream = stream;
            if (!solid)
            {
                this._streamPos = 0;
                this._pos = 0;
                this.TrainSize = 0;
            }
        }

        /// <summary>
        /// The put byte.
        /// </summary>
        /// <param name="b">
        /// The b.
        /// </param>
        public void PutByte(byte b)
        {
            this._buffer[this._pos++] = b;
            if (this._pos >= this._windowSize)
            {
                this.Flush();
            }
        }

        /// <summary>
        /// The release stream.
        /// </summary>
        public void ReleaseStream()
        {
            this.Flush();
            this._stream = null;
        }

        /// <summary>
        /// The train.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Train(Stream stream)
        {
            long len = stream.Length;
            uint size = (len < this._windowSize) ? (uint)len : this._windowSize;
            this.TrainSize = size;
            stream.Position = len - size;
            this._streamPos = this._pos = 0;
            while (size > 0)
            {
                uint curSize = this._windowSize - this._pos;
                if (size < curSize)
                {
                    curSize = size;
                }

                int numReadBytes = stream.Read(this._buffer, (int)this._pos, (int)curSize);
                if (numReadBytes == 0)
                {
                    return false;
                }

                size -= (uint)numReadBytes;
                this._pos += (uint)numReadBytes;
                this._streamPos += (uint)numReadBytes;
                if (this._pos == this._windowSize)
                {
                    this._streamPos = this._pos = 0;
                }
            }

            return true;
        }
    }
}