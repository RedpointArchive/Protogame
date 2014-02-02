// This is a copy of the 7-zip LZMA
// compression library.
namespace Protogame.Compression.RangeCoder
{
    using System;
    using System.IO;

    /// <summary>
    /// The encoder.
    /// </summary>
    internal class Encoder
    {
        /// <summary>
        /// The k top value.
        /// </summary>
        public const uint kTopValue = 1 << 24;

        /// <summary>
        /// The low.
        /// </summary>
        public ulong Low;

        /// <summary>
        /// The range.
        /// </summary>
        public uint Range;

        /// <summary>
        /// The start position.
        /// </summary>
        private long StartPosition;

        /// <summary>
        /// The stream.
        /// </summary>
        private Stream Stream;

        /// <summary>
        /// The _cache.
        /// </summary>
        private byte _cache;

        /// <summary>
        /// The _cache size.
        /// </summary>
        private uint _cacheSize;

        /// <summary>
        /// The close stream.
        /// </summary>
        public void CloseStream()
        {
            this.Stream.Close();
        }

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <param name="total">
        /// The total.
        /// </param>
        public void Encode(uint start, uint size, uint total)
        {
            this.Low += start * (this.Range /= total);
            this.Range *= size;
            while (this.Range < kTopValue)
            {
                this.Range <<= 8;
                this.ShiftLow();
            }
        }

        /// <summary>
        /// The encode bit.
        /// </summary>
        /// <param name="size0">
        /// The size 0.
        /// </param>
        /// <param name="numTotalBits">
        /// The num total bits.
        /// </param>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public void EncodeBit(uint size0, int numTotalBits, uint symbol)
        {
            uint newBound = (this.Range >> numTotalBits) * size0;
            if (symbol == 0)
            {
                this.Range = newBound;
            }
            else
            {
                this.Low += newBound;
                this.Range -= newBound;
            }

            while (this.Range < kTopValue)
            {
                this.Range <<= 8;
                this.ShiftLow();
            }
        }

        /// <summary>
        /// The encode direct bits.
        /// </summary>
        /// <param name="v">
        /// The v.
        /// </param>
        /// <param name="numTotalBits">
        /// The num total bits.
        /// </param>
        public void EncodeDirectBits(uint v, int numTotalBits)
        {
            for (int i = numTotalBits - 1; i >= 0; i--)
            {
                this.Range >>= 1;
                if (((v >> i) & 1) == 1)
                {
                    this.Low += this.Range;
                }

                if (this.Range < kTopValue)
                {
                    this.Range <<= 8;
                    this.ShiftLow();
                }
            }
        }

        /// <summary>
        /// The flush data.
        /// </summary>
        public void FlushData()
        {
            for (int i = 0; i < 5; i++)
            {
                this.ShiftLow();
            }
        }

        /// <summary>
        /// The flush stream.
        /// </summary>
        public void FlushStream()
        {
            this.Stream.Flush();
        }

        /// <summary>
        /// The get processed size add.
        /// </summary>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public long GetProcessedSizeAdd()
        {
            return this._cacheSize + this.Stream.Position - this.StartPosition + 4;

            // (long)Stream.GetProcessedSize();
        }

        /// <summary>
        /// The init.
        /// </summary>
        public void Init()
        {
            this.StartPosition = this.Stream.Position;

            this.Low = 0;
            this.Range = 0xFFFFFFFF;
            this._cacheSize = 1;
            this._cache = 0;
        }

        /// <summary>
        /// The release stream.
        /// </summary>
        public void ReleaseStream()
        {
            this.Stream = null;
        }

        /// <summary>
        /// The set stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public void SetStream(Stream stream)
        {
            this.Stream = stream;
        }

        /// <summary>
        /// The shift low.
        /// </summary>
        public void ShiftLow()
        {
            if ((uint)this.Low < 0xFF000000 || (uint)(this.Low >> 32) == 1)
            {
                byte temp = this._cache;
                do
                {
                    this.Stream.WriteByte((byte)(temp + (this.Low >> 32)));
                    temp = 0xFF;
                }
                while (--this._cacheSize != 0);
                this._cache = (byte)(((uint)this.Low) >> 24);
            }

            this._cacheSize++;
            this.Low = ((uint)this.Low) << 8;
        }
    }

    /// <summary>
    /// The decoder.
    /// </summary>
    internal class Decoder
    {
        /// <summary>
        /// The k top value.
        /// </summary>
        public const uint kTopValue = 1 << 24;

        /// <summary>
        /// The code.
        /// </summary>
        public uint Code;

        /// <summary>
        /// The range.
        /// </summary>
        public uint Range;

        // public Buffer.InBuffer Stream = new Buffer.InBuffer(1 << 16);
        /// <summary>
        /// The stream.
        /// </summary>
        public Stream Stream;

        /// <summary>
        /// The close stream.
        /// </summary>
        public void CloseStream()
        {
            this.Stream.Close();
        }

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <param name="total">
        /// The total.
        /// </param>
        public void Decode(uint start, uint size, uint total)
        {
            this.Code -= start * this.Range;
            this.Range *= size;
            this.Normalize();
        }

        /// <summary>
        /// The decode bit.
        /// </summary>
        /// <param name="size0">
        /// The size 0.
        /// </param>
        /// <param name="numTotalBits">
        /// The num total bits.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint DecodeBit(uint size0, int numTotalBits)
        {
            uint newBound = (this.Range >> numTotalBits) * size0;
            uint symbol;
            if (this.Code < newBound)
            {
                symbol = 0;
                this.Range = newBound;
            }
            else
            {
                symbol = 1;
                this.Code -= newBound;
                this.Range -= newBound;
            }

            this.Normalize();
            return symbol;
        }

        /// <summary>
        /// The decode direct bits.
        /// </summary>
        /// <param name="numTotalBits">
        /// The num total bits.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint DecodeDirectBits(int numTotalBits)
        {
            uint range = this.Range;
            uint code = this.Code;
            uint result = 0;
            for (int i = numTotalBits; i > 0; i--)
            {
                range >>= 1;

                /*
				result <<= 1;
				if (code >= range)
				{
					code -= range;
					result |= 1;
				}
				*/
                uint t = (code - range) >> 31;
                code -= range & (t - 1);
                result = (result << 1) | (1 - t);

                if (range < kTopValue)
                {
                    code = (code << 8) | (byte)this.Stream.ReadByte();
                    range <<= 8;
                }
            }

            this.Range = range;
            this.Code = code;
            return result;
        }

        /// <summary>
        /// The get threshold.
        /// </summary>
        /// <param name="total">
        /// The total.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetThreshold(uint total)
        {
            return this.Code / (this.Range /= total);
        }

        /// <summary>
        /// The init.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public void Init(Stream stream)
        {
            // Stream.Init(stream);
            this.Stream = stream;

            this.Code = 0;
            this.Range = 0xFFFFFFFF;
            for (int i = 0; i < 5; i++)
            {
                this.Code = (this.Code << 8) | (byte)this.Stream.ReadByte();
            }
        }

        /// <summary>
        /// The normalize.
        /// </summary>
        public void Normalize()
        {
            while (this.Range < kTopValue)
            {
                this.Code = (this.Code << 8) | (byte)this.Stream.ReadByte();
                this.Range <<= 8;
            }
        }

        /// <summary>
        /// The normalize 2.
        /// </summary>
        public void Normalize2()
        {
            if (this.Range < kTopValue)
            {
                this.Code = (this.Code << 8) | (byte)this.Stream.ReadByte();
                this.Range <<= 8;
            }
        }

        /// <summary>
        /// The release stream.
        /// </summary>
        public void ReleaseStream()
        {
            // Stream.ReleaseStream();
            this.Stream = null;
        }

        // ulong GetProcessedSize() {return Stream.GetProcessedSize(); }
    }
}