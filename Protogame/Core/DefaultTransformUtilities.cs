// ReSharper disable CheckNamespace
#pragma warning disable 1591

using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="ITransformUtilities"/>.
    /// </summary>
    /// <module>Core API</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.ITransformUtilities</interface_ref>
    public class DefaultTransformUtilities : ITransformUtilities
    {
        public ITransform CreateFromSRTMatrix(Vector3 localScale, Quaternion localRotation, Vector3 localPosition)
        {
            var transform = new DefaultTransform();
            transform.LocalPosition = localPosition;
            transform.LocalRotation = localRotation;
            transform.LocalScale = localScale;
            return transform;
        }

        public ITransform CreateFromModifiedSRTTransform(ITransform existingTransform, Vector3 scaleFactor, Quaternion appliedRotation, Vector3 addedPosition)
        {
            var transform = new DefaultTransform();
            transform.LocalPosition = existingTransform.LocalPosition + addedPosition;
            transform.LocalRotation = existingTransform.LocalRotation * appliedRotation;
            transform.LocalScale = existingTransform.LocalScale * scaleFactor;
            return transform;
        }

        public ITransform CreateFromModifiedSRTFinalTransform(IFinalTransform existingFinalTransform, Vector3 scaleFactor, Quaternion appliedRotation, Vector3 addedPosition)
        {
            var transform = new DefaultTransform();
            //transform.LocalPosition = existingFinalTransform.AbsolutePosition + addedPosition;
            //transform.LocalRotation = existingFinalTransform.AbsoluteRotation * appliedRotation;
            //transform.LocalScale = existingFinalTransform.AbsoluteScale * scaleFactor;            
            return transform;
        }

        public ITransform CreateFromCustomMatrix(Matrix localMatrix)
        {
            var transform = new DefaultTransform();
            transform.SetFromCustomMatrix(localMatrix);
            return transform;
        }

        public ITransform CreateFromDependencyInjection(IContext dependencyInjectionContext)
        {
            return new DefaultTransform();
        }

        public ITransform CreateLocalPosition(float x, float y, float z)
        {
            var transform = new DefaultTransform();
            transform.LocalPosition = new Vector3(x, y, z);
            return transform;
        }

        public ITransform CreateLocalPosition(Vector3 localPosition)
        {
            var transform = new DefaultTransform();
            transform.LocalPosition = localPosition;
            return transform;
        }
    }
}
