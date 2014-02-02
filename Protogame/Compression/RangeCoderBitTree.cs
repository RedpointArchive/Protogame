// This is a copy of the 7-zip LZMA
// compression library.
namespace Protogame.Compression.RangeCoder
{
    using System;

    /// <summary>
    /// The bit tree encoder.
    /// </summary>
    internal struct BitTreeEncoder
    {
        /// <summary>
        /// The models.
        /// </summary>
        private readonly BitEncoder[] Models;

        /// <summary>
        /// The num bit levels.
        /// </summary>
        private readonly int NumBitLevels;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitTreeEncoder"/> struct.
        /// </summary>
        /// <param name="numBitLevels">
        /// The num bit levels.
        /// </param>
        public BitTreeEncoder(int numBitLevels)
        {
            this.NumBitLevels = numBitLevels;
            this.Models = new BitEncoder[1 << numBitLevels];
        }

        /// <summary>
        /// The reverse encode.
        /// </summary>
        /// <param name="Models">
        /// The models.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <param name="rangeEncoder">
        /// The range encoder.
        /// </param>
        /// <param name="NumBitLevels">
        /// The num bit levels.
        /// </param>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public static void ReverseEncode(
            BitEncoder[] Models, 
            uint startIndex, 
            Encoder rangeEncoder, 
            int NumBitLevels, 
            uint symbol)
        {
            uint m = 1;
            for (int i = 0; i < NumBitLevels; i++)
            {
                uint bit = symbol & 1;
                Models[startIndex + m].Encode(rangeEncoder, bit);
                m = (m << 1) | bit;
                symbol >>= 1;
            }
        }

        /// <summary>
        /// The reverse get price.
        /// </summary>
        /// <param name="Models">
        /// The models.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <param name="NumBitLevels">
        /// The num bit levels.
        /// </param>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public static uint ReverseGetPrice(BitEncoder[] Models, uint startIndex, int NumBitLevels, uint symbol)
        {
            uint price = 0;
            uint m = 1;
            for (int i = NumBitLevels; i > 0; i--)
            {
                uint bit = symbol & 1;
                symbol >>= 1;
                price += Models[startIndex + m].GetPrice(bit);
                m = (m << 1) | bit;
            }

            return price;
        }

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="rangeEncoder">
        /// The range encoder.
        /// </param>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public void Encode(Encoder rangeEncoder, uint symbol)
        {
            uint m = 1;
            for (int bitIndex = this.NumBitLevels; bitIndex > 0;)
            {
                bitIndex--;
                uint bit = (symbol >> bitIndex) & 1;
                this.Models[m].Encode(rangeEncoder, bit);
                m = (m << 1) | bit;
            }
        }

        /// <summary>
        /// The get price.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetPrice(uint symbol)
        {
            uint price = 0;
            uint m = 1;
            for (int bitIndex = this.NumBitLevels; bitIndex > 0;)
            {
                bitIndex--;
                uint bit = (symbol >> bitIndex) & 1;
                price += this.Models[m].GetPrice(bit);
                m = (m << 1) + bit;
            }

            return price;
        }

        /// <summary>
        /// The init.
        /// </summary>
        public void Init()
        {
            for (uint i = 1; i < (1 << this.NumBitLevels); i++)
            {
                this.Models[i].Init();
            }
        }

        /// <summary>
        /// The reverse encode.
        /// </summary>
        /// <param name="rangeEncoder">
        /// The range encoder.
        /// </param>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public void ReverseEncode(Encoder rangeEncoder, uint symbol)
        {
            uint m = 1;
            for (uint i = 0; i < this.NumBitLevels; i++)
            {
                uint bit = symbol & 1;
                this.Models[m].Encode(rangeEncoder, bit);
                m = (m << 1) | bit;
                symbol >>= 1;
            }
        }

        /// <summary>
        /// The reverse get price.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint ReverseGetPrice(uint symbol)
        {
            uint price = 0;
            uint m = 1;
            for (int i = this.NumBitLevels; i > 0; i--)
            {
                uint bit = symbol & 1;
                symbol >>= 1;
                price += this.Models[m].GetPrice(bit);
                m = (m << 1) | bit;
            }

            return price;
        }
    }

    /// <summary>
    /// The bit tree decoder.
    /// </summary>
    internal struct BitTreeDecoder
    {
        /// <summary>
        /// The models.
        /// </summary>
        private readonly BitDecoder[] Models;

        /// <summary>
        /// The num bit levels.
        /// </summary>
        private readonly int NumBitLevels;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitTreeDecoder"/> struct.
        /// </summary>
        /// <param name="numBitLevels">
        /// The num bit levels.
        /// </param>
        public BitTreeDecoder(int numBitLevels)
        {
            this.NumBitLevels = numBitLevels;
            this.Models = new BitDecoder[1 << numBitLevels];
        }

        /// <summary>
        /// The reverse decode.
        /// </summary>
        /// <param name="Models">
        /// The models.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <param name="rangeDecoder">
        /// The range decoder.
        /// </param>
        /// <param name="NumBitLevels">
        /// The num bit levels.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public static uint ReverseDecode(BitDecoder[] Models, uint startIndex, Decoder rangeDecoder, int NumBitLevels)
        {
            uint m = 1;
            uint symbol = 0;
            for (int bitIndex = 0; bitIndex < NumBitLevels; bitIndex++)
            {
                uint bit = Models[startIndex + m].Decode(rangeDecoder);
                m <<= 1;
                m += bit;
                symbol |= bit << bitIndex;
            }

            return symbol;
        }

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="rangeDecoder">
        /// The range decoder.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint Decode(Decoder rangeDecoder)
        {
            uint m = 1;
            for (int bitIndex = this.NumBitLevels; bitIndex > 0; bitIndex--)
            {
                m = (m << 1) + this.Models[m].Decode(rangeDecoder);
            }

            return m - ((uint)1 << this.NumBitLevels);
        }

        /// <summary>
        /// The init.
        /// </summary>
        public void Init()
        {
            for (uint i = 1; i < (1 << this.NumBitLevels); i++)
            {
                this.Models[i].Init();
            }
        }

        /// <summary>
        /// The reverse decode.
        /// </summary>
        /// <param name="rangeDecoder">
        /// The range decoder.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint ReverseDecode(Decoder rangeDecoder)
        {
            uint m = 1;
            uint symbol = 0;
            for (int bitIndex = 0; bitIndex < this.NumBitLevels; bitIndex++)
            {
                uint bit = this.Models[m].Decode(rangeDecoder);
                m <<= 1;
                m += bit;
                symbol |= bit << bitIndex;
            }

            return symbol;
        }
    }
}