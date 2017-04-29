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
        private readonly List<string> _dependencies;

        public SerializedAsset()
        {
            _values = new Dictionary<string, byte[]>();
            _dependencies = new List<string>();
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

        public void AddCompilationDependency(string assetName)
        {
            _dependencies.Add(assetName);
        }

        public IReadOnlyCollection<string> Dependencies => _dependencies.AsReadOnly();

        public static Task<SerializedAsset> FromStream(Stream stream, bool dependenciesOnly)
        {
            var asset = new SerializedAsset();

            using (var reader = new BinaryReader(stream))
            {
                var dependencyCount = reader.ReadInt32();
                for (var i = 0; i < dependencyCount; i++)
                {
                    asset._dependencies.Add(reader.ReadString());
                }

                if (!dependenciesOnly)
                {
                    var propCount = reader.ReadInt32();
                    for (var i = 0; i < propCount; i++)
                    {
                        var propLength = reader.ReadInt32();
                        var valueLength = reader.ReadInt32();
                        var prop = Encoding.UTF8.GetString(reader.ReadBytes(propLength));
                        var value = reader.ReadBytes(valueLength);

                        asset._values[prop] = value;
                    }
                }
            }

            return Task.FromResult(asset);
        }

        public async Task WriteTo(Stream stream)
        {
            var writer = new BinaryWriter(stream);

            writer.Write((Int32)_dependencies.Count);
            foreach (var dep in _dependencies)
            {
                writer.Write(dep);
            }

            writer.Write((Int32)_values.Count);
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
