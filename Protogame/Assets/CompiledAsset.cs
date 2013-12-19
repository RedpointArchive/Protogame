using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Protogame
{
    [ProtoContract]
    public class CompiledAsset
    {
        [ProtoMember(1)]
        public string Loader
        {
            get;
            set;
        }

        [ProtoMember(3)]
        public PlatformData PlatformData
        {
            get;
            set;
        }
    }
}
