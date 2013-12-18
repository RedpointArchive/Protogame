using ProtoBuf;

namespace Protogame
{
    [ProtoContract]
    public class PlatformData
    {
        [ProtoMember(1)]
        public TargetPlatform Platform
        {
            get;
            set;
        }

        [ProtoMember(2)]
        public byte[] Data
        {
            get;
            set;
        }
    }
}
