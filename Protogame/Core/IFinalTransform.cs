using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IFinalTransform
    {
        Matrix AbsoluteMatrix { get; }

        Matrix AbsoluteMatrixWithoutScale { get; }

        Vector3 AbsolutePosition { get; }

        Quaternion AbsoluteRotation { get; }

        IFinalTransform Parent { get; }

        IHasTransform ParentObject { get; }

        ITransform Child { get; }

        IHasTransform ChildObject { get; }
    }
}
