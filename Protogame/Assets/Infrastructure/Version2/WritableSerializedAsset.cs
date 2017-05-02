using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protogame
{
    public class WritableSerializedAsset : IWritableSerializedAsset
    {
        private readonly Dictionary<string, byte[]> _values;
        private readonly List<string> _dependencies;

        public WritableSerializedAsset()
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

        public void WriteTo(Stream stream)
        {
            var writer = new BinaryWriter(stream);

            writer.Write((Int32)_dependencies.Count);
            foreach (var dep in _dependencies)
            {
                writer.Write(dep);
            }

            var orderedKeys = _values.ToArray();

            var indexLength = 0;
            using (var simulatedIndex = new MemoryStream())
            {
                var simulatedWriter = new BinaryWriter(simulatedIndex);
                simulatedWriter.Write((Int32)_values.Count);
                foreach (var kv in orderedKeys)
                {
                    var propNameBytes = Encoding.UTF8.GetBytes(kv.Key);
                    simulatedWriter.Write((Int32)propNameBytes.Length);
                    simulatedWriter.Write(propNameBytes);
                    simulatedWriter.Write((Int32)0);
                    simulatedWriter.Write((Int32)kv.Value.Length);
                }
                simulatedWriter.Flush();
                indexLength = (int)simulatedIndex.Position;
            }

            var accumulatedOffset = stream.Position;
            writer.Write((Int32)_values.Count);
            foreach (var kv in orderedKeys)
            {
                var propNameBytes = Encoding.UTF8.GetBytes(kv.Key);
                writer.Write((Int32)propNameBytes.Length);
                writer.Write(propNameBytes);
                writer.Write((Int32)(indexLength + accumulatedOffset));
                writer.Write((Int32)kv.Value.Length);
                accumulatedOffset += kv.Value.Length;
            }

            foreach (var kv in orderedKeys)
            {
                writer.Write(kv.Value);
            }
        }
    }
}
