using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Protogame
{
    public class ReadableSerializedAsset : IReadableSerializedAsset
    {
        private readonly Stream _stream;
        private readonly long _readCacheOffset;
        private readonly MemoryStream _readDataCacheForUnseekableStreams;
        private readonly Dictionary<string, Tuple<int, int>> _index;
        private readonly List<string> _dependencies;

        private ReadableSerializedAsset(Stream stream, Dictionary<string, Tuple<int, int>> index, List<string> dependencies)
        {
            _stream = stream;
            _readCacheOffset = _stream?.Position ?? 0;
            _readDataCacheForUnseekableStreams = new MemoryStream();
            _index = index;
            _dependencies = dependencies;
        }

        public void Dispose()
        {
            _stream?.Dispose();
            _readDataCacheForUnseekableStreams?.Dispose();
        }

        public IReadOnlyCollection<string> Dependencies => _dependencies.AsReadOnly();

        public static IReadableSerializedAsset FromStream(Stream stream, bool dependenciesOnly)
        {
            // If we only need the dependencies, just read them straight from the stream,
            // with no data caching.
            if (dependenciesOnly)
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    var dependencyCount = reader.ReadInt32();
                    var dependencies = new List<string>();
                    for (var i = 0; i < dependencyCount; i++)
                    {
                        dependencies.Add(reader.ReadString());
                    }

                    return new ReadableSerializedAsset(null, null, dependencies);
                }
            }
            
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var dependencyCount = reader.ReadInt32();
                var dependencies = new List<string>();
                for (var i = 0; i < dependencyCount; i++)
                {
                    dependencies.Add(reader.ReadString());
                }

                if (!dependenciesOnly)
                {
                    var index = new Dictionary<string, Tuple<int, int>>();
                    var indexCount = reader.ReadInt32();
                    for (var i = 0; i < indexCount; i++)
                    {
                        var propLength = reader.ReadInt32();
                        var prop = Encoding.UTF8.GetString(reader.ReadBytes(propLength));
                        var offset = reader.ReadInt32();
                        var length = reader.ReadInt32();

                        index[prop] = new Tuple<int, int>(offset, length);
                    }

                    return new ReadableSerializedAsset(stream, index, dependencies);
                }
                else
                {
                    // Dispose stream now, since disposing the serialized asset will not do it.
                    stream.Dispose();

                    return new ReadableSerializedAsset(null, null, dependencies);
                }
            }
        }

        public byte[] GetByteArray(string property)
        {
            if (_stream == null)
            {
                throw new InvalidOperationException("Attempted to read property from serialized asset that only has dependency information loaded!");
            }

            var t = _index[property];
            var offset = t.Item1;
            var length = t.Item2;
            if (_stream.CanSeek)
            {
                _stream.Seek(offset, SeekOrigin.Begin);
                var res = new byte[length];
                _stream.Read(res, 0, res.Length);
                return res;
            }
            else
            {
                if (_stream.Position < offset + length)
                {
                    // Read data into the read cache since this stream is unseekable.
                    var b = new byte[(offset + length) - _stream.Position];
                    _readDataCacheForUnseekableStreams.Seek(_stream.Position, SeekOrigin.Begin);
                    _stream.Read(b, 0, b.Length);
                    _readDataCacheForUnseekableStreams.Write(b, 0, b.Length);
                }

                _readDataCacheForUnseekableStreams.Seek(offset, SeekOrigin.Begin);
                var res = new byte[length];
                _readDataCacheForUnseekableStreams.Read(res, 0, res.Length);
                return res;
            }
        }
    }
}
