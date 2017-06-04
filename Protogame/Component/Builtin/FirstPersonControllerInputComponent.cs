using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protoinject;

namespace Protogame
{
    public class FirstPersonControllerInputComponent : IEventfulComponent, IUpdatableComponent, IEnabledComponent
    {
        private readonly FirstPersonCameraComponent _firstPersonCameraComponent;
        private readonly FirstPersonControllerPhysicsComponent _firstPersonControllerPhysicsComponent;

        private bool _didSetVelocityLastFrame = false;

        private float _cumulativeMoveX = 0;
        private float _cumulativeMoveZ = 0;

        public FirstPersonControllerInputComponent(
            [FromParent, RequireExisting] FirstPersonCameraComponent firstPersonCameraComponent,
            [FromParent, RequireExisting] FirstPersonControllerPhysicsComponent firstPersonControllerPhysicsComponent)
        {
            _firstPersonCameraComponent = firstPersonCameraComponent;
            _firstPersonControllerPhysicsComponent = firstPersonControllerPhysicsComponent;

            ThumbstickLookSensitivity = 1/100f;
            ThumbstickMoveSensitivity = 5f;
            MovementSpeed = 1f;
            MouseLock = true;

            Enabled = true;
        }

        public float ThumbstickLookSensitivity { get; set; }

        public float ThumbstickMoveSensitivity { get; set; }

        public float MovementSpeed { get; set; }

        public bool MouseLock { get; set; }

        public bool Enabled { get; set; }

        public void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext)
        {
            if (!Enabled)
            {
                return;
            }

            if (!_didSetVelocityLastFrame)
            {
                _firstPersonControllerPhysicsComponent.TargetVelocity = Vector3.Zero;
            }

            _didSetVelocityLastFrame = false;
            _cumulativeMoveX = 0;
            _cumulativeMoveZ = 0;
        }

        public bool Handle(ComponentizedEntity componentizedEntity, IGameContext gameContext,
            IEventEngine<IGameContext> eventEngine, Event @event)
        {
            if (!Enabled)
            {
                return false;
            }

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
                    gamepadEvent.GamePadState.ThumbSticks.Left.X*ThumbstickMoveSensitivity* MovementSpeed,
                    0f,
                    -gamepadEvent.GamePadState.ThumbSticks.Left.Y*ThumbstickMoveSensitivity * MovementSpeed);
                
                var absoluteMovementVector =
                    _firstPersonCameraComponent.ComputeWorldSpaceVectorFromLocalSpace(relativeMovementVector);

                _firstPersonControllerPhysicsComponent.TargetVelocity = absoluteMovementVector;
                _didSetVelocityLastFrame = true;

                return true;
            }

            if (mouseEvent != null && MouseLock)
            {
                var centerX = gameContext.Window.ClientBounds.Width/2;
                var centerY = gameContext.Window.ClientBounds.Height/2;

                _firstPersonCameraComponent.Yaw += (centerX - mouseEvent.X)/1000f;
                _firstPersonCameraComponent.Pitch += (centerY - mouseEvent.Y)/1000f;

                var limit = MathHelper.PiOver2 - MathHelper.ToRadians(5);
                _firstPersonCameraComponent.Pitch = MathHelper.Clamp(_firstPersonCameraComponent.Pitch, -limit, limit);
                
                gameContext.Window.SetCursorPosition(centerX, centerY);

                return true;
            }

            if (keyHeldEvent != null)
            {
                var didConsume = false;
                var moveX = 0;
                var moveZ = 0;

                if (keyHeldEvent.Key == Keys.A)
                {
                    _cumulativeMoveX -= 1;
                    didConsume = true;
                }
                if (keyHeldEvent.Key == Keys.D)
                {
                    _cumulativeMoveX += 1;
                    didConsume = true;
                }
                if (keyHeldEvent.Key == Keys.W)
                {
                    _cumulativeMoveZ += 1;
                    didConsume = true;
                }
                if (keyHeldEvent.Key == Keys.S)
                {
                    _cumulativeMoveZ -= 1;
                    didConsume = true;
                }

                var relativeMovementVector = new Vector3(
                    _cumulativeMoveX * ThumbstickMoveSensitivity * MovementSpeed,
                    0f,
                    -_cumulativeMoveZ * ThumbstickMoveSensitivity * MovementSpeed);
                var absoluteMovementVector =
                    _firstPersonCameraComponent.ComputeWorldSpaceVectorFromLocalSpace(relativeMovementVector);

                _firstPersonControllerPhysicsComponent.TargetVelocity = absoluteMovementVector;
                _didSetVelocityLastFrame = true;

                return didConsume;
            }

            return false;
        }
    }
}
