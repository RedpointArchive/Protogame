using ProtoBuf;

namespace Protogame
{
    [NetworkMessage("entity:prop"), ProtoContract]
    public class EntityPropertiesMessage : INetworkMessage
    {
        [ProtoMember(1)] public int EntityID;

        [ProtoMember(2)] public int FrameTick;

        [ProtoMember(3)] public string[] PropertyNames;

        [ProtoMember(4)] public int[] PropertyTypes;

        [ProtoMember(5)] public string[] PropertyValuesString;

        [ProtoMember(6)] public short[] PropertyValuesInt16;

        [ProtoMember(7)] public int[] PropertyValuesInt32;

        [ProtoMember(8)] public float[] PropertyValuesSingle;

        [ProtoMember(9)] public double[] PropertyValuesDouble;

        [ProtoMember(10)] public bool[] PropertyValuesBoolean;

        [ProtoMember(11)] public float[] PropertyValuesSingleArray;

        [ProtoMember(12)] public NetworkTransform[] PropertyValuesTransform;

        public const int PropertyTypeNone = 0;

        public const int PropertyTypeNull = 1;

        public const int PropertyTypeString = 2;

        public const int PropertyTypeInt16 = 3;

        public const int PropertyTypeInt32 = 4;

        public const int PropertyTypeSingle = 5;

        public const int PropertyTypeDouble = 6;

        public const int PropertyTypeBoolean = 7;

        public const int PropertyTypeVector2 = 8;

        public const int PropertyTypeVector3 = 9;

        public const int PropertyTypeVector4 = 10;

        public const int PropertyTypeQuaternion = 11;

        public const int PropertyTypeMatrix = 12;

        public const int PropertyTypeTransform = 13;
    }
}
