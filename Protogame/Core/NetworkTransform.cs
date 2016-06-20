using Microsoft.Xna.Framework;
using ProtoBuf;

namespace Protogame
{
    [ProtoContract]
    public class NetworkTransform
    {
        public ITransform DeserializeFromNetwork()
        {
            var transform = new DefaultTransform();
            DeserializeFromNetwork(transform);
            return transform;
        }

        public void DeserializeFromNetwork(ITransform target)
        {
            if (IsSRTMatrix)
            {
                var pos = SRTLocalPosition ?? new float[3];
                var rot = SRTLocalRotation ?? new float[4];
                var scale = SRTLocalScale ?? new float[3];

                target.SetFromSRTMatrix(
                    new Vector3(pos[0], pos[1], pos[2]),
                    new Quaternion(rot[0], rot[1], rot[2], rot[3]),
                    new Vector3(scale[0], scale[1], scale[2]));
            }
            else
            {
                var mat = CustomLocalMatrix ?? new float[16];
                
                target.SetFromCustomMatrix(new Matrix(
                    mat[0], mat[1], mat[2], mat[3],
                    mat[4], mat[5], mat[6], mat[7],
                    mat[8], mat[9], mat[10], mat[11],
                    mat[12], mat[13], mat[14], mat[15]));
            }
        }
        
        [ProtoMember(1)]
        public bool IsSRTMatrix;

        [ProtoMember(2)]
        public float[] SRTLocalPosition;

        [ProtoMember(3)]
        public float[] SRTLocalRotation;

        [ProtoMember(4)]
        public float[] SRTLocalScale;

        [ProtoMember(5)]
        public float[] CustomLocalMatrix;
    }
}
