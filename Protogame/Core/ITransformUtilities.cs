using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public interface ITransformUtilities
    {
        ITransform CreateFromCustomMatrix(Matrix localMatrix);
        ITransform CreateFromDependencyInjection(IContext dependencyInjectionContext);
        ITransform CreateFromModifiedSRTFinalTransform(IFinalTransform existingFinalTransform, Vector3 scaleFactor, Quaternion appliedRotation, Vector3 addedPosition);
        ITransform CreateFromModifiedSRTTransform(ITransform existingTransform, Vector3 scaleFactor, Quaternion appliedRotation, Vector3 addedPosition);
        ITransform CreateFromSRTMatrix(Vector3 localScale, Quaternion localRotation, Vector3 localPosition);
        ITransform CreateLocalPosition(Vector3 localPosition);
        ITransform CreateLocalPosition(float x, float y, float z);
    }
}