using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface ITransform
    {
        bool IsSRTMatrix { get; }
        Matrix LocalMatrix { get; }
        Matrix LocalMatrixWithoutScale { get; }
        Vector3 LocalPosition { get; set; }
        Quaternion LocalRotation { get; set; }
        Vector3 LocalScale { get; set; }

        void ResetAsCustomMatrix();
        void ResetAsSRTMatrix();
        void SetFromCustomMatrix(Matrix localMatrix);
        void SetFromSRTMatrix(Vector3 localPosition, Quaternion localRotation, Vector3 localScale);

        NetworkTransform SerializeToNetwork();

        void Assign(ITransform from);
    }
}