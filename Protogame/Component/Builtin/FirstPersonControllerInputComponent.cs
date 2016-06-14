using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class FirstPersonControllerInputComponent : IEventfulComponent
    {
        private readonly FirstPersonCameraComponent _firstPersonCameraComponent;
        private readonly FirstPersonControllerPhysicsComponent _firstPersonControllerPhysicsComponent;

        public FirstPersonControllerInputComponent(
            [FromParent] FirstPersonCameraComponent firstPersonCameraComponent,
            [FromParent] FirstPersonControllerPhysicsComponent firstPersonControllerPhysicsComponent)
        {
            _firstPersonCameraComponent = firstPersonCameraComponent;
            _firstPersonControllerPhysicsComponent = firstPersonControllerPhysicsComponent;

            ThumbstickLookSensitivity = 1/100f;
            ThumbstickMoveSensitivity = 1/20f;
        }

        public float ThumbstickLookSensitivity { get; set; }

        public float ThumbstickMoveSensitivity { get; set; }

        public bool Handle(ComponentizedEntity componentizedEntity, IGameContext gameContext,
            IEventEngine<IGameContext> eventEngine, Event @event)
        {
            var gamepadEvent = @event as GamePadEvent;
            var keyHeldEvent = @event as KeyHeldEvent;
            var mouseEvent = @event as MouseEvent;

            if (gamepadEvent != null)
            {
                _firstPersonCameraComponent.Yaw -= gamepadEvent.GamePadState.ThumbSticks.Right.X*
                                                   ThumbstickLookSensitivity;
                _firstPersonCameraComponent.Pitch += gamepadEvent.GamePadState.ThumbSticks.Right.Y*
                                                     ThumbstickLookSensitivity;

                var limit = MathHelper.PiOver2 - MathHelper.ToRadians(5);
                _firstPersonCameraComponent.Pitch = MathHelper.Clamp(_firstPersonCameraComponent.Pitch, -limit, limit);

                var lookAt = _firstPersonCameraComponent.Transform.LocalMatrix;
                var relativeMovementVector = new Vector3(
                    gamepadEvent.GamePadState.ThumbSticks.Left.X*ThumbstickMoveSensitivity,
                    0f,
                    -gamepadEvent.GamePadState.ThumbSticks.Left.Y*ThumbstickMoveSensitivity);

                var absoluteMovementVector = Vector3.Transform(relativeMovementVector, lookAt);

                _firstPersonControllerPhysicsComponent.TargetVelocity = absoluteMovementVector;

                return true;
            }

            if (mouseEvent != null)
            {
                var centerX = gameContext.Window.ClientBounds.Width/2;
                var centerY = gameContext.Window.ClientBounds.Height/2;

                _firstPersonCameraComponent.Yaw += (centerX - mouseEvent.MouseState.X)/1000f;
                _firstPersonCameraComponent.Pitch += (centerY - mouseEvent.MouseState.Y)/1000f;

                //Mouse.SetPosition(centerX, centerY);

                return true;
            }

            if (keyHeldEvent != null)
            {
                var didConsume = false;
                var moveX = 0;
                var moveZ = 0;

                if (keyHeldEvent.Key == Keys.A)
                {
                    moveX = -1;
                    didConsume = true;
                }
                if (keyHeldEvent.Key == Keys.D)
                {
                    moveX = 1;
                    didConsume = true;
                }
                if (keyHeldEvent.Key == Keys.W)
                {
                    moveZ = 1;
                    didConsume = true;
                }
                if (keyHeldEvent.Key == Keys.S)
                {
                    moveZ = -1;
                    didConsume = true;
                }
                var lookAt = _firstPersonCameraComponent.Transform.LocalMatrix;
                var relativeMovementVector = new Vector3(
                    moveX*ThumbstickMoveSensitivity,
                    0f,
                    -moveZ*ThumbstickMoveSensitivity);
                var absoluteMovementVector = Vector3.Transform(relativeMovementVector, lookAt);

                _firstPersonControllerPhysicsComponent.TargetVelocity = absoluteMovementVector;

                return didConsume;
            }

            return false;
        }
    }
}
