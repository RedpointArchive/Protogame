// ReSharper disable CheckNamespace

using ProtoBuf;

namespace Protogame
{
    [NetworkMessage("entity:input:predict"), ProtoContract]
    public class InputPredictMessage : IEntityPredictMessage
    {
        [ProtoMember(1)]
        public int EntityID { get; set; }

        [ProtoMember(2)]
        public int PredictionID { get; set; }

        [ProtoMember(3)]
        public float MovementDirX;

        [ProtoMember(4)]
        public float MovementDirY;

        [ProtoMember(5)]
        public float MovementDirZ;
    }
}
