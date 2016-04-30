using Microsoft.Xna.Framework;

namespace Protogame
{
    public class FirstPersonInputComponent : IEventfulComponent
    {
        private readonly FirstPersonCameraComponent _firstPersonCameraComponent;

        public FirstPersonInputComponent([FromParent] FirstPersonCameraComponent firstPersonCameraComponent)
        {
            _firstPersonCameraComponent = firstPersonCameraComponent;
            ThumbstickSensitivity = 1/100f;
        }

        public float ThumbstickSensitivity { get; set; }

        public bool Handle(ComponentizedEntity componentizedEntity, IGameContext gameContext, IEventEngine<IGameContext> eventEngine, Event @event)
        {
            var gamepadEvent = @event as GamePadEvent;

            if (gamepadEvent != null)
            {
                _firstPersonCameraComponent.Yaw -= gamepadEvent.GamePadState.ThumbSticks.Left.X * ThumbstickSensitivity;
                _firstPersonCameraComponent.Pitch += gamepadEvent.GamePadState.ThumbSticks.Left.Y * ThumbstickSensitivity;

                return true;
            }

            return false;
        }
    }
}
