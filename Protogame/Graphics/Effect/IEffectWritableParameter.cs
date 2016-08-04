using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface IEffectWritableParameter : IEffectParameter
    {
        void SetValue(Texture2D texture);

        void SetValue(Vector4 vector);

        void SetValue(Vector3 vector);

        void SetValue(Vector2 vector);

        void SetValue(float value);

        void SetValue(Matrix matrix);

        void SetValue(Matrix[] matrices);
    }
}