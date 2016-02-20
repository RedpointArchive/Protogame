using ProtoBuf;

namespace Protogame
{
    public class NetworkTagAttribute : ProtoMemberAttribute
    {
        public NetworkTagAttribute(int tag) : base(tag)
        {
        }
    }
}