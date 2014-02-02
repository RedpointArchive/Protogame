// This is an LZMA helper class based on the code available
// at http://www.nullskull.com/a/768/7zip-lzma-inmemory-compression-with-c.aspx.
namespace Protogame.Compression
{
    using System;
    using System.IO;

    /// <summary>
    /// The lzma helper.
    /// </summary>
    public static class LzmaHelper
    {
        /// <summary>
        /// The prop i ds.
        /// </summary>
        private static readonly CoderPropID[] propIDs =
        {
            CoderPropID.DictionarySize, CoderPropID.PosStateBits, 
            CoderPropID.LitContextBits, CoderPropID.LitPosBits, CoderPropID.Algorithm, CoderPropID.NumFastBytes, 
            CoderPropID.MatchFinder, CoderPropID.EndMarker
        };

        /// <summary>
        /// The properties.
        /// </summary>
        private static readonly object[] properties = { dictionary, 2, 3, 0, 2, 128, "bt4", eos };

        /// <summary>
        /// The dictionary.
        /// </summary>
        private static int dictionary = 1 << 23;

        /// <summary>
        /// The eos.
        /// </summary>
        private static bool eos = false;

        /// <summary>
        /// The compress.
        /// </summary>
        /// <param name="inStream">
        /// The in stream.
        /// </param>
        /// <param name="outStream">
        /// The out stream.
        /// </param>
        public static void Compress(Stream inStream, Stream outStream)
        {
            var encoder = new LzmaEncoder();
            encoder.SetCoderProperties(propIDs, properties);
            encoder.WriteCoderProperties(outStream);
            long fileSize = inStream.Length;
            for (int i = 0; i < 8; i++)
            {
                outStream.WriteByte((Byte)(fileSize >> (8 * i)));
            }

            encoder.Code(inStream, outStream, -1, -1, null);
        }

        /// <summary>
        /// The decompress.
        /// </summary>
        /// <param name="newInStream">
        /// The new in stream.
        /// </param>
        /// <param name="newOutStream">
        /// The new out stream.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        public static void Decompress(Stream newInStream, Stream newOutStream)
        {
            var decoder = new LzmaDecoder();
            var properties2 = new byte[5];
            if (newInStream.Read(properties2, 0, 5) != 5)
            {
                throw new Exception("input .lzma is too short");
            }

            long outSize = 0;
            for (int i = 0; i < 8; i++)
            {
                int v = newInStream.ReadByte();
                if (v < 0)
                {
                    throw new Exception("Can't Read 1");
                }

                outSize |= ((long)(byte)v) << (8 * i);
            }

            decoder.SetDecoderProperties(properties2);
            long compressedSize = newInStream.Length - newInStream.Position;
            decoder.Code(newInStream, newOutStream, compressedSize, outSize, null);
        }
    }
}