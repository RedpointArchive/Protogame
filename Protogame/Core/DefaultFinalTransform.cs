// ReSharper disable CheckNamespace
#pragma warning disable 1591

using Microsoft.Xna.Framework;
using Protoinject;

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
        private IHasTransform _currentObject;

        private INode _currentNode;

        public DefaultFinalTransform(IHasTransform currentObject, INode currentNode)
        {
            _currentObject = currentObject;
            _currentNode = currentNode;
        }

        public Matrix AbsoluteMatrix
        {
            get
            {
                var parent = _currentNode?.Parent?.UntypedValue as IHasTransform;

                if (parent != null)
                {
                    return _currentObject.Transform.LocalMatrix * parent.FinalTransform.AbsoluteMatrix;
                }

                return _currentObject.Transform.LocalMatrix;
            }
        }

        public Matrix AbsoluteMatrixWithoutScale
        {
            get
            {
                var parent = _currentNode?.Parent?.UntypedValue as IHasTransform;

                if (parent != null)
                {
                    return _currentObject.Transform.LocalMatrixWithoutScale * parent.FinalTransform.AbsoluteMatrixWithoutScale;
                }

                return _currentObject.Transform.LocalMatrixWithoutScale;
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

        public IFinalTransform Parent => (_currentNode?.Parent?.UntypedValue as IHasTransform)?.FinalTransform;

        public IHasTransform ParentObject => _currentNode?.Parent?.UntypedValue as IHasTransform;

        public ITransform Child => _currentObject.Transform;

        public IHasTransform ChildObject => _currentObject;

        public override string ToString()
        {
            if (_currentObject.Transform.IsSRTMatrix)
            {
                var parent = _currentNode?.Parent?.UntypedValue as IHasTransform;

                if ((parent != null && parent.Transform.IsSRTMatrix) || parent == null)
                {
                    return "AT SRT P: " + AbsolutePosition + " R: " + AbsoluteRotation + " M: " + AbsoluteMatrix;
                }
            }

            return "AT CUS M: " + AbsoluteMatrix;
        }
    }
}