using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protoinject;

namespace Protogame
{
    public class FirstPersonControllerInputComponent : IEventfulComponent, IUpdatableComponent, IEnabledComponent
    {
        private readonly FirstPersonCameraComponent _firstPersonCameraComponent;
        private readonly IFirstPersonControllerComponent _firstPersonControllerComponent;

        private float _cumulativeMoveX = 0;
        private float _cumulativeMoveZ = 0;

        public FirstPersonControllerInputComponent(
            [FromParent, RequireExisting] FirstPersonCameraComponent firstPersonCameraComponent,
            [FromParent, RequireExisting] IFirstPersonControllerComponent firstPersonControllerComponent)
        {
            _firstPersonCameraComponent = firstPersonCameraComponent;
            _firstPersonControllerComponent = firstPersonControllerComponent;

            ThumbstickLookSensitivity = 1/100f;
            ThumbstickMoveSensitivity = 1f;
            MouseLock = true;

            Enabled = true;
        }

        public float ThumbstickLookSensitivity { get; set; }

        public float ThumbstickMoveSensitivity { get; set; }

        public bool MouseLock { get; set; }

        public bool Enabled { get; set; }

        public void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext)
        {
            if (!Enabled)
            {
                return;
            }

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
                    gamepadEvent.GamePadState.ThumbSticks.Left.X*ThumbstickMoveSensitivity,
                    0f,
                    -gamepadEvent.GamePadState.ThumbSticks.Left.Y*ThumbstickMoveSensitivity);
                
                var absoluteMovementVector =
                    _firstPersonCameraComponent.ComputeWorldSpaceVectorFromLocalSpace(relativeMovementVector);

                _firstPersonControllerComponent.MoveInDirection(absoluteMovementVector);

                return true;
            }

            if (mouseEvent != null && MouseLock)
            {
                var centerX = gameContext.Window.ClientBounds.Width/2;
                var centerY = gameContext.Window.ClientBounds.Height/2;

                _firstPersonCameraComponent.Yaw += (centerX - mouseEvent.MouseState.X)/1000f;
                _firstPersonCameraComponent.Pitch += (centerY - mouseEvent.MouseState.Y)/1000f;

                var limit = MathHelper.PiOver2 - MathHelper.ToRadians(5);
                _firstPersonCameraComponent.Pitch = MathHelper.Clamp(_firstPersonCameraComponent.Pitch, -limit, limit);

                Mouse.SetPosition(centerX, centerY);

                return true;
            }

            if (keyHeldEvent != null)
            {
                var didConsume = false;

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
                    _cumulativeMoveX * ThumbstickMoveSensitivity,
                    0f,
                    -_cumulativeMoveZ * ThumbstickMoveSensitivity);
                var absoluteMovementVector =
                    _firstPersonCameraComponent.ComputeWorldSpaceVectorFromLocalSpace(relativeMovementVector);

                _firstPersonControllerComponent.MoveInDirection(absoluteMovementVector);

                return didConsume;
            }

            return false;
        }
    }
}
