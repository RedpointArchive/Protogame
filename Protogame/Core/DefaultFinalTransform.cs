// ReSharper disable CheckNamespace
#pragma warning disable 1591

using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="IFinalTransform"/>.
    /// </summary>
    /// <module>Core API</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IFinalTransform</interface_ref>
    public class DefaultFinalTransform : IFinalTransform
    {
        private IHasTransform _parent;

        private IHasTransform _child;

        private DefaultFinalTransform()
        {
        }

        public static IFinalTransform Create(IHasTransform parent, IHasTransform child)
        {
            var attachedTransform = new DefaultFinalTransform
            {
                _parent = parent,
                _child = child
            };
            return attachedTransform;
        }

        public static IFinalTransform Create(IHasTransform detachedChild)
        {
            var attachedTransform = new DefaultFinalTransform
            {
                _child = detachedChild
            };
            return attachedTransform;
        }

        public Matrix AbsoluteMatrix
        {
            get
            {
                if (_parent != null)
                {
                    return _child.Transform.LocalMatrix * _parent.FinalTransform.AbsoluteMatrix;
                }

                return _child.Transform.LocalMatrix;
            }
        }

        public Matrix AbsoluteMatrixWithoutScale
        {
            get
            {
                if (_parent != null)
                {
                    return _child.Transform.LocalMatrixWithoutScale * _parent.FinalTransform.AbsoluteMatrixWithoutScale;
                }

                return _child.Transform.LocalMatrixWithoutScale;
            }
        }

        public Vector3 AbsolutePosition => Vector3.Transform(Vector3.Zero, AbsoluteMatrix);

        public Quaternion AbsoluteRotation
        {
            get
            {
                Vector3 scale, translation;
                Quaternion rotation;
                if (AbsoluteMatrixWithoutScale.Decompose(out scale, out rotation, out translation))
                {
                    return rotation;
                }

                return Quaternion.Identity;
            }
        }

        public IFinalTransform Parent => _parent.FinalTransform;

        public IHasTransform ParentObject => _parent;

        public ITransform Child => _child.Transform;

        public IHasTransform ChildObject => _child;

        public override string ToString()
        {
            if (_child.Transform.IsSRTMatrix)
            {
                if ((_parent != null && _parent.Transform.IsSRTMatrix) || _parent == null)
                {
                    return "AT SRT P: " + AbsolutePosition + " R: " + AbsoluteRotation + " M: " + AbsoluteMatrix;
                }
            }

            return "AT CUS M: " + AbsoluteMatrix;
        }
    }
}