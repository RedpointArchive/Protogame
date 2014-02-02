// This is a copy of the 7-zip LZMA
// compression library.
namespace Protogame.Compression
{
    using System;
    using System.IO;
    using Protogame.Compression.RangeCoder;

    /// <summary>
    /// The lzma encoder.
    /// </summary>
    public class LzmaEncoder : ICoder, ISetCoderProperties, IWriteCoderProperties
    {
        /// <summary>
        /// The k default dictionary log size.
        /// </summary>
        private const int kDefaultDictionaryLogSize = 22;

        /// <summary>
        /// The k ifinity price.
        /// </summary>
        private const uint kIfinityPrice = 0xFFFFFFF;

        /// <summary>
        /// The k num fast bytes default.
        /// </summary>
        private const uint kNumFastBytesDefault = 0x20;

        /// <summary>
        /// The k num len spec symbols.
        /// </summary>
        private const uint kNumLenSpecSymbols = Base.kNumLowLenSymbols + Base.kNumMidLenSymbols;

        /// <summary>
        /// The k num opts.
        /// </summary>
        private const uint kNumOpts = 1 << 12;

        /// <summary>
        /// The k prop size.
        /// </summary>
        private const int kPropSize = 5;

        /// <summary>
        /// The g_ fast pos.
        /// </summary>
        private static readonly byte[] g_FastPos = new byte[1 << 11];

        /// <summary>
        /// The k match finder i ds.
        /// </summary>
        private static readonly string[] kMatchFinderIDs = { "BT2", "BT4" };

        /// <summary>
        /// The _align prices.
        /// </summary>
        private readonly uint[] _alignPrices = new uint[Base.kAlignTableSize];

        /// <summary>
        /// The _distances prices.
        /// </summary>
        private readonly uint[] _distancesPrices = new uint[Base.kNumFullDistances << Base.kNumLenToPosStatesBits];

        /// <summary>
        /// The _is match.
        /// </summary>
        private readonly BitEncoder[] _isMatch = new BitEncoder[Base.kNumStates << Base.kNumPosStatesBitsMax];

        /// <summary>
        /// The _is rep.
        /// </summary>
        private readonly BitEncoder[] _isRep = new BitEncoder[Base.kNumStates];

        /// <summary>
        /// The _is rep 0 long.
        /// </summary>
        private readonly BitEncoder[] _isRep0Long = new BitEncoder[Base.kNumStates << Base.kNumPosStatesBitsMax];

        /// <summary>
        /// The _is rep g 0.
        /// </summary>
        private readonly BitEncoder[] _isRepG0 = new BitEncoder[Base.kNumStates];

        /// <summary>
        /// The _is rep g 1.
        /// </summary>
        private readonly BitEncoder[] _isRepG1 = new BitEncoder[Base.kNumStates];

        /// <summary>
        /// The _is rep g 2.
        /// </summary>
        private readonly BitEncoder[] _isRepG2 = new BitEncoder[Base.kNumStates];

        /// <summary>
        /// The _len encoder.
        /// </summary>
        private readonly LenPriceTableEncoder _lenEncoder = new LenPriceTableEncoder();

        /// <summary>
        /// The _literal encoder.
        /// </summary>
        private readonly LiteralEncoder _literalEncoder = new LiteralEncoder();

        /// <summary>
        /// The _match distances.
        /// </summary>
        private readonly uint[] _matchDistances = new uint[Base.kMatchMaxLen * 2 + 2];

        /// <summary>
        /// The _optimum.
        /// </summary>
        private readonly Optimal[] _optimum = new Optimal[kNumOpts];

        /// <summary>
        /// The _pos encoders.
        /// </summary>
        private readonly BitEncoder[] _posEncoders = new BitEncoder[Base.kNumFullDistances - Base.kEndPosModelIndex];

        /// <summary>
        /// The _pos slot encoder.
        /// </summary>
        private readonly BitTreeEncoder[] _posSlotEncoder = new BitTreeEncoder[Base.kNumLenToPosStates];

        /// <summary>
        /// The _pos slot prices.
        /// </summary>
        private readonly uint[] _posSlotPrices = new uint[1 << (Base.kNumPosSlotBits + Base.kNumLenToPosStatesBits)];

        /// <summary>
        /// The _range encoder.
        /// </summary>
        private readonly Encoder _rangeEncoder = new Encoder();

        /// <summary>
        /// The _rep distances.
        /// </summary>
        private readonly uint[] _repDistances = new uint[Base.kNumRepDistances];

        /// <summary>
        /// The _rep match len encoder.
        /// </summary>
        private readonly LenPriceTableEncoder _repMatchLenEncoder = new LenPriceTableEncoder();

        /// <summary>
        /// The properties.
        /// </summary>
        private readonly byte[] properties = new byte[kPropSize];

        /// <summary>
        /// The rep lens.
        /// </summary>
        private readonly uint[] repLens = new uint[Base.kNumRepDistances];

        /// <summary>
        /// The reps.
        /// </summary>
        private readonly uint[] reps = new uint[Base.kNumRepDistances];

        /// <summary>
        /// The temp prices.
        /// </summary>
        private readonly uint[] tempPrices = new uint[Base.kNumFullDistances];

        /// <summary>
        /// The _additional offset.
        /// </summary>
        private uint _additionalOffset;

        /// <summary>
        /// The _align price count.
        /// </summary>
        private uint _alignPriceCount;

        /// <summary>
        /// The _dictionary size.
        /// </summary>
        private uint _dictionarySize = 1 << kDefaultDictionaryLogSize;

        /// <summary>
        /// The _dictionary size prev.
        /// </summary>
        private uint _dictionarySizePrev = 0xFFFFFFFF;

        /// <summary>
        /// The _dist table size.
        /// </summary>
        private uint _distTableSize = kDefaultDictionaryLogSize * 2;

        /// <summary>
        /// The _finished.
        /// </summary>
        private bool _finished;

        /// <summary>
        /// The _in stream.
        /// </summary>
        private Stream _inStream;

        /// <summary>
        /// The _longest match length.
        /// </summary>
        private uint _longestMatchLength;

        /// <summary>
        /// The _longest match was found.
        /// </summary>
        private bool _longestMatchWasFound;

        /// <summary>
        /// The _match finder.
        /// </summary>
        private IMatchFinder _matchFinder;

        /// <summary>
        /// The _match finder type.
        /// </summary>
        private EMatchFinderType _matchFinderType = EMatchFinderType.BT4;

        /// <summary>
        /// The _match price count.
        /// </summary>
        private uint _matchPriceCount;

        /// <summary>
        /// The _need release mf stream.
        /// </summary>
        private bool _needReleaseMFStream;

        /// <summary>
        /// The _num distance pairs.
        /// </summary>
        private uint _numDistancePairs;

        /// <summary>
        /// The _num fast bytes.
        /// </summary>
        private uint _numFastBytes = kNumFastBytesDefault;

        /// <summary>
        /// The _num fast bytes prev.
        /// </summary>
        private uint _numFastBytesPrev = 0xFFFFFFFF;

        /// <summary>
        /// The _num literal context bits.
        /// </summary>
        private int _numLiteralContextBits = 3;

        /// <summary>
        /// The _num literal pos state bits.
        /// </summary>
        private int _numLiteralPosStateBits;

        /// <summary>
        /// The _optimum current index.
        /// </summary>
        private uint _optimumCurrentIndex;

        /// <summary>
        /// The _optimum end index.
        /// </summary>
        private uint _optimumEndIndex;

        /// <summary>
        /// The _pos align encoder.
        /// </summary>
        private BitTreeEncoder _posAlignEncoder = new BitTreeEncoder(Base.kNumAlignBits);

        /// <summary>
        /// The _pos state bits.
        /// </summary>
        private int _posStateBits = 2;

        /// <summary>
        /// The _pos state mask.
        /// </summary>
        private uint _posStateMask = 4 - 1;

        /// <summary>
        /// The _previous byte.
        /// </summary>
        private byte _previousByte;

        /// <summary>
        /// The _state.
        /// </summary>
        private Base.State _state = new Base.State();

        /// <summary>
        /// The _train size.
        /// </summary>
        private uint _trainSize;

        /// <summary>
        /// The _write end mark.
        /// </summary>
        private bool _writeEndMark;

        /// <summary>
        /// The now pos 64.
        /// </summary>
        private long nowPos64;

        /// <summary>
        /// Initializes static members of the <see cref="LzmaEncoder"/> class.
        /// </summary>
        static LzmaEncoder()
        {
            const byte kFastSlots = 22;
            int c = 2;
            g_FastPos[0] = 0;
            g_FastPos[1] = 1;
            for (byte slotFast = 2; slotFast < kFastSlots; slotFast++)
            {
                uint k = (UInt32)1 << ((slotFast >> 1) - 1);
                for (uint j = 0; j < k; j++, c++)
                {
                    g_FastPos[c] = slotFast;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LzmaEncoder"/> class.
        /// </summary>
        public LzmaEncoder()
        {
            for (int i = 0; i < kNumOpts; i++)
            {
                this._optimum[i] = new Optimal();
            }

            for (int i = 0; i < Base.kNumLenToPosStates; i++)
            {
                this._posSlotEncoder[i] = new BitTreeEncoder(Base.kNumPosSlotBits);
            }
        }

        /// <summary>
        /// The e match finder type.
        /// </summary>
        private enum EMatchFinderType
        {
            /// <summary>
            /// The b t 2.
            /// </summary>
            BT2, 

            /// <summary>
            /// The b t 4.
            /// </summary>
            BT4, 
        };

        /// <summary>
        /// The code.
        /// </summary>
        /// <param name="inStream">
        /// The in stream.
        /// </param>
        /// <param name="outStream">
        /// The out stream.
        /// </param>
        /// <param name="inSize">
        /// The in size.
        /// </param>
        /// <param name="outSize">
        /// The out size.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress)
        {
            this._needReleaseMFStream = false;
            try
            {
                this.SetStreams(inStream, outStream, inSize, outSize);
                while (true)
                {
                    long processedInSize;
                    long processedOutSize;
                    bool finished;
                    this.CodeOneBlock(out processedInSize, out processedOutSize, out finished);
                    if (finished)
                    {
                        return;
                    }

                    if (progress != null)
                    {
                        progress.SetProgress(processedInSize, processedOutSize);
                    }
                }
            }
            finally
            {
                this.ReleaseStreams();
            }
        }

        /// <summary>
        /// The code one block.
        /// </summary>
        /// <param name="inSize">
        /// The in size.
        /// </param>
        /// <param name="outSize">
        /// The out size.
        /// </param>
        /// <param name="finished">
        /// The finished.
        /// </param>
        public void CodeOneBlock(out long inSize, out long outSize, out bool finished)
        {
            inSize = 0;
            outSize = 0;
            finished = true;

            if (this._inStream != null)
            {
                this._matchFinder.SetStream(this._inStream);
                this._matchFinder.Init();
                this._needReleaseMFStream = true;
                this._inStream = null;
                if (this._trainSize > 0)
                {
                    this._matchFinder.Skip(this._trainSize);
                }
            }

            if (this._finished)
            {
                return;
            }

            this._finished = true;

            long progressPosValuePrev = this.nowPos64;
            if (this.nowPos64 == 0)
            {
                if (this._matchFinder.GetNumAvailableBytes() == 0)
                {
                    this.Flush((UInt32)this.nowPos64);
                    return;
                }

                uint len, numDistancePairs; // it's not used
                this.ReadMatchDistances(out len, out numDistancePairs);
                uint posState = (UInt32)this.nowPos64 & this._posStateMask;
                this._isMatch[(this._state.Index << Base.kNumPosStatesBitsMax) + posState].Encode(this._rangeEncoder, 0);
                this._state.UpdateChar();
                byte curByte = this._matchFinder.GetIndexByte((Int32)(0 - this._additionalOffset));
                this._literalEncoder.GetSubCoder((UInt32)this.nowPos64, this._previousByte)
                    .Encode(this._rangeEncoder, curByte);
                this._previousByte = curByte;
                this._additionalOffset--;
                this.nowPos64++;
            }

            if (this._matchFinder.GetNumAvailableBytes() == 0)
            {
                this.Flush((UInt32)this.nowPos64);
                return;
            }

            while (true)
            {
                uint pos;
                uint len = this.GetOptimum((UInt32)this.nowPos64, out pos);

                uint posState = ((UInt32)this.nowPos64) & this._posStateMask;
                uint complexState = (this._state.Index << Base.kNumPosStatesBitsMax) + posState;
                if (len == 1 && pos == 0xFFFFFFFF)
                {
                    this._isMatch[complexState].Encode(this._rangeEncoder, 0);
                    byte curByte = this._matchFinder.GetIndexByte((Int32)(0 - this._additionalOffset));
                    LiteralEncoder.Encoder2 subCoder = this._literalEncoder.GetSubCoder(
                        (UInt32)this.nowPos64, 
                        this._previousByte);
                    if (!this._state.IsCharState())
                    {
                        byte matchByte =
                            this._matchFinder.GetIndexByte(
                                (Int32)(0 - this._repDistances[0] - 1 - this._additionalOffset));
                        subCoder.EncodeMatched(this._rangeEncoder, matchByte, curByte);
                    }
                    else
                    {
                        subCoder.Encode(this._rangeEncoder, curByte);
                    }

                    this._previousByte = curByte;
                    this._state.UpdateChar();
                }
                else
                {
                    this._isMatch[complexState].Encode(this._rangeEncoder, 1);
                    if (pos < Base.kNumRepDistances)
                    {
                        this._isRep[this._state.Index].Encode(this._rangeEncoder, 1);
                        if (pos == 0)
                        {
                            this._isRepG0[this._state.Index].Encode(this._rangeEncoder, 0);
                            if (len == 1)
                            {
                                this._isRep0Long[complexState].Encode(this._rangeEncoder, 0);
                            }
                            else
                            {
                                this._isRep0Long[complexState].Encode(this._rangeEncoder, 1);
                            }
                        }
                        else
                        {
                            this._isRepG0[this._state.Index].Encode(this._rangeEncoder, 1);
                            if (pos == 1)
                            {
                                this._isRepG1[this._state.Index].Encode(this._rangeEncoder, 0);
                            }
                            else
                            {
                                this._isRepG1[this._state.Index].Encode(this._rangeEncoder, 1);
                                this._isRepG2[this._state.Index].Encode(this._rangeEncoder, pos - 2);
                            }
                        }

                        if (len == 1)
                        {
                            this._state.UpdateShortRep();
                        }
                        else
                        {
                            this._repMatchLenEncoder.Encode(this._rangeEncoder, len - Base.kMatchMinLen, posState);
                            this._state.UpdateRep();
                        }

                        uint distance = this._repDistances[pos];
                        if (pos != 0)
                        {
                            for (uint i = pos; i >= 1; i--)
                            {
                                this._repDistances[i] = this._repDistances[i - 1];
                            }

                            this._repDistances[0] = distance;
                        }
                    }
                    else
                    {
                        this._isRep[this._state.Index].Encode(this._rangeEncoder, 0);
                        this._state.UpdateMatch();
                        this._lenEncoder.Encode(this._rangeEncoder, len - Base.kMatchMinLen, posState);
                        pos -= Base.kNumRepDistances;
                        uint posSlot = GetPosSlot(pos);
                        uint lenToPosState = Base.GetLenToPosState(len);
                        this._posSlotEncoder[lenToPosState].Encode(this._rangeEncoder, posSlot);

                        if (posSlot >= Base.kStartPosModelIndex)
                        {
                            var footerBits = (int)((posSlot >> 1) - 1);
                            uint baseVal = (2 | (posSlot & 1)) << footerBits;
                            uint posReduced = pos - baseVal;

                            if (posSlot < Base.kEndPosModelIndex)
                            {
                                BitTreeEncoder.ReverseEncode(
                                    this._posEncoders, 
                                    baseVal - posSlot - 1, 
                                    this._rangeEncoder, 
                                    footerBits, 
                                    posReduced);
                            }
                            else
                            {
                                this._rangeEncoder.EncodeDirectBits(
                                    posReduced >> Base.kNumAlignBits, 
                                    footerBits - Base.kNumAlignBits);
                                this._posAlignEncoder.ReverseEncode(this._rangeEncoder, posReduced & Base.kAlignMask);
                                this._alignPriceCount++;
                            }
                        }

                        uint distance = pos;
                        for (uint i = Base.kNumRepDistances - 1; i >= 1; i--)
                        {
                            this._repDistances[i] = this._repDistances[i - 1];
                        }

                        this._repDistances[0] = distance;
                        this._matchPriceCount++;
                    }

                    this._previousByte = this._matchFinder.GetIndexByte((Int32)(len - 1 - this._additionalOffset));
                }

                this._additionalOffset -= len;
                this.nowPos64 += len;
                if (this._additionalOffset == 0)
                {
                    // if (!_fastMode)
                    if (this._matchPriceCount >= (1 << 7))
                    {
                        this.FillDistancesPrices();
                    }

                    if (this._alignPriceCount >= Base.kAlignTableSize)
                    {
                        this.FillAlignPrices();
                    }

                    inSize = this.nowPos64;
                    outSize = this._rangeEncoder.GetProcessedSizeAdd();
                    if (this._matchFinder.GetNumAvailableBytes() == 0)
                    {
                        this.Flush((UInt32)this.nowPos64);
                        return;
                    }

                    if (this.nowPos64 - progressPosValuePrev >= (1 << 12))
                    {
                        this._finished = false;
                        finished = false;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// The set coder properties.
        /// </summary>
        /// <param name="propIDs">
        /// The prop i ds.
        /// </param>
        /// <param name="properties">
        /// The properties.
        /// </param>
        /// <exception cref="InvalidParamException">
        /// </exception>
        public void SetCoderProperties(CoderPropID[] propIDs, object[] properties)
        {
            for (uint i = 0; i < properties.Length; i++)
            {
                object prop = properties[i];
                switch (propIDs[i])
                {
                    case CoderPropID.NumFastBytes:
                    {
                        if (!(prop is Int32))
                        {
                            throw new InvalidParamException();
                        }

                        var numFastBytes = (Int32)prop;
                        if (numFastBytes < 5 || numFastBytes > Base.kMatchMaxLen)
                        {
                            throw new InvalidParamException();
                        }

                        this._numFastBytes = (UInt32)numFastBytes;
                        break;
                    }

                    case CoderPropID.Algorithm:
                    {
                        /*
						if (!(prop is Int32))
							throw new InvalidParamException();
						Int32 maximize = (Int32)prop;
						_fastMode = (maximize == 0);
						_maxMode = (maximize >= 2);
						*/
                        break;
                    }

                    case CoderPropID.MatchFinder:
                    {
                        if (!(prop is String))
                        {
                            throw new InvalidParamException();
                        }

                        EMatchFinderType matchFinderIndexPrev = this._matchFinderType;
                        int m = FindMatchFinder(((string)prop).ToUpper());
                        if (m < 0)
                        {
                            throw new InvalidParamException();
                        }

                        this._matchFinderType = (EMatchFinderType)m;
                        if (this._matchFinder != null && matchFinderIndexPrev != this._matchFinderType)
                        {
                            this._dictionarySizePrev = 0xFFFFFFFF;
                            this._matchFinder = null;
                        }

                        break;
                    }

                    case CoderPropID.DictionarySize:
                    {
                        const int kDicLogSizeMaxCompress = 30;
                        if (!(prop is Int32))
                        {
                            throw new InvalidParamException();
                        }

                        ;
                        var dictionarySize = (Int32)prop;
                        if (dictionarySize < (UInt32)(1 << Base.kDicLogSizeMin)
                            || dictionarySize > (UInt32)(1 << kDicLogSizeMaxCompress))
                        {
                            throw new InvalidParamException();
                        }

                        this._dictionarySize = (UInt32)dictionarySize;
                        int dicLogSize;
                        for (dicLogSize = 0; dicLogSize < (UInt32)kDicLogSizeMaxCompress; dicLogSize++)
                        {
                            if (dictionarySize <= ((UInt32)1 << dicLogSize))
                            {
                                break;
                            }
                        }

                        this._distTableSize = (UInt32)dicLogSize * 2;
                        break;
                    }

                    case CoderPropID.PosStateBits:
                    {
                        if (!(prop is Int32))
                        {
                            throw new InvalidParamException();
                        }

                        var v = (Int32)prop;
                        if (v < 0 || v > (UInt32)Base.kNumPosStatesBitsEncodingMax)
                        {
                            throw new InvalidParamException();
                        }

                        this._posStateBits = v;
                        this._posStateMask = (((UInt32)1) << this._posStateBits) - 1;
                        break;
                    }

                    case CoderPropID.LitPosBits:
                    {
                        if (!(prop is Int32))
                        {
                            throw new InvalidParamException();
                        }

                        var v = (Int32)prop;
                        if (v < 0 || v > Base.kNumLitPosStatesBitsEncodingMax)
                        {
                            throw new InvalidParamException();
                        }

                        this._numLiteralPosStateBits = v;
                        break;
                    }

                    case CoderPropID.LitContextBits:
                    {
                        if (!(prop is Int32))
                        {
                            throw new InvalidParamException();
                        }

                        var v = (Int32)prop;
                        if (v < 0 || v > Base.kNumLitContextBitsMax)
                        {
                            throw new InvalidParamException();
                        }

                        ;
                        this._numLiteralContextBits = v;
                        break;
                    }

                    case CoderPropID.EndMarker:
                    {
                        if (!(prop is Boolean))
                        {
                            throw new InvalidParamException();
                        }

                        this.SetWriteEndMarkerMode((Boolean)prop);
                        break;
                    }

                    default:
                        throw new InvalidParamException();
                }
            }
        }

        /// <summary>
        /// The set train size.
        /// </summary>
        /// <param name="trainSize">
        /// The train size.
        /// </param>
        public void SetTrainSize(uint trainSize)
        {
            this._trainSize = trainSize;
        }

        /// <summary>
        /// The write coder properties.
        /// </summary>
        /// <param name="outStream">
        /// The out stream.
        /// </param>
        public void WriteCoderProperties(Stream outStream)
        {
            this.properties[0] =
                (Byte)((this._posStateBits * 5 + this._numLiteralPosStateBits) * 9 + this._numLiteralContextBits);
            for (int i = 0; i < 4; i++)
            {
                this.properties[1 + i] = (Byte)((this._dictionarySize >> (8 * i)) & 0xFF);
            }

            outStream.Write(this.properties, 0, kPropSize);
        }

        /// <summary>
        /// The find match finder.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int FindMatchFinder(string s)
        {
            for (int m = 0; m < kMatchFinderIDs.Length; m++)
            {
                if (s == kMatchFinderIDs[m])
                {
                    return m;
                }
            }

            return -1;
        }

        /// <summary>
        /// The get pos slot.
        /// </summary>
        /// <param name="pos">
        /// The pos.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        private static uint GetPosSlot(uint pos)
        {
            if (pos < (1 << 11))
            {
                return g_FastPos[pos];
            }

            if (pos < (1 << 21))
            {
                return (UInt32)(g_FastPos[pos >> 10] + 20);
            }

            return (UInt32)(g_FastPos[pos >> 20] + 40);
        }

        /// <summary>
        /// The get pos slot 2.
        /// </summary>
        /// <param name="pos">
        /// The pos.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        private static uint GetPosSlot2(uint pos)
        {
            if (pos < (1 << 17))
            {
                return (UInt32)(g_FastPos[pos >> 6] + 12);
            }

            if (pos < (1 << 27))
            {
                return (UInt32)(g_FastPos[pos >> 16] + 32);
            }

            return (UInt32)(g_FastPos[pos >> 26] + 52);
        }

        /// <summary>
        /// The backward.
        /// </summary>
        /// <param name="backRes">
        /// The back res.
        /// </param>
        /// <param name="cur">
        /// The cur.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        private uint Backward(out uint backRes, uint cur)
        {
            this._optimumEndIndex = cur;
            uint posMem = this._optimum[cur].PosPrev;
            uint backMem = this._optimum[cur].BackPrev;
            do
            {
                if (this._optimum[cur].Prev1IsChar)
                {
                    this._optimum[posMem].MakeAsChar();
                    this._optimum[posMem].PosPrev = posMem - 1;
                    if (this._optimum[cur].Prev2)
                    {
                        this._optimum[posMem - 1].Prev1IsChar = false;
                        this._optimum[posMem - 1].PosPrev = this._optimum[cur].PosPrev2;
                        this._optimum[posMem - 1].BackPrev = this._optimum[cur].BackPrev2;
                    }
                }

                uint posPrev = posMem;
                uint backCur = backMem;

                backMem = this._optimum[posPrev].BackPrev;
                posMem = this._optimum[posPrev].PosPrev;

                this._optimum[posPrev].BackPrev = backCur;
                this._optimum[posPrev].PosPrev = cur;
                cur = posPrev;
            }
            while (cur > 0);
            backRes = this._optimum[0].BackPrev;
            this._optimumCurrentIndex = this._optimum[0].PosPrev;
            return this._optimumCurrentIndex;
        }

        /// <summary>
        /// The base init.
        /// </summary>
        private void BaseInit()
        {
            this._state.Init();
            this._previousByte = 0;
            for (uint i = 0; i < Base.kNumRepDistances; i++)
            {
                this._repDistances[i] = 0;
            }
        }

        /// <summary>
        /// The change pair.
        /// </summary>
        /// <param name="smallDist">
        /// The small dist.
        /// </param>
        /// <param name="bigDist">
        /// The big dist.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ChangePair(uint smallDist, uint bigDist)
        {
            const int kDif = 7;
            return smallDist < ((UInt32)(1) << (32 - kDif)) && bigDist >= (smallDist << kDif);
        }

        /// <summary>
        /// The create.
        /// </summary>
        private void Create()
        {
            if (this._matchFinder == null)
            {
                var bt = new BinTree();
                int numHashBytes = 4;
                if (this._matchFinderType == EMatchFinderType.BT2)
                {
                    numHashBytes = 2;
                }

                bt.SetType(numHashBytes);
                this._matchFinder = bt;
            }

            this._literalEncoder.Create(this._numLiteralPosStateBits, this._numLiteralContextBits);

            if (this._dictionarySize == this._dictionarySizePrev && this._numFastBytesPrev == this._numFastBytes)
            {
                return;
            }

            this._matchFinder.Create(this._dictionarySize, kNumOpts, this._numFastBytes, Base.kMatchMaxLen + 1);
            this._dictionarySizePrev = this._dictionarySize;
            this._numFastBytesPrev = this._numFastBytes;
        }

        /// <summary>
        /// The fill align prices.
        /// </summary>
        private void FillAlignPrices()
        {
            for (uint i = 0; i < Base.kAlignTableSize; i++)
            {
                this._alignPrices[i] = this._posAlignEncoder.ReverseGetPrice(i);
            }

            this._alignPriceCount = 0;
        }

        /// <summary>
        /// The fill distances prices.
        /// </summary>
        private void FillDistancesPrices()
        {
            for (uint i = Base.kStartPosModelIndex; i < Base.kNumFullDistances; i++)
            {
                uint posSlot = GetPosSlot(i);
                var footerBits = (int)((posSlot >> 1) - 1);
                uint baseVal = (2 | (posSlot & 1)) << footerBits;
                this.tempPrices[i] = BitTreeEncoder.ReverseGetPrice(
                    this._posEncoders, 
                    baseVal - posSlot - 1, 
                    footerBits, 
                    i - baseVal);
            }

            for (uint lenToPosState = 0; lenToPosState < Base.kNumLenToPosStates; lenToPosState++)
            {
                uint posSlot;
                BitTreeEncoder encoder = this._posSlotEncoder[lenToPosState];

                uint st = lenToPosState << Base.kNumPosSlotBits;
                for (posSlot = 0; posSlot < this._distTableSize; posSlot++)
                {
                    this._posSlotPrices[st + posSlot] = encoder.GetPrice(posSlot);
                }

                for (posSlot = Base.kEndPosModelIndex; posSlot < this._distTableSize; posSlot++)
                {
                    this._posSlotPrices[st + posSlot] += (((posSlot >> 1) - 1) - Base.kNumAlignBits)
                                                          << BitEncoder.kNumBitPriceShiftBits;
                }

                uint st2 = lenToPosState * Base.kNumFullDistances;
                uint i;
                for (i = 0; i < Base.kStartPosModelIndex; i++)
                {
                    this._distancesPrices[st2 + i] = this._posSlotPrices[st + i];
                }

                for (; i < Base.kNumFullDistances; i++)
                {
                    this._distancesPrices[st2 + i] = this._posSlotPrices[st + GetPosSlot(i)] + this.tempPrices[i];
                }
            }

            this._matchPriceCount = 0;
        }

        /// <summary>
        /// The flush.
        /// </summary>
        /// <param name="nowPos">
        /// The now pos.
        /// </param>
        private void Flush(uint nowPos)
        {
            this.ReleaseMFStream();
            this.WriteEndMarker(nowPos & this._posStateMask);
            this._rangeEncoder.FlushData();
            this._rangeEncoder.FlushStream();
        }

        /// <summary>
        /// The get optimum.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="backRes">
        /// The back res.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        private uint GetOptimum(uint position, out uint backRes)
        {
            if (this._optimumEndIndex != this._optimumCurrentIndex)
            {
                uint lenRes = this._optimum[this._optimumCurrentIndex].PosPrev - this._optimumCurrentIndex;
                backRes = this._optimum[this._optimumCurrentIndex].BackPrev;
                this._optimumCurrentIndex = this._optimum[this._optimumCurrentIndex].PosPrev;
                return lenRes;
            }

            this._optimumCurrentIndex = this._optimumEndIndex = 0;

            uint lenMain, numDistancePairs;
            if (!this._longestMatchWasFound)
            {
                this.ReadMatchDistances(out lenMain, out numDistancePairs);
            }
            else
            {
                lenMain = this._longestMatchLength;
                numDistancePairs = this._numDistancePairs;
                this._longestMatchWasFound = false;
            }

            uint numAvailableBytes = this._matchFinder.GetNumAvailableBytes() + 1;
            if (numAvailableBytes < 2)
            {
                backRes = 0xFFFFFFFF;
                return 1;
            }

            if (numAvailableBytes > Base.kMatchMaxLen)
            {
                numAvailableBytes = Base.kMatchMaxLen;
            }

            uint repMaxIndex = 0;
            uint i;
            for (i = 0; i < Base.kNumRepDistances; i++)
            {
                this.reps[i] = this._repDistances[i];
                this.repLens[i] = this._matchFinder.GetMatchLen(0 - 1, this.reps[i], Base.kMatchMaxLen);
                if (this.repLens[i] > this.repLens[repMaxIndex])
                {
                    repMaxIndex = i;
                }
            }

            if (this.repLens[repMaxIndex] >= this._numFastBytes)
            {
                backRes = repMaxIndex;
                uint lenRes = this.repLens[repMaxIndex];
                this.MovePos(lenRes - 1);
                return lenRes;
            }

            if (lenMain >= this._numFastBytes)
            {
                backRes = this._matchDistances[numDistancePairs - 1] + Base.kNumRepDistances;
                this.MovePos(lenMain - 1);
                return lenMain;
            }

            byte currentByte = this._matchFinder.GetIndexByte(0 - 1);
            byte matchByte = this._matchFinder.GetIndexByte((Int32)(0 - this._repDistances[0] - 1 - 1));

            if (lenMain < 2 && currentByte != matchByte && this.repLens[repMaxIndex] < 2)
            {
                backRes = 0xFFFFFFFF;
                return 1;
            }

            this._optimum[0].State = this._state;

            uint posState = position & this._posStateMask;

            this._optimum[1].Price =
                this._isMatch[(this._state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice0()
                + this._literalEncoder.GetSubCoder(position, this._previousByte)
                      .GetPrice(!this._state.IsCharState(), matchByte, currentByte);
            this._optimum[1].MakeAsChar();

            uint matchPrice = this._isMatch[(this._state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice1();
            uint repMatchPrice = matchPrice + this._isRep[this._state.Index].GetPrice1();

            if (matchByte == currentByte)
            {
                uint shortRepPrice = repMatchPrice + this.GetRepLen1Price(this._state, posState);
                if (shortRepPrice < this._optimum[1].Price)
                {
                    this._optimum[1].Price = shortRepPrice;
                    this._optimum[1].MakeAsShortRep();
                }
            }

            uint lenEnd = (lenMain >= this.repLens[repMaxIndex]) ? lenMain : this.repLens[repMaxIndex];

            if (lenEnd < 2)
            {
                backRes = this._optimum[1].BackPrev;
                return 1;
            }

            this._optimum[1].PosPrev = 0;

            this._optimum[0].Backs0 = this.reps[0];
            this._optimum[0].Backs1 = this.reps[1];
            this._optimum[0].Backs2 = this.reps[2];
            this._optimum[0].Backs3 = this.reps[3];

            uint len = lenEnd;
            do this._optimum[len--].Price = kIfinityPrice;
            while (len >= 2);

            for (i = 0; i < Base.kNumRepDistances; i++)
            {
                uint repLen = this.repLens[i];
                if (repLen < 2)
                {
                    continue;
                }

                uint price = repMatchPrice + this.GetPureRepPrice(i, this._state, posState);
                do
                {
                    uint curAndLenPrice = price + this._repMatchLenEncoder.GetPrice(repLen - 2, posState);
                    Optimal optimum = this._optimum[repLen];
                    if (curAndLenPrice < optimum.Price)
                    {
                        optimum.Price = curAndLenPrice;
                        optimum.PosPrev = 0;
                        optimum.BackPrev = i;
                        optimum.Prev1IsChar = false;
                    }
                }
                while (--repLen >= 2);
            }

            uint normalMatchPrice = matchPrice + this._isRep[this._state.Index].GetPrice0();

            len = (this.repLens[0] >= 2) ? this.repLens[0] + 1 : 2;
            if (len <= lenMain)
            {
                uint offs = 0;
                while (len > this._matchDistances[offs])
                {
                    offs += 2;
                }

                for (;; len++)
                {
                    uint distance = this._matchDistances[offs + 1];
                    uint curAndLenPrice = normalMatchPrice + this.GetPosLenPrice(distance, len, posState);
                    Optimal optimum = this._optimum[len];
                    if (curAndLenPrice < optimum.Price)
                    {
                        optimum.Price = curAndLenPrice;
                        optimum.PosPrev = 0;
                        optimum.BackPrev = distance + Base.kNumRepDistances;
                        optimum.Prev1IsChar = false;
                    }

                    if (len == this._matchDistances[offs])
                    {
                        offs += 2;
                        if (offs == numDistancePairs)
                        {
                            break;
                        }
                    }
                }
            }

            uint cur = 0;

            while (true)
            {
                cur++;
                if (cur == lenEnd)
                {
                    return this.Backward(out backRes, cur);
                }

                uint newLen;
                this.ReadMatchDistances(out newLen, out numDistancePairs);
                if (newLen >= this._numFastBytes)
                {
                    this._numDistancePairs = numDistancePairs;
                    this._longestMatchLength = newLen;
                    this._longestMatchWasFound = true;
                    return this.Backward(out backRes, cur);
                }

                position++;
                uint posPrev = this._optimum[cur].PosPrev;
                Base.State state;
                if (this._optimum[cur].Prev1IsChar)
                {
                    posPrev--;
                    if (this._optimum[cur].Prev2)
                    {
                        state = this._optimum[this._optimum[cur].PosPrev2].State;
                        if (this._optimum[cur].BackPrev2 < Base.kNumRepDistances)
                        {
                            state.UpdateRep();
                        }
                        else
                        {
                            state.UpdateMatch();
                        }
                    }
                    else
                    {
                        state = this._optimum[posPrev].State;
                    }

                    state.UpdateChar();
                }
                else
                {
                    state = this._optimum[posPrev].State;
                }

                if (posPrev == cur - 1)
                {
                    if (this._optimum[cur].IsShortRep())
                    {
                        state.UpdateShortRep();
                    }
                    else
                    {
                        state.UpdateChar();
                    }
                }
                else
                {
                    uint pos;
                    if (this._optimum[cur].Prev1IsChar && this._optimum[cur].Prev2)
                    {
                        posPrev = this._optimum[cur].PosPrev2;
                        pos = this._optimum[cur].BackPrev2;
                        state.UpdateRep();
                    }
                    else
                    {
                        pos = this._optimum[cur].BackPrev;
                        if (pos < Base.kNumRepDistances)
                        {
                            state.UpdateRep();
                        }
                        else
                        {
                            state.UpdateMatch();
                        }
                    }

                    Optimal opt = this._optimum[posPrev];
                    if (pos < Base.kNumRepDistances)
                    {
                        if (pos == 0)
                        {
                            this.reps[0] = opt.Backs0;
                            this.reps[1] = opt.Backs1;
                            this.reps[2] = opt.Backs2;
                            this.reps[3] = opt.Backs3;
                        }
                        else if (pos == 1)
                        {
                            this.reps[0] = opt.Backs1;
                            this.reps[1] = opt.Backs0;
                            this.reps[2] = opt.Backs2;
                            this.reps[3] = opt.Backs3;
                        }
                        else if (pos == 2)
                        {
                            this.reps[0] = opt.Backs2;
                            this.reps[1] = opt.Backs0;
                            this.reps[2] = opt.Backs1;
                            this.reps[3] = opt.Backs3;
                        }
                        else
                        {
                            this.reps[0] = opt.Backs3;
                            this.reps[1] = opt.Backs0;
                            this.reps[2] = opt.Backs1;
                            this.reps[3] = opt.Backs2;
                        }
                    }
                    else
                    {
                        this.reps[0] = pos - Base.kNumRepDistances;
                        this.reps[1] = opt.Backs0;
                        this.reps[2] = opt.Backs1;
                        this.reps[3] = opt.Backs2;
                    }
                }

                this._optimum[cur].State = state;
                this._optimum[cur].Backs0 = this.reps[0];
                this._optimum[cur].Backs1 = this.reps[1];
                this._optimum[cur].Backs2 = this.reps[2];
                this._optimum[cur].Backs3 = this.reps[3];
                uint curPrice = this._optimum[cur].Price;

                currentByte = this._matchFinder.GetIndexByte(0 - 1);
                matchByte = this._matchFinder.GetIndexByte((Int32)(0 - this.reps[0] - 1 - 1));

                posState = position & this._posStateMask;

                uint curAnd1Price = curPrice
                                      + this._isMatch[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice0()
                                      + this._literalEncoder.GetSubCoder(
                                          position, 
                                          this._matchFinder.GetIndexByte(0 - 2))
                                            .GetPrice(!state.IsCharState(), matchByte, currentByte);

                Optimal nextOptimum = this._optimum[cur + 1];

                bool nextIsChar = false;
                if (curAnd1Price < nextOptimum.Price)
                {
                    nextOptimum.Price = curAnd1Price;
                    nextOptimum.PosPrev = cur;
                    nextOptimum.MakeAsChar();
                    nextIsChar = true;
                }

                matchPrice = curPrice + this._isMatch[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice1();
                repMatchPrice = matchPrice + this._isRep[state.Index].GetPrice1();

                if (matchByte == currentByte && !(nextOptimum.PosPrev < cur && nextOptimum.BackPrev == 0))
                {
                    uint shortRepPrice = repMatchPrice + this.GetRepLen1Price(state, posState);
                    if (shortRepPrice <= nextOptimum.Price)
                    {
                        nextOptimum.Price = shortRepPrice;
                        nextOptimum.PosPrev = cur;
                        nextOptimum.MakeAsShortRep();
                        nextIsChar = true;
                    }
                }

                uint numAvailableBytesFull = this._matchFinder.GetNumAvailableBytes() + 1;
                numAvailableBytesFull = Math.Min(kNumOpts - 1 - cur, numAvailableBytesFull);
                numAvailableBytes = numAvailableBytesFull;

                if (numAvailableBytes < 2)
                {
                    continue;
                }

                if (numAvailableBytes > this._numFastBytes)
                {
                    numAvailableBytes = this._numFastBytes;
                }

                if (!nextIsChar && matchByte != currentByte)
                {
                    // try Literal + rep0
                    uint t = Math.Min(numAvailableBytesFull - 1, this._numFastBytes);
                    uint lenTest2 = this._matchFinder.GetMatchLen(0, this.reps[0], t);
                    if (lenTest2 >= 2)
                    {
                        Base.State state2 = state;
                        state2.UpdateChar();
                        uint posStateNext = (position + 1) & this._posStateMask;
                        uint nextRepMatchPrice = curAnd1Price
                                                   + this._isMatch[
                                                       (state2.Index << Base.kNumPosStatesBitsMax) + posStateNext]
                                                         .GetPrice1() + this._isRep[state2.Index].GetPrice1();
                        {
                            uint offset = cur + 1 + lenTest2;
                            while (lenEnd < offset)
                            {
                                this._optimum[++lenEnd].Price = kIfinityPrice;
                            }

                            uint curAndLenPrice = nextRepMatchPrice
                                                    + this.GetRepPrice(0, lenTest2, state2, posStateNext);
                            Optimal optimum = this._optimum[offset];
                            if (curAndLenPrice < optimum.Price)
                            {
                                optimum.Price = curAndLenPrice;
                                optimum.PosPrev = cur + 1;
                                optimum.BackPrev = 0;
                                optimum.Prev1IsChar = true;
                                optimum.Prev2 = false;
                            }
                        }
                    }
                }

                uint startLen = 2; // speed optimization 

                for (uint repIndex = 0; repIndex < Base.kNumRepDistances; repIndex++)
                {
                    uint lenTest = this._matchFinder.GetMatchLen(0 - 1, this.reps[repIndex], numAvailableBytes);
                    if (lenTest < 2)
                    {
                        continue;
                    }

                    uint lenTestTemp = lenTest;
                    do
                    {
                        while (lenEnd < cur + lenTest)
                        {
                            this._optimum[++lenEnd].Price = kIfinityPrice;
                        }

                        uint curAndLenPrice = repMatchPrice + this.GetRepPrice(repIndex, lenTest, state, posState);
                        Optimal optimum = this._optimum[cur + lenTest];
                        if (curAndLenPrice < optimum.Price)
                        {
                            optimum.Price = curAndLenPrice;
                            optimum.PosPrev = cur;
                            optimum.BackPrev = repIndex;
                            optimum.Prev1IsChar = false;
                        }
                    }
                    while (--lenTest >= 2);
                    lenTest = lenTestTemp;

                    if (repIndex == 0)
                    {
                        startLen = lenTest + 1;
                    }

                    // if (_maxMode)
                    if (lenTest < numAvailableBytesFull)
                    {
                        uint t = Math.Min(numAvailableBytesFull - 1 - lenTest, this._numFastBytes);
                        uint lenTest2 = this._matchFinder.GetMatchLen((Int32)lenTest, this.reps[repIndex], t);
                        if (lenTest2 >= 2)
                        {
                            Base.State state2 = state;
                            state2.UpdateRep();
                            uint posStateNext = (position + lenTest) & this._posStateMask;
                            uint curAndLenCharPrice = repMatchPrice
                                                        + this.GetRepPrice(repIndex, lenTest, state, posState)
                                                        + this._isMatch[
                                                            (state2.Index << Base.kNumPosStatesBitsMax) + posStateNext]
                                                              .GetPrice0()
                                                        + this._literalEncoder.GetSubCoder(
                                                            position + lenTest, 
                                                            this._matchFinder.GetIndexByte((Int32)lenTest - 1 - 1))
                                                              .GetPrice(
                                                                  true, 
                                                                  this._matchFinder.GetIndexByte(
                                                                      (Int32)lenTest - 1
                                                                      - (Int32)(this.reps[repIndex] + 1)), 
                                                                  this._matchFinder.GetIndexByte((Int32)lenTest - 1));
                            state2.UpdateChar();
                            posStateNext = (position + lenTest + 1) & this._posStateMask;
                            uint nextMatchPrice = curAndLenCharPrice
                                                    + this._isMatch[
                                                        (state2.Index << Base.kNumPosStatesBitsMax) + posStateNext]
                                                          .GetPrice1();
                            uint nextRepMatchPrice = nextMatchPrice + this._isRep[state2.Index].GetPrice1();
                            {
                                // for(; lenTest2 >= 2; lenTest2--)
                                uint offset = lenTest + 1 + lenTest2;
                                while (lenEnd < cur + offset)
                                {
                                    this._optimum[++lenEnd].Price = kIfinityPrice;
                                }

                                uint curAndLenPrice = nextRepMatchPrice
                                                        + this.GetRepPrice(0, lenTest2, state2, posStateNext);
                                Optimal optimum = this._optimum[cur + offset];
                                if (curAndLenPrice < optimum.Price)
                                {
                                    optimum.Price = curAndLenPrice;
                                    optimum.PosPrev = cur + lenTest + 1;
                                    optimum.BackPrev = 0;
                                    optimum.Prev1IsChar = true;
                                    optimum.Prev2 = true;
                                    optimum.PosPrev2 = cur;
                                    optimum.BackPrev2 = repIndex;
                                }
                            }
                        }
                    }
                }

                if (newLen > numAvailableBytes)
                {
                    newLen = numAvailableBytes;
                    for (numDistancePairs = 0; newLen > this._matchDistances[numDistancePairs]; numDistancePairs += 2)
                    {
                        ;
                    }

                    this._matchDistances[numDistancePairs] = newLen;
                    numDistancePairs += 2;
                }

                if (newLen >= startLen)
                {
                    normalMatchPrice = matchPrice + this._isRep[state.Index].GetPrice0();
                    while (lenEnd < cur + newLen)
                    {
                        this._optimum[++lenEnd].Price = kIfinityPrice;
                    }

                    uint offs = 0;
                    while (startLen > this._matchDistances[offs])
                    {
                        offs += 2;
                    }

                    for (uint lenTest = startLen;; lenTest++)
                    {
                        uint curBack = this._matchDistances[offs + 1];
                        uint curAndLenPrice = normalMatchPrice + this.GetPosLenPrice(curBack, lenTest, posState);
                        Optimal optimum = this._optimum[cur + lenTest];
                        if (curAndLenPrice < optimum.Price)
                        {
                            optimum.Price = curAndLenPrice;
                            optimum.PosPrev = cur;
                            optimum.BackPrev = curBack + Base.kNumRepDistances;
                            optimum.Prev1IsChar = false;
                        }

                        if (lenTest == this._matchDistances[offs])
                        {
                            if (lenTest < numAvailableBytesFull)
                            {
                                uint t = Math.Min(numAvailableBytesFull - 1 - lenTest, this._numFastBytes);
                                uint lenTest2 = this._matchFinder.GetMatchLen((Int32)lenTest, curBack, t);
                                if (lenTest2 >= 2)
                                {
                                    Base.State state2 = state;
                                    state2.UpdateMatch();
                                    uint posStateNext = (position + lenTest) & this._posStateMask;
                                    uint curAndLenCharPrice = curAndLenPrice
                                                                + this._isMatch[
                                                                    (state2.Index << Base.kNumPosStatesBitsMax)
                                                                    + posStateNext].GetPrice0()
                                                                + this._literalEncoder.GetSubCoder(
                                                                    position + lenTest, 
                                                                    this._matchFinder.GetIndexByte(
                                                                        (Int32)lenTest - 1 - 1))
                                                                      .GetPrice(
                                                                          true, 
                                                                          this._matchFinder.GetIndexByte(
                                                                              (Int32)lenTest - (Int32)(curBack + 1) - 1), 
                                                                          this._matchFinder.GetIndexByte(
                                                                              (Int32)lenTest - 1));
                                    state2.UpdateChar();
                                    posStateNext = (position + lenTest + 1) & this._posStateMask;
                                    uint nextMatchPrice = curAndLenCharPrice
                                                            + this._isMatch[
                                                                (state2.Index << Base.kNumPosStatesBitsMax)
                                                                + posStateNext].GetPrice1();
                                    uint nextRepMatchPrice = nextMatchPrice + this._isRep[state2.Index].GetPrice1();

                                    uint offset = lenTest + 1 + lenTest2;
                                    while (lenEnd < cur + offset)
                                    {
                                        this._optimum[++lenEnd].Price = kIfinityPrice;
                                    }

                                    curAndLenPrice = nextRepMatchPrice
                                                     + this.GetRepPrice(0, lenTest2, state2, posStateNext);
                                    optimum = this._optimum[cur + offset];
                                    if (curAndLenPrice < optimum.Price)
                                    {
                                        optimum.Price = curAndLenPrice;
                                        optimum.PosPrev = cur + lenTest + 1;
                                        optimum.BackPrev = 0;
                                        optimum.Prev1IsChar = true;
                                        optimum.Prev2 = true;
                                        optimum.PosPrev2 = cur;
                                        optimum.BackPrev2 = curBack + Base.kNumRepDistances;
                                    }
                                }
                            }

                            offs += 2;
                            if (offs == numDistancePairs)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The get pos len price.
        /// </summary>
        /// <param name="pos">
        /// The pos.
        /// </param>
        /// <param name="len">
        /// The len.
        /// </param>
        /// <param name="posState">
        /// The pos state.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        private uint GetPosLenPrice(uint pos, uint len, uint posState)
        {
            uint price;
            uint lenToPosState = Base.GetLenToPosState(len);
            if (pos < Base.kNumFullDistances)
            {
                price = this._distancesPrices[(lenToPosState * Base.kNumFullDistances) + pos];
            }
            else
            {
                price = this._posSlotPrices[(lenToPosState << Base.kNumPosSlotBits) + GetPosSlot2(pos)]
                        + this._alignPrices[pos & Base.kAlignMask];
            }

            return price + this._lenEncoder.GetPrice(len - Base.kMatchMinLen, posState);
        }

        /// <summary>
        /// The get pure rep price.
        /// </summary>
        /// <param name="repIndex">
        /// The rep index.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="posState">
        /// The pos state.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        private uint GetPureRepPrice(uint repIndex, Base.State state, uint posState)
        {
            uint price;
            if (repIndex == 0)
            {
                price = this._isRepG0[state.Index].GetPrice0();
                price += this._isRep0Long[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice1();
            }
            else
            {
                price = this._isRepG0[state.Index].GetPrice1();
                if (repIndex == 1)
                {
                    price += this._isRepG1[state.Index].GetPrice0();
                }
                else
                {
                    price += this._isRepG1[state.Index].GetPrice1();
                    price += this._isRepG2[state.Index].GetPrice(repIndex - 2);
                }
            }

            return price;
        }

        /// <summary>
        /// The get rep len 1 price.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="posState">
        /// The pos state.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        private uint GetRepLen1Price(Base.State state, uint posState)
        {
            return this._isRepG0[state.Index].GetPrice0()
                   + this._isRep0Long[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice0();
        }

        /// <summary>
        /// The get rep price.
        /// </summary>
        /// <param name="repIndex">
        /// The rep index.
        /// </param>
        /// <param name="len">
        /// The len.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="posState">
        /// The pos state.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        private uint GetRepPrice(uint repIndex, uint len, Base.State state, uint posState)
        {
            uint price = this._repMatchLenEncoder.GetPrice(len - Base.kMatchMinLen, posState);
            return price + this.GetPureRepPrice(repIndex, state, posState);
        }

        /// <summary>
        /// The init.
        /// </summary>
        private void Init()
        {
            this.BaseInit();
            this._rangeEncoder.Init();

            uint i;
            for (i = 0; i < Base.kNumStates; i++)
            {
                for (uint j = 0; j <= this._posStateMask; j++)
                {
                    uint complexState = (i << Base.kNumPosStatesBitsMax) + j;
                    this._isMatch[complexState].Init();
                    this._isRep0Long[complexState].Init();
                }

                this._isRep[i].Init();
                this._isRepG0[i].Init();
                this._isRepG1[i].Init();
                this._isRepG2[i].Init();
            }

            this._literalEncoder.Init();
            for (i = 0; i < Base.kNumLenToPosStates; i++)
            {
                this._posSlotEncoder[i].Init();
            }

            for (i = 0; i < Base.kNumFullDistances - Base.kEndPosModelIndex; i++)
            {
                this._posEncoders[i].Init();
            }

            this._lenEncoder.Init((UInt32)1 << this._posStateBits);
            this._repMatchLenEncoder.Init((UInt32)1 << this._posStateBits);

            this._posAlignEncoder.Init();

            this._longestMatchWasFound = false;
            this._optimumEndIndex = 0;
            this._optimumCurrentIndex = 0;
            this._additionalOffset = 0;
        }

        /// <summary>
        /// The move pos.
        /// </summary>
        /// <param name="num">
        /// The num.
        /// </param>
        private void MovePos(uint num)
        {
            if (num > 0)
            {
                this._matchFinder.Skip(num);
                this._additionalOffset += num;
            }
        }

        /// <summary>
        /// The read match distances.
        /// </summary>
        /// <param name="lenRes">
        /// The len res.
        /// </param>
        /// <param name="numDistancePairs">
        /// The num distance pairs.
        /// </param>
        private void ReadMatchDistances(out uint lenRes, out uint numDistancePairs)
        {
            lenRes = 0;
            numDistancePairs = this._matchFinder.GetMatches(this._matchDistances);
            if (numDistancePairs > 0)
            {
                lenRes = this._matchDistances[numDistancePairs - 2];
                if (lenRes == this._numFastBytes)
                {
                    lenRes += this._matchFinder.GetMatchLen(
                        (int)lenRes - 1, 
                        this._matchDistances[numDistancePairs - 1], 
                        Base.kMatchMaxLen - lenRes);
                }
            }

            this._additionalOffset++;
        }

        /// <summary>
        /// The release mf stream.
        /// </summary>
        private void ReleaseMFStream()
        {
            if (this._matchFinder != null && this._needReleaseMFStream)
            {
                this._matchFinder.ReleaseStream();
                this._needReleaseMFStream = false;
            }
        }

        /// <summary>
        /// The release out stream.
        /// </summary>
        private void ReleaseOutStream()
        {
            this._rangeEncoder.ReleaseStream();
        }

        /// <summary>
        /// The release streams.
        /// </summary>
        private void ReleaseStreams()
        {
            this.ReleaseMFStream();
            this.ReleaseOutStream();
        }

        /// <summary>
        /// The set out stream.
        /// </summary>
        /// <param name="outStream">
        /// The out stream.
        /// </param>
        private void SetOutStream(Stream outStream)
        {
            this._rangeEncoder.SetStream(outStream);
        }

        /// <summary>
        /// The set streams.
        /// </summary>
        /// <param name="inStream">
        /// The in stream.
        /// </param>
        /// <param name="outStream">
        /// The out stream.
        /// </param>
        /// <param name="inSize">
        /// The in size.
        /// </param>
        /// <param name="outSize">
        /// The out size.
        /// </param>
        private void SetStreams(Stream inStream, Stream outStream, long inSize, long outSize)
        {
            this._inStream = inStream;
            this._finished = false;
            this.Create();
            this.SetOutStream(outStream);
            this.Init();
            {
                // if (!_fastMode)
                this.FillDistancesPrices();
                this.FillAlignPrices();
            }

            this._lenEncoder.SetTableSize(this._numFastBytes + 1 - Base.kMatchMinLen);
            this._lenEncoder.UpdateTables((UInt32)1 << this._posStateBits);
            this._repMatchLenEncoder.SetTableSize(this._numFastBytes + 1 - Base.kMatchMinLen);
            this._repMatchLenEncoder.UpdateTables((UInt32)1 << this._posStateBits);

            this.nowPos64 = 0;
        }

        /// <summary>
        /// The set write end marker mode.
        /// </summary>
        /// <param name="writeEndMarker">
        /// The write end marker.
        /// </param>
        private void SetWriteEndMarkerMode(bool writeEndMarker)
        {
            this._writeEndMark = writeEndMarker;
        }

        /// <summary>
        /// The write end marker.
        /// </summary>
        /// <param name="posState">
        /// The pos state.
        /// </param>
        private void WriteEndMarker(uint posState)
        {
            if (!this._writeEndMark)
            {
                return;
            }

            this._isMatch[(this._state.Index << Base.kNumPosStatesBitsMax) + posState].Encode(this._rangeEncoder, 1);
            this._isRep[this._state.Index].Encode(this._rangeEncoder, 0);
            this._state.UpdateMatch();
            uint len = Base.kMatchMinLen;
            this._lenEncoder.Encode(this._rangeEncoder, len - Base.kMatchMinLen, posState);
            uint posSlot = (1 << Base.kNumPosSlotBits) - 1;
            uint lenToPosState = Base.GetLenToPosState(len);
            this._posSlotEncoder[lenToPosState].Encode(this._rangeEncoder, posSlot);
            int footerBits = 30;
            uint posReduced = (((UInt32)1) << footerBits) - 1;
            this._rangeEncoder.EncodeDirectBits(posReduced >> Base.kNumAlignBits, footerBits - Base.kNumAlignBits);
            this._posAlignEncoder.ReverseEncode(this._rangeEncoder, posReduced & Base.kAlignMask);
        }

        /// <summary>
        /// The len encoder.
        /// </summary>
        private class LenEncoder
        {
            /// <summary>
            /// The _low coder.
            /// </summary>
            private readonly BitTreeEncoder[] _lowCoder = new BitTreeEncoder[Base.kNumPosStatesEncodingMax];

            /// <summary>
            /// The _mid coder.
            /// </summary>
            private readonly BitTreeEncoder[] _midCoder = new BitTreeEncoder[Base.kNumPosStatesEncodingMax];

            /// <summary>
            /// The _choice.
            /// </summary>
            private BitEncoder _choice = new BitEncoder();

            /// <summary>
            /// The _choice 2.
            /// </summary>
            private BitEncoder _choice2 = new BitEncoder();

            /// <summary>
            /// The _high coder.
            /// </summary>
            private BitTreeEncoder _highCoder = new BitTreeEncoder(Base.kNumHighLenBits);

            /// <summary>
            /// Initializes a new instance of the <see cref="LenEncoder"/> class.
            /// </summary>
            public LenEncoder()
            {
                for (uint posState = 0; posState < Base.kNumPosStatesEncodingMax; posState++)
                {
                    this._lowCoder[posState] = new BitTreeEncoder(Base.kNumLowLenBits);
                    this._midCoder[posState] = new BitTreeEncoder(Base.kNumMidLenBits);
                }
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
            /// <param name="posState">
            /// The pos state.
            /// </param>
            public void Encode(Encoder rangeEncoder, uint symbol, uint posState)
            {
                if (symbol < Base.kNumLowLenSymbols)
                {
                    this._choice.Encode(rangeEncoder, 0);
                    this._lowCoder[posState].Encode(rangeEncoder, symbol);
                }
                else
                {
                    symbol -= Base.kNumLowLenSymbols;
                    this._choice.Encode(rangeEncoder, 1);
                    if (symbol < Base.kNumMidLenSymbols)
                    {
                        this._choice2.Encode(rangeEncoder, 0);
                        this._midCoder[posState].Encode(rangeEncoder, symbol);
                    }
                    else
                    {
                        this._choice2.Encode(rangeEncoder, 1);
                        this._highCoder.Encode(rangeEncoder, symbol - Base.kNumMidLenSymbols);
                    }
                }
            }

            /// <summary>
            /// The init.
            /// </summary>
            /// <param name="numPosStates">
            /// The num pos states.
            /// </param>
            public void Init(uint numPosStates)
            {
                this._choice.Init();
                this._choice2.Init();
                for (uint posState = 0; posState < numPosStates; posState++)
                {
                    this._lowCoder[posState].Init();
                    this._midCoder[posState].Init();
                }

                this._highCoder.Init();
            }

            /// <summary>
            /// The set prices.
            /// </summary>
            /// <param name="posState">
            /// The pos state.
            /// </param>
            /// <param name="numSymbols">
            /// The num symbols.
            /// </param>
            /// <param name="prices">
            /// The prices.
            /// </param>
            /// <param name="st">
            /// The st.
            /// </param>
            public void SetPrices(uint posState, uint numSymbols, uint[] prices, uint st)
            {
                uint a0 = this._choice.GetPrice0();
                uint a1 = this._choice.GetPrice1();
                uint b0 = a1 + this._choice2.GetPrice0();
                uint b1 = a1 + this._choice2.GetPrice1();
                uint i = 0;
                for (i = 0; i < Base.kNumLowLenSymbols; i++)
                {
                    if (i >= numSymbols)
                    {
                        return;
                    }

                    prices[st + i] = a0 + this._lowCoder[posState].GetPrice(i);
                }

                for (; i < Base.kNumLowLenSymbols + Base.kNumMidLenSymbols; i++)
                {
                    if (i >= numSymbols)
                    {
                        return;
                    }

                    prices[st + i] = b0 + this._midCoder[posState].GetPrice(i - Base.kNumLowLenSymbols);
                }

                for (; i < numSymbols; i++)
                {
                    prices[st + i] = b1 + this._highCoder.GetPrice(i - Base.kNumLowLenSymbols - Base.kNumMidLenSymbols);
                }
            }
        };

        /// <summary>
        /// The len price table encoder.
        /// </summary>
        private class LenPriceTableEncoder : LenEncoder
        {
            /// <summary>
            /// The _counters.
            /// </summary>
            private readonly uint[] _counters = new uint[Base.kNumPosStatesEncodingMax];

            /// <summary>
            /// The _prices.
            /// </summary>
            private readonly uint[] _prices = new uint[Base.kNumLenSymbols << Base.kNumPosStatesBitsEncodingMax];

            /// <summary>
            /// The _table size.
            /// </summary>
            private uint _tableSize;

            /// <summary>
            /// The encode.
            /// </summary>
            /// <param name="rangeEncoder">
            /// The range encoder.
            /// </param>
            /// <param name="symbol">
            /// The symbol.
            /// </param>
            /// <param name="posState">
            /// The pos state.
            /// </param>
            public new void Encode(Encoder rangeEncoder, uint symbol, uint posState)
            {
                base.Encode(rangeEncoder, symbol, posState);
                if (--this._counters[posState] == 0)
                {
                    this.UpdateTable(posState);
                }
            }

            /// <summary>
            /// The get price.
            /// </summary>
            /// <param name="symbol">
            /// The symbol.
            /// </param>
            /// <param name="posState">
            /// The pos state.
            /// </param>
            /// <returns>
            /// The <see cref="uint"/>.
            /// </returns>
            public uint GetPrice(uint symbol, uint posState)
            {
                return this._prices[posState * Base.kNumLenSymbols + symbol];
            }

            /// <summary>
            /// The set table size.
            /// </summary>
            /// <param name="tableSize">
            /// The table size.
            /// </param>
            public void SetTableSize(uint tableSize)
            {
                this._tableSize = tableSize;
            }

            /// <summary>
            /// The update tables.
            /// </summary>
            /// <param name="numPosStates">
            /// The num pos states.
            /// </param>
            public void UpdateTables(uint numPosStates)
            {
                for (uint posState = 0; posState < numPosStates; posState++)
                {
                    this.UpdateTable(posState);
                }
            }

            /// <summary>
            /// The update table.
            /// </summary>
            /// <param name="posState">
            /// The pos state.
            /// </param>
            private void UpdateTable(uint posState)
            {
                this.SetPrices(posState, this._tableSize, this._prices, posState * Base.kNumLenSymbols);
                this._counters[posState] = this._tableSize;
            }
        }

        /// <summary>
        /// The literal encoder.
        /// </summary>
        private class LiteralEncoder
        {
            /// <summary>
            /// The m_ coders.
            /// </summary>
            private Encoder2[] m_Coders;

            /// <summary>
            /// The m_ num pos bits.
            /// </summary>
            private int m_NumPosBits;

            /// <summary>
            /// The m_ num prev bits.
            /// </summary>
            private int m_NumPrevBits;

            /// <summary>
            /// The m_ pos mask.
            /// </summary>
            private uint m_PosMask;

            /// <summary>
            /// The create.
            /// </summary>
            /// <param name="numPosBits">
            /// The num pos bits.
            /// </param>
            /// <param name="numPrevBits">
            /// The num prev bits.
            /// </param>
            public void Create(int numPosBits, int numPrevBits)
            {
                if (this.m_Coders != null && this.m_NumPrevBits == numPrevBits && this.m_NumPosBits == numPosBits)
                {
                    return;
                }

                this.m_NumPosBits = numPosBits;
                this.m_PosMask = ((uint)1 << numPosBits) - 1;
                this.m_NumPrevBits = numPrevBits;
                uint numStates = (uint)1 << (this.m_NumPrevBits + this.m_NumPosBits);
                this.m_Coders = new Encoder2[numStates];
                for (uint i = 0; i < numStates; i++)
                {
                    this.m_Coders[i].Create();
                }
            }

            /// <summary>
            /// The get sub coder.
            /// </summary>
            /// <param name="pos">
            /// The pos.
            /// </param>
            /// <param name="prevByte">
            /// The prev byte.
            /// </param>
            /// <returns>
            /// The <see cref="Encoder2"/>.
            /// </returns>
            public Encoder2 GetSubCoder(uint pos, byte prevByte)
            {
                return
                    this.m_Coders[
                        ((pos & this.m_PosMask) << this.m_NumPrevBits) + (uint)(prevByte >> (8 - this.m_NumPrevBits))];
            }

            /// <summary>
            /// The init.
            /// </summary>
            public void Init()
            {
                uint numStates = (uint)1 << (this.m_NumPrevBits + this.m_NumPosBits);
                for (uint i = 0; i < numStates; i++)
                {
                    this.m_Coders[i].Init();
                }
            }

            /// <summary>
            /// The encoder 2.
            /// </summary>
            public struct Encoder2
            {
                /// <summary>
                /// The m_ encoders.
                /// </summary>
                private BitEncoder[] m_Encoders;

                /// <summary>
                /// The create.
                /// </summary>
                public void Create()
                {
                    this.m_Encoders = new BitEncoder[0x300];
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
                public void Encode(Encoder rangeEncoder, byte symbol)
                {
                    uint context = 1;
                    for (int i = 7; i >= 0; i--)
                    {
                        var bit = (uint)((symbol >> i) & 1);
                        this.m_Encoders[context].Encode(rangeEncoder, bit);
                        context = (context << 1) | bit;
                    }
                }

                /// <summary>
                /// The encode matched.
                /// </summary>
                /// <param name="rangeEncoder">
                /// The range encoder.
                /// </param>
                /// <param name="matchByte">
                /// The match byte.
                /// </param>
                /// <param name="symbol">
                /// The symbol.
                /// </param>
                public void EncodeMatched(Encoder rangeEncoder, byte matchByte, byte symbol)
                {
                    uint context = 1;
                    bool same = true;
                    for (int i = 7; i >= 0; i--)
                    {
                        var bit = (uint)((symbol >> i) & 1);
                        uint state = context;
                        if (same)
                        {
                            var matchBit = (uint)((matchByte >> i) & 1);
                            state += (1 + matchBit) << 8;
                            same = matchBit == bit;
                        }

                        this.m_Encoders[state].Encode(rangeEncoder, bit);
                        context = (context << 1) | bit;
                    }
                }

                /// <summary>
                /// The get price.
                /// </summary>
                /// <param name="matchMode">
                /// The match mode.
                /// </param>
                /// <param name="matchByte">
                /// The match byte.
                /// </param>
                /// <param name="symbol">
                /// The symbol.
                /// </param>
                /// <returns>
                /// The <see cref="uint"/>.
                /// </returns>
                public uint GetPrice(bool matchMode, byte matchByte, byte symbol)
                {
                    uint price = 0;
                    uint context = 1;
                    int i = 7;
                    if (matchMode)
                    {
                        for (; i >= 0; i--)
                        {
                            uint matchBit = (uint)(matchByte >> i) & 1;
                            uint bit = (uint)(symbol >> i) & 1;
                            price += this.m_Encoders[((1 + matchBit) << 8) + context].GetPrice(bit);
                            context = (context << 1) | bit;
                            if (matchBit != bit)
                            {
                                i--;
                                break;
                            }
                        }
                    }

                    for (; i >= 0; i--)
                    {
                        uint bit = (uint)(symbol >> i) & 1;
                        price += this.m_Encoders[context].GetPrice(bit);
                        context = (context << 1) | bit;
                    }

                    return price;
                }

                /// <summary>
                /// The init.
                /// </summary>
                public void Init()
                {
                    for (int i = 0; i < 0x300; i++)
                    {
                        this.m_Encoders[i].Init();
                    }
                }
            }
        }

        /// <summary>
        /// The optimal.
        /// </summary>
        private class Optimal
        {
            /// <summary>
            /// The back prev.
            /// </summary>
            public uint BackPrev;

            /// <summary>
            /// The back prev 2.
            /// </summary>
            public uint BackPrev2;

            /// <summary>
            /// The backs 0.
            /// </summary>
            public uint Backs0;

            /// <summary>
            /// The backs 1.
            /// </summary>
            public uint Backs1;

            /// <summary>
            /// The backs 2.
            /// </summary>
            public uint Backs2;

            /// <summary>
            /// The backs 3.
            /// </summary>
            public uint Backs3;

            /// <summary>
            /// The pos prev.
            /// </summary>
            public uint PosPrev;

            /// <summary>
            /// The pos prev 2.
            /// </summary>
            public uint PosPrev2;

            /// <summary>
            /// The prev 1 is char.
            /// </summary>
            public bool Prev1IsChar;

            /// <summary>
            /// The prev 2.
            /// </summary>
            public bool Prev2;

            /// <summary>
            /// The price.
            /// </summary>
            public uint Price;

            /// <summary>
            /// The state.
            /// </summary>
            public Base.State State;

            /// <summary>
            /// The is short rep.
            /// </summary>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            public bool IsShortRep()
            {
                return this.BackPrev == 0;
            }

            /// <summary>
            /// The make as char.
            /// </summary>
            public void MakeAsChar()
            {
                this.BackPrev = 0xFFFFFFFF;
                this.Prev1IsChar = false;
            }

            /// <summary>
            /// The make as short rep.
            /// </summary>
            public void MakeAsShortRep()
            {
                this.BackPrev = 0;
                
                this.Prev1IsChar = false;
            }
        };
    }
}