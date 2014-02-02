// This is a copy of the 7-zip LZMA
// compression library.
namespace Protogame.Compression
{
    using System;
    using System.IO;

    /// <summary>
    /// The bin tree.
    /// </summary>
    public class BinTree : InWindow, IMatchFinder
    {
        /// <summary>
        /// The k b t 2 hash size.
        /// </summary>
        private const uint kBT2HashSize = 1 << 16;

        /// <summary>
        /// The k empty hash value.
        /// </summary>
        private const uint kEmptyHashValue = 0;

        /// <summary>
        /// The k hash 2 size.
        /// </summary>
        private const uint kHash2Size = 1 << 10;

        /// <summary>
        /// The k hash 3 offset.
        /// </summary>
        private const uint kHash3Offset = kHash2Size;

        /// <summary>
        /// The k hash 3 size.
        /// </summary>
        private const uint kHash3Size = 1 << 16;

        /// <summary>
        /// The k max val for normalize.
        /// </summary>
        private const uint kMaxValForNormalize = ((UInt32)1 << 31) - 1;

        /// <summary>
        /// The k start max len.
        /// </summary>
        private const uint kStartMaxLen = 1;

        /// <summary>
        /// The has h_ array.
        /// </summary>
        private bool HASH_ARRAY = true;

        /// <summary>
        /// The _cut value.
        /// </summary>
        private uint _cutValue = 0xFF;

        /// <summary>
        /// The _cyclic buffer pos.
        /// </summary>
        private uint _cyclicBufferPos;

        /// <summary>
        /// The _cyclic buffer size.
        /// </summary>
        private uint _cyclicBufferSize;

        /// <summary>
        /// The _hash.
        /// </summary>
        private uint[] _hash;

        /// <summary>
        /// The _hash mask.
        /// </summary>
        private uint _hashMask;

        /// <summary>
        /// The _hash size sum.
        /// </summary>
        private uint _hashSizeSum;

        /// <summary>
        /// The _match max len.
        /// </summary>
        private uint _matchMaxLen;

        /// <summary>
        /// The _son.
        /// </summary>
        private uint[] _son;

        /// <summary>
        /// The k fix hash size.
        /// </summary>
        private uint kFixHashSize = kHash2Size + kHash3Size;

        /// <summary>
        /// The k min match check.
        /// </summary>
        private uint kMinMatchCheck = 4;

        /// <summary>
        /// The k num hash direct bytes.
        /// </summary>
        private uint kNumHashDirectBytes;

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="historySize">
        /// The history size.
        /// </param>
        /// <param name="keepAddBufferBefore">
        /// The keep add buffer before.
        /// </param>
        /// <param name="matchMaxLen">
        /// The match max len.
        /// </param>
        /// <param name="keepAddBufferAfter">
        /// The keep add buffer after.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        public void Create(
            uint historySize, 
            uint keepAddBufferBefore, 
            uint matchMaxLen, 
            uint keepAddBufferAfter)
        {
            if (historySize > kMaxValForNormalize - 256)
            {
                throw new Exception();
            }

            this._cutValue = 16 + (matchMaxLen >> 1);

            uint windowReservSize = (historySize + keepAddBufferBefore + matchMaxLen + keepAddBufferAfter) / 2 + 256;

            this.Create(historySize + keepAddBufferBefore, matchMaxLen + keepAddBufferAfter, windowReservSize);

            this._matchMaxLen = matchMaxLen;

            uint cyclicBufferSize = historySize + 1;
            if (this._cyclicBufferSize != cyclicBufferSize)
            {
                this._son = new uint[(this._cyclicBufferSize = cyclicBufferSize) * 2];
            }

            uint hs = kBT2HashSize;

            if (this.HASH_ARRAY)
            {
                hs = historySize - 1;
                hs |= hs >> 1;
                hs |= hs >> 2;
                hs |= hs >> 4;
                hs |= hs >> 8;
                hs >>= 1;
                hs |= 0xFFFF;
                if (hs > (1 << 24))
                {
                    hs >>= 1;
                }

                this._hashMask = hs;
                hs++;
                hs += this.kFixHashSize;
            }

            if (hs != this._hashSizeSum)
            {
                this._hash = new uint[this._hashSizeSum = hs];
            }
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
        public new byte GetIndexByte(int index)
        {
            return base.GetIndexByte(index);
        }

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
        public new uint GetMatchLen(int index, uint distance, uint limit)
        {
            return base.GetMatchLen(index, distance, limit);
        }

        /// <summary>
        /// The get matches.
        /// </summary>
        /// <param name="distances">
        /// The distances.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetMatches(uint[] distances)
        {
            uint lenLimit;
            if (this._pos + this._matchMaxLen <= this._streamPos)
            {
                lenLimit = this._matchMaxLen;
            }
            else
            {
                lenLimit = this._streamPos - this._pos;
                if (lenLimit < this.kMinMatchCheck)
                {
                    this.MovePos();
                    return 0;
                }
            }

            uint offset = 0;
            uint matchMinPos = (this._pos > this._cyclicBufferSize) ? (this._pos - this._cyclicBufferSize) : 0;
            uint cur = this._bufferOffset + this._pos;
            uint maxLen = kStartMaxLen; // to avoid items for len < hashSize;
            uint hashValue, hash2Value = 0, hash3Value = 0;

            if (this.HASH_ARRAY)
            {
                uint temp = CRC.Table[this._bufferBase[cur]] ^ this._bufferBase[cur + 1];
                hash2Value = temp & (kHash2Size - 1);
                temp ^= (UInt32)(this._bufferBase[cur + 2]) << 8;
                hash3Value = temp & (kHash3Size - 1);
                hashValue = (temp ^ (CRC.Table[this._bufferBase[cur + 3]] << 5)) & this._hashMask;
            }
            else
            {
                hashValue = this._bufferBase[cur] ^ ((UInt32)this._bufferBase[cur + 1] << 8);
            }

            uint curMatch = this._hash[this.kFixHashSize + hashValue];
            if (this.HASH_ARRAY)
            {
                uint curMatch2 = this._hash[hash2Value];
                uint curMatch3 = this._hash[kHash3Offset + hash3Value];
                this._hash[hash2Value] = this._pos;
                this._hash[kHash3Offset + hash3Value] = this._pos;
                if (curMatch2 > matchMinPos)
                {
                    if (this._bufferBase[this._bufferOffset + curMatch2] == this._bufferBase[cur])
                    {
                        distances[offset++] = maxLen = 2;
                        distances[offset++] = this._pos - curMatch2 - 1;
                    }
                }

                if (curMatch3 > matchMinPos)
                {
                    if (this._bufferBase[this._bufferOffset + curMatch3] == this._bufferBase[cur])
                    {
                        if (curMatch3 == curMatch2)
                        {
                            offset -= 2;
                        }

                        distances[offset++] = maxLen = 3;
                        distances[offset++] = this._pos - curMatch3 - 1;
                        curMatch2 = curMatch3;
                    }
                }

                if (offset != 0 && curMatch2 == curMatch)
                {
                    offset -= 2;
                    maxLen = kStartMaxLen;
                }
            }

            this._hash[this.kFixHashSize + hashValue] = this._pos;

            uint ptr0 = (this._cyclicBufferPos << 1) + 1;
            uint ptr1 = this._cyclicBufferPos << 1;

            uint len0, len1;
            len0 = len1 = this.kNumHashDirectBytes;

            if (this.kNumHashDirectBytes != 0)
            {
                if (curMatch > matchMinPos)
                {
                    if (this._bufferBase[this._bufferOffset + curMatch + this.kNumHashDirectBytes]
                        != this._bufferBase[cur + this.kNumHashDirectBytes])
                    {
                        distances[offset++] = maxLen = this.kNumHashDirectBytes;
                        distances[offset++] = this._pos - curMatch - 1;
                    }
                }
            }

            uint count = this._cutValue;

            while (true)
            {
                if (curMatch <= matchMinPos || count-- == 0)
                {
                    this._son[ptr0] = this._son[ptr1] = kEmptyHashValue;
                    break;
                }

                uint delta = this._pos - curMatch;
                uint cyclicPos = ((delta <= this._cyclicBufferPos)
                                        ? (this._cyclicBufferPos - delta)
                                        : (this._cyclicBufferPos - delta + this._cyclicBufferSize)) << 1;

                uint pby1 = this._bufferOffset + curMatch;
                uint len = Math.Min(len0, len1);
                if (this._bufferBase[pby1 + len] == this._bufferBase[cur + len])
                {
                    while (++len != lenLimit)
                    {
                        if (this._bufferBase[pby1 + len] != this._bufferBase[cur + len])
                        {
                            break;
                        }
                    }

                    if (maxLen < len)
                    {
                        distances[offset++] = maxLen = len;
                        distances[offset++] = delta - 1;
                        if (len == lenLimit)
                        {
                            this._son[ptr1] = this._son[cyclicPos];
                            this._son[ptr0] = this._son[cyclicPos + 1];
                            break;
                        }
                    }
                }

                if (this._bufferBase[pby1 + len] < this._bufferBase[cur + len])
                {
                    this._son[ptr1] = curMatch;
                    ptr1 = cyclicPos + 1;
                    curMatch = this._son[ptr1];
                    len1 = len;
                }
                else
                {
                    this._son[ptr0] = curMatch;
                    ptr0 = cyclicPos;
                    curMatch = this._son[ptr0];
                    len0 = len;
                }
            }

            this.MovePos();
            return offset;
        }

        /// <summary>
        /// The get num available bytes.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public new uint GetNumAvailableBytes()
        {
            return base.GetNumAvailableBytes();
        }

        /// <summary>
        /// The init.
        /// </summary>
        public new void Init()
        {
            base.Init();
            for (uint i = 0; i < this._hashSizeSum; i++)
            {
                this._hash[i] = kEmptyHashValue;
            }

            this._cyclicBufferPos = 0;
            this.ReduceOffsets(-1);
        }

        /// <summary>
        /// The move pos.
        /// </summary>
        public new void MovePos()
        {
            if (++this._cyclicBufferPos >= this._cyclicBufferSize)
            {
                this._cyclicBufferPos = 0;
            }

            base.MovePos();
            if (this._pos == kMaxValForNormalize)
            {
                this.Normalize();
            }
        }

        /// <summary>
        /// The release stream.
        /// </summary>
        public new void ReleaseStream()
        {
            base.ReleaseStream();
        }

        /// <summary>
        /// The set cut value.
        /// </summary>
        /// <param name="cutValue">
        /// The cut value.
        /// </param>
        public void SetCutValue(uint cutValue)
        {
            this._cutValue = cutValue;
        }

        /// <summary>
        /// The set stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public new void SetStream(Stream stream)
        {
            base.SetStream(stream);
        }

        /// <summary>
        /// The set type.
        /// </summary>
        /// <param name="numHashBytes">
        /// The num hash bytes.
        /// </param>
        public void SetType(int numHashBytes)
        {
            this.HASH_ARRAY = numHashBytes > 2;
            if (this.HASH_ARRAY)
            {
                this.kNumHashDirectBytes = 0;
                this.kMinMatchCheck = 4;
                this.kFixHashSize = kHash2Size + kHash3Size;
            }
            else
            {
                this.kNumHashDirectBytes = 2;
                this.kMinMatchCheck = 2 + 1;
                this.kFixHashSize = 0;
            }
        }

        /// <summary>
        /// The skip.
        /// </summary>
        /// <param name="num">
        /// The num.
        /// </param>
        public void Skip(uint num)
        {
            do
            {
                uint lenLimit;
                if (this._pos + this._matchMaxLen <= this._streamPos)
                {
                    lenLimit = this._matchMaxLen;
                }
                else
                {
                    lenLimit = this._streamPos - this._pos;
                    if (lenLimit < this.kMinMatchCheck)
                    {
                        this.MovePos();
                        continue;
                    }
                }

                uint matchMinPos = (this._pos > this._cyclicBufferSize) ? (this._pos - this._cyclicBufferSize) : 0;
                uint cur = this._bufferOffset + this._pos;

                uint hashValue;

                if (this.HASH_ARRAY)
                {
                    uint temp = CRC.Table[this._bufferBase[cur]] ^ this._bufferBase[cur + 1];
                    uint hash2Value = temp & (kHash2Size - 1);
                    this._hash[hash2Value] = this._pos;
                    temp ^= (UInt32)(this._bufferBase[cur + 2]) << 8;
                    uint hash3Value = temp & (kHash3Size - 1);
                    this._hash[kHash3Offset + hash3Value] = this._pos;
                    hashValue = (temp ^ (CRC.Table[this._bufferBase[cur + 3]] << 5)) & this._hashMask;
                }
                else
                {
                    hashValue = this._bufferBase[cur] ^ ((UInt32)this._bufferBase[cur + 1] << 8);
                }

                uint curMatch = this._hash[this.kFixHashSize + hashValue];
                this._hash[this.kFixHashSize + hashValue] = this._pos;

                uint ptr0 = (this._cyclicBufferPos << 1) + 1;
                uint ptr1 = this._cyclicBufferPos << 1;

                uint len0, len1;
                len0 = len1 = this.kNumHashDirectBytes;

                uint count = this._cutValue;
                while (true)
                {
                    if (curMatch <= matchMinPos || count-- == 0)
                    {
                        this._son[ptr0] = this._son[ptr1] = kEmptyHashValue;
                        break;
                    }

                    uint delta = this._pos - curMatch;
                    uint cyclicPos = ((delta <= this._cyclicBufferPos)
                                            ? (this._cyclicBufferPos - delta)
                                            : (this._cyclicBufferPos - delta + this._cyclicBufferSize)) << 1;

                    uint pby1 = this._bufferOffset + curMatch;
                    uint len = Math.Min(len0, len1);
                    if (this._bufferBase[pby1 + len] == this._bufferBase[cur + len])
                    {
                        while (++len != lenLimit)
                        {
                            if (this._bufferBase[pby1 + len] != this._bufferBase[cur + len])
                            {
                                break;
                            }
                        }

                        if (len == lenLimit)
                        {
                            this._son[ptr1] = this._son[cyclicPos];
                            this._son[ptr0] = this._son[cyclicPos + 1];
                            break;
                        }
                    }

                    if (this._bufferBase[pby1 + len] < this._bufferBase[cur + len])
                    {
                        this._son[ptr1] = curMatch;
                        ptr1 = cyclicPos + 1;
                        curMatch = this._son[ptr1];
                        len1 = len;
                    }
                    else
                    {
                        this._son[ptr0] = curMatch;
                        ptr0 = cyclicPos;
                        curMatch = this._son[ptr0];
                        len0 = len;
                    }
                }

                this.MovePos();
            }
            while (--num != 0);
        }

        /// <summary>
        /// The normalize.
        /// </summary>
        private void Normalize()
        {
            uint subValue = this._pos - this._cyclicBufferSize;
            this.NormalizeLinks(this._son, this._cyclicBufferSize * 2, subValue);
            this.NormalizeLinks(this._hash, this._hashSizeSum, subValue);
            this.ReduceOffsets((Int32)subValue);
        }

        /// <summary>
        /// The normalize links.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="numItems">
        /// The num items.
        /// </param>
        /// <param name="subValue">
        /// The sub value.
        /// </param>
        private void NormalizeLinks(uint[] items, uint numItems, uint subValue)
        {
            for (uint i = 0; i < numItems; i++)
            {
                uint value = items[i];
                if (value <= subValue)
                {
                    value = kEmptyHashValue;
                }
                else
                {
                    value -= subValue;
                }

                items[i] = value;
            }
        }
    }
}