// This is a copy of the 7-zip LZMA
// compression library.
namespace Protogame.Compression
{
    using System;
    using System.IO;

    /// <summary>
    /// The exception that is thrown when an error in input stream occurs during decoding.
    /// </summary>
    internal class DataErrorException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataErrorException"/> class.
        /// </summary>
        public DataErrorException()
            : base("Data Error")
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when the value of an argument is outside the allowable range.
    /// </summary>
    internal class InvalidParamException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidParamException"/> class.
        /// </summary>
        public InvalidParamException()
            : base("Invalid Parameter")
        {
        }
    }

    /// <summary>
    /// The CodeProgress interface.
    /// </summary>
    public interface ICodeProgress
    {
        /// <summary>
        /// Callback progress.
        /// </summary>
        /// <param name="inSize">
        /// Input size. -1 if unknown.
        /// </param>
        /// <param name="outSize">
        /// Output size. -1 if unknown.
        /// </param>
        void SetProgress(long inSize, long outSize);
    };

    /// <summary>
    /// The Coder interface.
    /// </summary>
    public interface ICoder
    {
        /// <summary>
        /// Codes streams.
        /// </summary>
        /// <param name="inStream">
        /// Input Stream.
        /// </param>
        /// <param name="outStream">
        /// Output Stream.
        /// </param>
        /// <param name="inSize">
        /// Input Size. -1 if unknown.
        /// </param>
        /// <param name="outSize">
        /// Output Size. -1 if unknown.
        /// </param>
        /// <param name="progress">
        /// Callback progress reference.
        /// </param>
        /// <exception cref="SevenZip.DataErrorException">
        /// If input stream is not valid.
        /// </exception>
        void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress);
    };

    /*
	public interface ICoder2
	{
		 void Code(ISequentialInStream []inStreams,
				const UInt64 []inSizes, 
				ISequentialOutStream []outStreams, 
				UInt64 []outSizes,
				ICodeProgress progress);
	};
  */

    /// <summary>
    /// Provides the fields that represent properties idenitifiers for compressing.
    /// </summary>
    public enum CoderPropID
    {
        /// <summary>
        /// Specifies default property.
        /// </summary>
        DefaultProp = 0, 

        /// <summary>
        /// Specifies size of dictionary.
        /// </summary>
        DictionarySize, 

        /// <summary>
        /// Specifies size of memory for PPM*.
        /// </summary>
        UsedMemorySize, 

        /// <summary>
        /// Specifies order for PPM methods.
        /// </summary>
        Order, 

        /// <summary>
        /// Specifies Block Size.
        /// </summary>
        BlockSize, 

        /// <summary>
        /// The pos state bits.
        /// </summary>
        PosStateBits, 

        /// <summary>
        /// The lit context bits.
        /// </summary>
        LitContextBits, 

        /// <summary>
        /// The lit pos bits.
        /// </summary>
        LitPosBits, 

        /// <summary>
        /// Specifies number of fast bytes for LZ*.
        /// </summary>
        NumFastBytes, 

        /// <summary>
        /// Specifies match finder. LZMA: "BT2", "BT4" or "BT4B".
        /// </summary>
        MatchFinder, 

        /// <summary>
        /// Specifies the number of match finder cyckes.
        /// </summary>
        MatchFinderCycles, 

        /// <summary>
        /// Specifies number of passes.
        /// </summary>
        NumPasses, 

        /// <summary>
        /// Specifies number of algorithm.
        /// </summary>
        Algorithm, 

        /// <summary>
        /// Specifies the number of threads.
        /// </summary>
        NumThreads, 

        /// <summary>
        /// Specifies mode with end marker.
        /// </summary>
        EndMarker
    };

    /// <summary>
    /// The SetCoderProperties interface.
    /// </summary>
    public interface ISetCoderProperties
    {
        /// <summary>
        /// The set coder properties.
        /// </summary>
        /// <param name="propIDs">
        /// The prop i ds.
        /// </param>
        /// <param name="properties">
        /// The properties.
        /// </param>
        void SetCoderProperties(CoderPropID[] propIDs, object[] properties);
    };

    /// <summary>
    /// The WriteCoderProperties interface.
    /// </summary>
    public interface IWriteCoderProperties
    {
        /// <summary>
        /// The write coder properties.
        /// </summary>
        /// <param name="outStream">
        /// The out stream.
        /// </param>
        void WriteCoderProperties(Stream outStream);
    }

    /// <summary>
    /// The SetDecoderProperties interface.
    /// </summary>
    public interface ISetDecoderProperties
    {
        /// <summary>
        /// The set decoder properties.
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        void SetDecoderProperties(byte[] properties);
    }
}