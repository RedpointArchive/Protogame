// This is an LZMA helper class based on the code available
// at http://www.nullskull.com/a/768/7zip-lzma-inmemory-compression-with-c.aspx.

using System;
using System.IO;

namespace Protogame.Compression
{
    public static class LzmaHelper
    {
        static int dictionary = 1 << 23;
        static bool eos = false;

        static CoderPropID[] propIDs = 
		{
			CoderPropID.DictionarySize,
			CoderPropID.PosStateBits,
			CoderPropID.LitContextBits,
			CoderPropID.LitPosBits,
			CoderPropID.Algorithm,
			CoderPropID.NumFastBytes,
			CoderPropID.MatchFinder,
			CoderPropID.EndMarker
		};

        static object[] properties = 
		{
			(Int32)(dictionary),
			(Int32)(2),
			(Int32)(3),
			(Int32)(0),
			(Int32)(2),
			(Int32)(128),
			"bt4",
			eos
		};

        private static LzmaEncoder m_Encoder;

        private static LzmaDecoder m_Decoder;

        private static byte[] m_DecoderProps;

        private static object m_EncoderLock = new object();

        private static object m_DecoderLock = new object();

        public static void Compress(Stream inStream, Stream outStream)
        {
            lock (m_EncoderLock)
            {
                if (m_Encoder == null)
                {
                    m_Encoder = new LzmaEncoder();
                    m_Encoder.SetCoderProperties(propIDs, properties);
                }

                m_Encoder.WriteCoderProperties(outStream);
                long fileSize = inStream.Length;
                for (int i = 0; i < 8; i++) outStream.WriteByte((Byte)(fileSize >> (8 * i)));
                m_Encoder.Code(inStream, outStream, -1, -1, null);
            }
        }

        public static void Decompress(Stream newInStream, Stream newOutStream)
        {
            lock (m_DecoderLock)
            {
                if (m_Decoder == null)
                {
                    m_Decoder = new LzmaDecoder();
                }

                if (m_DecoderProps == null)
                {
                    m_DecoderProps = new byte[5];
                }
                if (newInStream.Read(m_DecoderProps, 0, 5) != 5) throw (new Exception("input .lzma is too short"));
                long outSize = 0;
                for (int i = 0; i < 8; i++)
                {
                    int v = newInStream.ReadByte();
                    if (v < 0) throw (new Exception("Can't Read 1"));
                    outSize |= ((long)(byte)v) << (8 * i);
                }

                m_Decoder.SetDecoderProperties(m_DecoderProps);
                long compressedSize = newInStream.Length - newInStream.Position;
                m_Decoder.Code(newInStream, newOutStream, compressedSize, outSize, null);
            }
        }
    }
}
