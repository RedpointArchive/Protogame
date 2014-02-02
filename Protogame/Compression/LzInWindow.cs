// This is a copy of the 7-zip LZMA
// compression library.
namespace Protogame.Compression
{
    using System;
    using System.IO;

    /// <summary>
    /// The in window.
    /// </summary>
    public class InWindow
    {
        /// <summary>
        /// The _block size.
        /// </summary>
        public uint _blockSize; // Size of Allocated memory block

        /// <summary>
        /// The _buffer base.
        /// </summary>
        public byte[] _bufferBase = null; // pointer to buffer with data

        /// <summary>
        /// The _buffer offset.
        /// </summary>
        public uint _bufferOffset;

        /// <summary>
        /// The _pos.
        /// </summary>
        public uint _pos; // offset (from _buffer) of curent byte

        /// <summary>
        /// The _stream pos.
        /// </summary>
        public uint _streamPos; // offset (from _buffer) of first not read byte from Stream

        /// <summary>
        /// The _keep size after.
        /// </summary>
        private uint _keepSizeAfter; // how many BYTEs must be kept buffer after _pos

        /// <summary>
        /// The _keep size before.
        /// </summary>
        private uint _keepSizeBefore; // how many BYTEs must be kept in buffer before _pos

        /// <summary>
        /// The _pointer to last safe position.
        /// </summary>
        private uint _pointerToLastSafePosition;

        /// <summary>
        /// The _pos limit.
        /// </summary>
        private uint _posLimit; // offset (from _buffer) of first byte when new block reading must be done

        /// <summary>
        /// The _stream.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// The _stream end was reached.
        /// </summary>
        private bool _streamEndWasReached; // if (true) then _streamPos shows real end of stream

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="keepSizeBefore">
        /// The keep size before.
        /// </param>
        /// <param name="keepSizeAfter">
        /// The keep size after.
        /// </param>
        /// <param name="keepSizeReserv">
        /// The keep size reserv.
        /// </param>
        public void Create(uint keepSizeBefore, uint keepSizeAfter, uint keepSizeReserv)
        {
            this._keepSizeBefore = keepSizeBefore;
            this._keepSizeAfter = keepSizeAfter;
            uint blockSize = keepSizeBefore + keepSizeAfter + keepSizeReserv;
            if (this._bufferBase == null || this._blockSize != blockSize)
            {
                this.Free();
                this._blockSize = blockSize;
                this._bufferBase = new byte[this._blockSize];
            }

            this._pointerToLastSafePosition = this._blockSize - keepSizeAfter;
        }

        /// <summary>
        /// The get index byte.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public byte GetIndexByte(int index)
        {
            return this._bufferBase[this._bufferOffset + this._pos + index];
        }

        // index + limit have not to exceed _keepSizeAfter;
        /// <summary>
        /// The get match len.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="distance">
        /// The distance.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetMatchLen(int index, uint distance, uint limit)
        {
            if (this._streamEndWasReached)
            {
                if ((this._pos + index) + limit > this._streamPos)
                {
                    limit = this._streamPos - (UInt32)(this._pos + index);
                }
            }

            distance++;

            // Byte *pby = _buffer + (size_t)_pos + index;
            uint pby = this._bufferOffset + this._pos + (UInt32)index;

            uint i;
            for (i = 0; i < limit && this._bufferBase[pby + i] == this._bufferBase[pby + i - distance]; i++)
            {
                ;
            }

            return i;
        }

        /// <summary>
        /// The get num available bytes.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetNumAvailableBytes()
        {
            return this._streamPos - this._pos;
        }

        /// <summary>
        /// The init.
        /// </summary>
        public void Init()
        {
            this._bufferOffset = 0;
            this._pos = 0;
            this._streamPos = 0;
            this._streamEndWasReached = false;
            this.ReadBlock();
        }

        /// <summary>
        /// The move block.
        /// </summary>
        public void MoveBlock()
        {
            uint offset = this._bufferOffset + this._pos - this._keepSizeBefore;

            // we need one additional byte, since MovePos moves on 1 byte.
            if (offset > 0)
            {
                offset--;
            }

            uint numBytes = this._bufferOffset + this._streamPos - offset;

            // check negative offset ????
            for (uint i = 0; i < numBytes; i++)
            {
                this._bufferBase[i] = this._bufferBase[offset + i];
            }

            this._bufferOffset -= offset;
        }

        /// <summary>
        /// The move pos.
        /// </summary>
        public void MovePos()
        {
            this._pos++;
            if (this._pos > this._posLimit)
            {
                uint pointerToPostion = this._bufferOffset + this._pos;
                if (pointerToPostion > this._pointerToLastSafePosition)
                {
                    this.MoveBlock();
                }

                this.ReadBlock();
            }
        }

        /// <summary>
        /// The read block.
        /// </summary>
        public virtual void ReadBlock()
        {
            if (this._streamEndWasReached)
            {
                return;
            }

            while (true)
            {
                var size = (int)((0 - this._bufferOffset) + this._blockSize - this._streamPos);
                if (size == 0)
                {
                    return;
                }

                int numReadBytes = this._stream.Read(
                    this._bufferBase, 
                    (int)(this._bufferOffset + this._streamPos), 
                    size);
                if (numReadBytes == 0)
                {
                    this._posLimit = this._streamPos;
                    uint pointerToPostion = this._bufferOffset + this._posLimit;
                    if (pointerToPostion > this._pointerToLastSafePosition)
                    {
                        this._posLimit = this._pointerToLastSafePosition - this._bufferOffset;
                    }

                    this._streamEndWasReached = true;
                    return;
                }

                this._streamPos += (UInt32)numReadBytes;
                if (this._streamPos >= this._pos + this._keepSizeAfter)
                {
                    this._posLimit = this._streamPos - this._keepSizeAfter;
                }
            }
        }

        /// <summary>
        /// The reduce offsets.
        /// </summary>
        /// <param name="subValue">
        /// The sub value.
        /// </param>
        public void ReduceOffsets(int subValue)
        {
            this._bufferOffset += (UInt32)subValue;
            this._posLimit -= (UInt32)subValue;
            this._pos -= (UInt32)subValue;
            this._streamPos -= (UInt32)subValue;
        }

        /// <summary>
        /// The release stream.
        /// </summary>
        public void ReleaseStream()
        {
            this._stream = null;
        }

        /// <summary>
        /// The set stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public void SetStream(Stream stream)
        {
            this._stream = stream;
        }

        /// <summary>
        /// The free.
        /// </summary>
        private void Free()
        {
            this._bufferBase = null;
        }
    }
}