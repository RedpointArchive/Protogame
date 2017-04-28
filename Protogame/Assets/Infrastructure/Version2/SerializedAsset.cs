using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Protogame
{
    public class SerializedAsset : ISerializedAsset
    {
        private readonly Dictionary<string, byte[]> _values;

        public SerializedAsset()
        {
            _values = new Dictionary<string, byte[]>();
        }

        public byte[] GetByteArray(string property)
        {
            return _values[property];
        }

        public void SetByteArray(string property, byte[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _values[property] = value;
        }

        public static Task<SerializedAsset> FromStream(Stream stream)
        {
            var asset = new SerializedAsset();

            using (var reader = new BinaryReader(stream))
            {
                var propLength = reader.ReadInt32();
                var valueLength = reader.ReadInt32();
                var prop = Encoding.UTF8.GetString(reader.ReadBytes(propLength));
                var value = reader.ReadBytes(valueLength);

                asset._values[prop] = value;
            }

            return Task.FromResult(asset);
        }

        public async Task WriteTo(Stream stream)
        {
            var writer = new BinaryWriter(stream);
            foreach (var kv in _values)
            {
                var propNameBytes = Encoding.UTF8.GetBytes(kv.Key);
                writer.Write((Int32)propNameBytes.Length);
                writer.Write((Int32)kv.Value.Length);
                writer.Write(propNameBytes);
                writer.Write(kv.Value);
            }
        }
    }
}
