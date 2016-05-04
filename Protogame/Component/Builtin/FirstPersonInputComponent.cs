using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class FirstPersonInputComponent : IEventfulComponent
    {
        private readonly FirstPersonCameraComponent _firstPersonCameraComponent;

        public FirstPersonInputComponent([FromParent] FirstPersonCameraComponent firstPersonCameraComponent)
        {
            _firstPersonCameraComponent = firstPersonCameraComponent;
            ThumbstickLookSensitivity = 1/100f;
            ThumbstickMoveSensitivity = 1/20f;
        }

        public float ThumbstickLookSensitivity { get; set; }

        public float ThumbstickMoveSensitivity { get; set; }

        public bool Handle(ComponentizedEntity componentizedEntity, IGameContext gameContext, IEventEngine<IGameContext> eventEngine, Event @event)
        {
            var gamepadEvent = @event as GamePadEvent;
            var keyHeldEvent = @event as KeyHeldEvent;

            if (gamepadEvent != null)
            {
                _firstPersonCameraComponent.Yaw -= gamepadEvent.GamePadState.ThumbSticks.Right.X * ThumbstickLookSensitivity;
                _firstPersonCameraComponent.Pitch += gamepadEvent.GamePadState.ThumbSticks.Right.Y * ThumbstickLookSensitivity;

                var limit = MathHelper.PiOver2 - MathHelper.ToRadians(5);
                _firstPersonCameraComponent.Pitch = MathHelper.Clamp(_firstPersonCameraComponent.Pitch, -limit, limit);

                var lookAt = _firstPersonCameraComponent.LocalMatrix;
                var relativeMovementVector = new Vector3(
                    gamepadEvent.GamePadState.ThumbSticks.Left.X * ThumbstickMoveSensitivity,
                    0f,
                    -gamepadEvent.GamePadState.ThumbSticks.Left.Y * ThumbstickMoveSensitivity);
                var absoluteMovementVector = Vector3.Transform(relativeMovementVector, lookAt);

                componentizedEntity.LocalMatrix *= Matrix.CreateTranslation(absoluteMovementVector);

                return true;
            }

            if (keyHeldEvent != null)
            {
                var didConsume = false;
                if (keyHeldEvent.Key == Keys.A)
                {
                    _firstPersonCameraComponent.Yaw -= -1 * ThumbstickLookSensitivity;
                    didConsume = true;
                }
                if (keyHeldEvent.Key == Keys.D)
                {
                    _firstPersonCameraComponent.Yaw -= 1 * ThumbstickLookSensitivity;
                    didConsume = true;
                }
                if (keyHeldEvent.Key == Keys.W)
                {
                    _firstPersonCameraComponent.Pitch += -1 * ThumbstickLookSensitivity;
                    didConsume = true;
                }
                if (keyHeldEvent.Key == Keys.S)
                {
                    _firstPersonCameraComponent.Pitch += 1 * ThumbstickLookSensitivity;
                    didConsume = true;
                }

                var limit = MathHelper.PiOver2 - MathHelper.ToRadians(5);
                _firstPersonCameraComponent.Pitch = MathHelper.Clamp(_firstPersonCameraComponent.Pitch, -limit, limit);

                var moveX = 0;
                var moveZ = 0;

                if (keyHeldEvent.Key == Keys.Left)
                {
                    moveX = -1;
                    didConsume = true;
                }
                if (keyHeldEvent.Key == Keys.Right)
                {
                    moveX = 1;
                    didConsume = true;
                }
                if (keyHeldEvent.Key == Keys.Up)
                {
                    moveZ = 1;
                    didConsume = true;
                }
                if (keyHeldEvent.Key == Keys.Down)
                {
                    moveZ = -1;
                    didConsume = true;
                }
                var lookAt = _firstPersonCameraComponent.LocalMatrix;
                var relativeMovementVector = new Vector3(
                    moveX * ThumbstickMoveSensitivity,
                    0f,
                    -moveZ * ThumbstickMoveSensitivity);
                var absoluteMovementVector = Vector3.Transform(relativeMovementVector, lookAt);

                componentizedEntity.LocalMatrix *= Matrix.CreateTranslation(absoluteMovementVector);

                return didConsume;
            }

            return false;
        }
    }
}
