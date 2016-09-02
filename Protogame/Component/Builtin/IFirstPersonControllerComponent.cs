using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IFirstPersonControllerComponent
    {
        float MovementSpeed { get; set; }

        void MoveInDirection(Vector3 direction);
    }
}
