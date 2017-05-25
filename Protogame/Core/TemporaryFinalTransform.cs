// ReSharper disable CheckNamespace
#pragma warning disable 1591

using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// An implementation of <see cref="IFinalTransform"/> which is used in temporary contexts,
    /// like network replay scenarios.
    /// </summary>
    /// <module>Core API</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IFinalTransform</interface_ref>
    public class TemporaryFinalTransform : IFinalTransform
    {
        private IHasTransform _parent;

        private IHasTransform _child;

        public TemporaryFinalTransform(IHasTransform parent, IHasTransform child)
        {
            _parent = parent;
            _child = child;
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

        public IFinalTransform Parent => _parent?.FinalTransform;

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