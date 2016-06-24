using Microsoft.Xna.Framework;

namespace Protogame
{
    public class DefaultFinalTransform : IFinalTransform
    {
        private IHasTransform _parent;

        private IHasTransform _child;

        private DefaultFinalTransform()
        {
        }

        public static IFinalTransform Create(IHasTransform parent, IHasTransform child)
        {
            var attachedTransform = new DefaultFinalTransform();
            attachedTransform._parent = parent;
            attachedTransform._child = child;
            return attachedTransform;
        }

        public static IFinalTransform Create(IHasTransform detachedChild)
        {
            var attachedTransform = new DefaultFinalTransform();
            attachedTransform._child = detachedChild;
            return attachedTransform;
        }

        public Matrix AbsoluteMatrix
        {
            get
            {
                if (_parent != null)
                {
                    return _parent.FinalTransform.AbsoluteMatrix*_child.Transform.LocalMatrix;
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
                    return _parent.FinalTransform.AbsoluteMatrixWithoutScale * _child.Transform.LocalMatrixWithoutScale;
                }

                return _child.Transform.LocalMatrixWithoutScale;
            }
        }

        public Vector3 AbsolutePosition
        {
            get { return Vector3.Transform(Vector3.Zero, AbsoluteMatrix); }
        }

        public Quaternion AbsoluteRotation
        {
            get { return AbsoluteMatrixWithoutScale.Rotation; }
        }

        public IFinalTransform Parent
        {
            get { return _parent.FinalTransform; }
        }

        public IHasTransform ParentObject
        {
            get { return _parent; }
        }

        public ITransform Child
        {
            get { return _child.Transform; }
        }

        public IHasTransform ChildObject
        {
            get { return _child; }
        }

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