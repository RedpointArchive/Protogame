// This is a copy of the 7-zip LZMA
// compression library.
namespace Protogame.Compression.RangeCoder
{
    using System;

    /// <summary>
    /// The bit encoder.
    /// </summary>
    internal struct BitEncoder
    {
        /// <summary>
        /// The k bit model total.
        /// </summary>
        public const uint kBitModelTotal = 1 << kNumBitModelTotalBits;

        /// <summary>
        /// The k num bit model total bits.
        /// </summary>
        public const int kNumBitModelTotalBits = 11;

        /// <summary>
        /// The k num bit price shift bits.
        /// </summary>
        public const int kNumBitPriceShiftBits = 6;

        /// <summary>
        /// The k num move bits.
        /// </summary>
        private const int kNumMoveBits = 5;

        /// <summary>
        /// The k num move reducing bits.
        /// </summary>
        private const int kNumMoveReducingBits = 2;

        /// <summary>
        /// The prob prices.
        /// </summary>
        private static readonly uint[] ProbPrices = new uint[kBitModelTotal >> kNumMoveReducingBits];

        /// <summary>
        /// The prob.
        /// </summary>
        private uint Prob;

        /// <summary>
        /// Initializes static members of the <see cref="BitEncoder"/> struct.
        /// </summary>
        static BitEncoder()
        {
            const int kNumBits = kNumBitModelTotalBits - kNumMoveReducingBits;
            for (int i = kNumBits - 1; i >= 0; i--)
            {
                uint start = (UInt32)1 << (kNumBits - i - 1);
                uint end = (UInt32)1 << (kNumBits - i);
                for (uint j = start; j < end; j++)
                {
                    ProbPrices[j] = ((UInt32)i << kNumBitPriceShiftBits)
                                    + (((end - j) << kNumBitPriceShiftBits) >> (kNumBits - i - 1));
                }
            }
        }

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="encoder">
        /// The encoder.
        /// </param>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public void Encode(Encoder encoder, uint symbol)
        {
            // encoder.EncodeBit(Prob, kNumBitModelTotalBits, symbol);
            // UpdateModel(symbol);
            uint newBound = (encoder.Range >> kNumBitModelTotalBits) * this.Prob;
            if (symbol == 0)
            {
                encoder.Range = newBound;
                this.Prob += (kBitModelTotal - this.Prob) >> kNumMoveBits;
            }
            else
            {
                encoder.Low += newBound;
                encoder.Range -= newBound;
                this.Prob -= this.Prob >> kNumMoveBits;
            }

            if (encoder.Range < Encoder.kTopValue)
            {
                encoder.Range <<= 8;
                encoder.ShiftLow();
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
            return
                ProbPrices[(((this.Prob - symbol) ^ (-(int)symbol)) & (kBitModelTotal - 1)) >> kNumMoveReducingBits];
        }

        /// <summary>
        /// The get price 0.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetPrice0()
        {
            return ProbPrices[this.Prob >> kNumMoveReducingBits];
        }

        /// <summary>
        /// The get price 1.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetPrice1()
        {
            return ProbPrices[(kBitModelTotal - this.Prob) >> kNumMoveReducingBits];
        }

        /// <summary>
        /// The init.
        /// </summary>
        public void Init()
        {
            this.Prob = kBitModelTotal >> 1;
        }

        /// <summary>
        /// The update model.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public void UpdateModel(uint symbol)
        {
            if (symbol == 0)
            {
                this.Prob += (kBitModelTotal - this.Prob) >> kNumMoveBits;
            }
            else
            {
                this.Prob -= this.Prob >> kNumMoveBits;
            }
        }
    }

    /// <summary>
    /// The bit decoder.
    /// </summary>
    internal struct BitDecoder
    {
        /// <summary>
        /// The k bit model total.
        /// </summary>
        public const uint kBitModelTotal = 1 << kNumBitModelTotalBits;

        /// <summary>
        /// The k num bit model total bits.
        /// </summary>
        public const int kNumBitModelTotalBits = 11;

        /// <summary>
        /// The k num move bits.
        /// </summary>
        private const int kNumMoveBits = 5;

        /// <summary>
        /// The prob.
        /// </summary>
        private uint Prob;

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
            uint newBound = (rangeDecoder.Range >> kNumBitModelTotalBits) * this.Prob;
            if (rangeDecoder.Code < newBound)
            {
                rangeDecoder.Range = newBound;
                this.Prob += (kBitModelTotal - this.Prob) >> kNumMoveBits;
                if (rangeDecoder.Range < Decoder.kTopValue)
                {
                    rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte)rangeDecoder.Stream.ReadByte();
                    rangeDecoder.Range <<= 8;
                }

                return 0;
            }

            rangeDecoder.Range -= newBound;
            rangeDecoder.Code -= newBound;
            this.Prob -= this.Prob >> kNumMoveBits;
            if (rangeDecoder.Range < Decoder.kTopValue)
            {
                rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte)rangeDecoder.Stream.ReadByte();
                rangeDecoder.Range <<= 8;
            }

            return 1;
        }

        /// <summary>
        /// The init.
        /// </summary>
        public void Init()
        {
            this.Prob = kBitModelTotal >> 1;
        }

        /// <summary>
        /// The update model.
        /// </summary>
        /// <param name="numMoveBits">
        /// The num move bits.
        /// </param>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public void UpdateModel(int numMoveBits, uint symbol)
        {
            if (symbol == 0)
            {
                this.Prob += (kBitModelTotal - this.Prob) >> numMoveBits;
            }
            else
            {
                this.Prob -= this.Prob >> numMoveBits;
            }
        }
    }
}