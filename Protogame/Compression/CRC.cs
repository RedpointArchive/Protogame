// This is a copy of the 7-zip LZMA
// compression library.
namespace Protogame.Compression
{
    /// <summary>
    /// The crc.
    /// </summary>
    internal class CRC
    {
        /// <summary>
        /// The table.
        /// </summary>
        public static readonly uint[] Table;

        /// <summary>
        /// The _value.
        /// </summary>
        private uint _value = 0xFFFFFFFF;

        /// <summary>
        /// Initializes static members of the <see cref="CRC"/> class.
        /// </summary>
        static CRC()
        {
            Table = new uint[256];
            const uint kPoly = 0xEDB88320;
            for (uint i = 0; i < 256; i++)
            {
                uint r = i;
                for (int j = 0; j < 8; j++)
                {
                    if ((r & 1) != 0)
                    {
                        r = (r >> 1) ^ kPoly;
                    }
                    else
                    {
                        r >>= 1;
                    }
                }

                Table[i] = r;
            }
        }

        /// <summary>
        /// The get digest.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetDigest()
        {
            return this._value ^ 0xFFFFFFFF;
        }

        /// <summary>
        /// The init.
        /// </summary>
        public void Init()
        {
            this._value = 0xFFFFFFFF;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        public void Update(byte[] data, uint offset, uint size)
        {
            for (uint i = 0; i < size; i++)
            {
                this._value = Table[((byte)(this._value)) ^ data[offset + i]] ^ (this._value >> 8);
            }
        }

        /// <summary>
        /// The update byte.
        /// </summary>
        /// <param name="b">
        /// The b.
        /// </param>
        public void UpdateByte(byte b)
        {
            this._value = Table[((byte)(this._value)) ^ b] ^ (this._value >> 8);
        }

        /// <summary>
        /// The calculate digest.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        private static uint CalculateDigest(byte[] data, uint offset, uint size)
        {
            var crc = new CRC();

            // crc.Init();
            crc.Update(data, offset, size);
            return crc.GetDigest();
        }

        /// <summary>
        /// The verify digest.
        /// </summary>
        /// <param name="digest">
        /// The digest.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool VerifyDigest(uint digest, byte[] data, uint offset, uint size)
        {
            return CalculateDigest(data, offset, size) == digest;
        }
    }
}