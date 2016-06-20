using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf.Meta;

namespace Protogame
{
    public class AheadOfTimeNetworkMessageSerialization : INetworkMessageSerialization
    {
        private Dictionary<string, Type> _stringToType;

        private Dictionary<Type, string> _typeToString;

        private Dictionary<Type, TypeModel> _typeToSerializer;

        public AheadOfTimeNetworkMessageSerialization(INetworkMessage[] networkMessages)
        {
            var mapped = networkMessages.Select(x => x.GetType())
                .Select(x => new
                {
                    Type = x,
                    Attribute =
                        x.GetCustomAttributes(typeof (NetworkMessageAttribute), false).FirstOrDefault() as
                            NetworkMessageAttribute
                }).Where(x => x.Attribute != null).ToList();
            _stringToType = mapped.ToDictionary(k => k.Attribute.MessageType, v => v.Type);
            _typeToString = mapped.ToDictionary(k => k.Type, v => v.Attribute.MessageType);

            var assembliesToSerializers = networkMessages.Select(x => x.GetType().Assembly).Distinct()
                .ToDictionary(
                    k => k,
                    v => (TypeModel) Activator.CreateInstance(
                        v.GetType("_NetworkSerializers.<>GeneratedSerializer")));
            _typeToSerializer = networkMessages.Select(x => x.GetType())
                .ToDictionary(
                    k => k,
                    v => assembliesToSerializers[v.Assembly]);
        }

        public byte[] Serialize<T>(T message)
        {
            var serialized = InternalSerialize(message);
            var type = _typeToString[typeof(T)];

            var bytes = new byte[1 + type.Length + serialized.Length];
            bytes[0] = (byte)type.Length;
            Encoding.ASCII.GetBytes(type).CopyTo(bytes, 1);
            serialized.CopyTo(bytes, 1 + type.Length);

            return bytes;
        }

        public byte[] Serialize(object message)
        {
            var serialized = InternalSerialize(message);
            var type = _typeToString[message.GetType()];

            var bytes = new byte[1 + type.Length + serialized.Length];
            bytes[0] = (byte)type.Length;
            Encoding.ASCII.GetBytes(type).CopyTo(bytes, 1);
            serialized.CopyTo(bytes, 1 + type.Length);

            return bytes;
        }

        public object Deserialize(byte[] message, out Type type)
        {
            var len = message[0];
            var tlen = message.Length;
            var typeBytes = new byte[len];
            Array.Copy(message, 1, typeBytes, 0, len);
            var dataBytes = new byte[tlen - 1 - len];
            Array.Copy(message, 1 + len, dataBytes, 0, tlen - 1 - len);
            var typeString = Encoding.ASCII.GetString(typeBytes);
            type = _stringToType[typeString];
            return InternalDeserialize(dataBytes, type);
        }

        public object Deserialize(byte[] message)
        {
            Type discarded;
            return Deserialize(message, out discarded);
        }

        private byte[] InternalSerialize<T>(T message)
        {
            using (var memory = new MemoryStream())
            {
                InternalSerialize(message, memory);
                var b = new byte[memory.Position];
                memory.Seek(0, SeekOrigin.Begin);
                memory.Read(b, 0, b.Length);
                return b;
            }
        }

        private void InternalSerialize<T>(T message, Stream stream)
        {
            if (!_typeToSerializer.ContainsKey(message.GetType()))
            {
                throw new InvalidOperationException(
                    "Type is not registered for network serialization '" + message.GetType().FullName + "'. " +
                    "Did you forget to bind it in a dependency injection module?");
            }

            _typeToSerializer[message.GetType()].Serialize(stream, message);
        }

        private object InternalDeserialize(Stream message, Type type)
        {
            if (!_typeToSerializer.ContainsKey(type))
            {
                throw new InvalidOperationException(
                    "Type is not registered for network serialization '" + type.FullName + "'. " +
                    "Did you forget to bind it in a dependency injection module?");
            }
            
            var obj = Activator.CreateInstance(type);
            var result = _typeToSerializer[type].Deserialize(message, obj, type);
            return result;
        }

        private object InternalDeserialize(byte[] message, Type type)
        {
            using (var memory = new MemoryStream(message))
            {
                return InternalDeserialize(memory, type);
            }
        }
    }
}