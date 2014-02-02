// This is a copy of the 7-zip LZMA
// compression library.
namespace Protogame.Compression
{
    /// <summary>
    /// The base.
    /// </summary>
    internal abstract class Base
    {
        /// <summary>
        /// The k align mask.
        /// </summary>
        public const uint kAlignMask = kAlignTableSize - 1;

        /// <summary>
        /// The k align table size.
        /// </summary>
        public const uint kAlignTableSize = 1 << kNumAlignBits;

        /// <summary>
        /// The k dic log size min.
        /// </summary>
        public const int kDicLogSizeMin = 0;

        /// <summary>
        /// The k end pos model index.
        /// </summary>
        public const uint kEndPosModelIndex = 14;

        /// <summary>
        /// The k match max len.
        /// </summary>
        public const uint kMatchMaxLen = kMatchMinLen + kNumLenSymbols - 1;

        // public const int kDicLogSizeMax = 30;
        // public const uint kDistTableSizeMax = kDicLogSizeMax * 2;

        /// <summary>
        /// The k match min len.
        /// </summary>
        public const uint kMatchMinLen = 2;

        /// <summary>
        /// The k num align bits.
        /// </summary>
        public const int kNumAlignBits = 4;

        /// <summary>
        /// The k num full distances.
        /// </summary>
        public const uint kNumFullDistances = 1 << ((int)kEndPosModelIndex / 2);

        /// <summary>
        /// The k num high len bits.
        /// </summary>
        public const int kNumHighLenBits = 8;

        /// <summary>
        /// The k num len symbols.
        /// </summary>
        public const uint kNumLenSymbols = kNumLowLenSymbols + kNumMidLenSymbols + (1 << kNumHighLenBits);

        /// <summary>
        /// The k num len to pos states.
        /// </summary>
        public const uint kNumLenToPosStates = 1 << kNumLenToPosStatesBits;

        /// <summary>
        /// The k num len to pos states bits.
        /// </summary>
        public const int kNumLenToPosStatesBits = 2; // it's for speed optimization

        /// <summary>
        /// The k num lit context bits max.
        /// </summary>
        public const uint kNumLitContextBitsMax = 8;

        /// <summary>
        /// The k num lit pos states bits encoding max.
        /// </summary>
        public const uint kNumLitPosStatesBitsEncodingMax = 4;

        /// <summary>
        /// The k num low len bits.
        /// </summary>
        public const int kNumLowLenBits = 3;

        /// <summary>
        /// The k num low len symbols.
        /// </summary>
        public const uint kNumLowLenSymbols = 1 << kNumLowLenBits;

        /// <summary>
        /// The k num mid len bits.
        /// </summary>
        public const int kNumMidLenBits = 3;

        /// <summary>
        /// The k num mid len symbols.
        /// </summary>
        public const uint kNumMidLenSymbols = 1 << kNumMidLenBits;

        /// <summary>
        /// The k num pos models.
        /// </summary>
        public const uint kNumPosModels = kEndPosModelIndex - kStartPosModelIndex;

        /// <summary>
        /// The k num pos slot bits.
        /// </summary>
        public const int kNumPosSlotBits = 6;

        /// <summary>
        /// The k num pos states bits encoding max.
        /// </summary>
        public const int kNumPosStatesBitsEncodingMax = 4;

        /// <summary>
        /// The k num pos states bits max.
        /// </summary>
        public const int kNumPosStatesBitsMax = 4;

        /// <summary>
        /// The k num pos states encoding max.
        /// </summary>
        public const uint kNumPosStatesEncodingMax = 1 << kNumPosStatesBitsEncodingMax;

        /// <summary>
        /// The k num pos states max.
        /// </summary>
        public const uint kNumPosStatesMax = 1 << kNumPosStatesBitsMax;

        /// <summary>
        /// The k num rep distances.
        /// </summary>
        public const uint kNumRepDistances = 4;

        /// <summary>
        /// The k num states.
        /// </summary>
        public const uint kNumStates = 12;

        /// <summary>
        /// The k start pos model index.
        /// </summary>
        public const uint kStartPosModelIndex = 4;

        /// <summary>
        /// The get len to pos state.
        /// </summary>
        /// <param name="len">
        /// The len.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public static uint GetLenToPosState(uint len)
        {
            len -= kMatchMinLen;
            if (len < kNumLenToPosStates)
            {
                return len;
            }

            return kNumLenToPosStates - 1;
        }

        /// <summary>
        /// The state.
        /// </summary>
        public struct State
        {
            /// <summary>
            /// The index.
            /// </summary>
            public uint Index;

            /// <summary>
            /// The init.
            /// </summary>
            public void Init()
            {
                this.Index = 0;
            }

            /// <summary>
            /// The is char state.
            /// </summary>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            public bool IsCharState()
            {
                return this.Index < 7;
            }

            /// <summary>
            /// The update char.
            /// </summary>
            public void UpdateChar()
            {
                if (this.Index < 4)
                {
                    this.Index = 0;
                }
                else if (this.Index < 10)
                {
                    this.Index -= 3;
                }
                else
                {
                    this.Index -= 6;
                }
            }

            /// <summary>
            /// The update match.
            /// </summary>
            public void UpdateMatch()
            {
                this.Index = (uint)(this.Index < 7 ? 7 : 10);
            }

            /// <summary>
            /// The update rep.
            /// </summary>
            public void UpdateRep()
            {
                this.Index = (uint)(this.Index < 7 ? 8 : 11);
            }

            /// <summary>
            /// The update short rep.
            /// </summary>
            public void UpdateShortRep()
            {
                this.Index = (uint)(this.Index < 7 ? 9 : 11);
            }
        }
    }
}