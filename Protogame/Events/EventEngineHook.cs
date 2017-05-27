using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
#if PLATFORM_ANDROID || PLATFORM_OUYA
	using Microsoft.Xna.Framework.Input.Touch;
#endif

    /// <summary>
    /// A game engine hook that raised appropriate input events as they occur.
    /// </summary>
    /// <module>Events</module>
    /// <internal>True</internal>
    public class EventEngineHook : IEngineHook
    {
        /// <summary>
        /// The event engine through which we raise events.
        /// </summary>
        private readonly IEventEngine<IGameContext> _eventEngine;

        /// <summary>
        /// A dictionary of the last game pad state for each player.  We use this to detect
        /// the difference between button press and button held events.
        /// </summary>
        private readonly Dictionary<int, GamePadState> _lastGamePadStates;

        /// <summary>
        /// The last mouse state.  We use this to detect mouse move events and only fire them
        /// when appropriate.
        /// </summary>
        private MouseState? _lastMouseState;

        /// <summary>
        /// Determines if we are using the gamepad.
        /// </summary>
        private bool _gamepadEnabled = true;

        /// <summary>
        /// Determines if we've checked gamepad connectivity.  On some systems, calling
        /// <see cref="GamePad.GetState(int)"/> is relatively expensive and can cause stuttering
        /// in the game, so by default we only check connectivity at the start of the game.
        /// </summary>
        private bool _hasCheckedGamepadConnectivity = false;

        /// <summary>
        /// Once we've checked connectivity, we store whether we have any gamepads in here.  If
        /// we don't have any gamepads, we don't call <see cref="GamePad.GetState(int)"/>.
        /// </summary>
        private bool _hasAnyGamepads = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventEngineHook"/> class.
        /// </summary>
        /// <param name="eventEngine">
        /// The event engine to raise events on.
        /// </param>
        public EventEngineHook(IEventEngine<IGameContext> eventEngine)
        {
            _eventEngine = eventEngine;
            _lastGamePadStates = new Dictionary<int, GamePadState>();
        }

        /// <summary>
        /// The render callback for the engine hook.  This is triggered right before the rendering
        /// of the world manager.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        /// <summary>
        /// The update callback for the engine hook.  This is triggered right before the update of the
        /// world manager.
        /// <para>
        /// For this engine hook, this updates and fires input events as appropriate.
        /// </para>
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="updateContext">
        /// The update context.
        /// </param>
        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            UpdateKeyboard(gameContext);
            UpdateMouse(gameContext);

            if (_gamepadEnabled)
            {
                try
                {
                    UpdateGamepad(gameContext);
                }
                catch (DllNotFoundException)
                {
                    _gamepadEnabled = false;
                }
            }

#if PLATFORM_ANDROID || PLATFORM_OUYA
            this.UpdateTouch(gameContext);
#endif
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
        }

        /// <summary>
        /// Iterates through all of the connected game pads, updates their state and fires input events.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        private void UpdateGamepad(IGameContext gameContext)
        {
            if (!_hasCheckedGamepadConnectivity)
            {
                for (var index = 0; index < GamePad.MaximumGamePadCount; index++)
                {
                    var gamepadState = GamePad.GetState(index);

                    if (gamepadState.IsConnected)
                    {
                        _hasAnyGamepads = true;
                        break;
                    }
                }

                _hasCheckedGamepadConnectivity = true;
            }

            if (!_hasAnyGamepads)
            {
                return;
            }

            for (var index = 0; index < GamePad.MaximumGamePadCount; index++)
            {
                var gamepadState = GamePad.GetState(index);

                if (gamepadState.IsConnected)
                {
                    UpdateGamepadSingle(gameContext, index, gamepadState);
                }
            }
        }

        /// <summary>
        /// Updates and fires input events for a single game pad.  This method is used by <see cref="UpdateGamepad"/>.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="index">
        /// The player index.
        /// </param>
        /// <param name="gamepadState">
        /// The game pad state.
        /// </param>
        private void UpdateGamepadSingle(IGameContext gameContext, int index, GamePadState gamepadState)
        {
            var lastGamepadState = new GamePadState();
            if (_lastGamePadStates.ContainsKey(index))
            {
                lastGamepadState = _lastGamePadStates[index];
            }

            if (gamepadState.ThumbSticks.Left.X != 0 || gamepadState.ThumbSticks.Right.X != 0 || gamepadState.ThumbSticks.Left.Y != 0 || gamepadState.ThumbSticks.Right.Y != 0)
            {
                _eventEngine.Fire(
                    gameContext,
                    new GamePadThumbstickActiveEvent
                    {
                        GamePadIndex = index,
                        GamePadState = gamepadState
                    });
            }

            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                if (gamepadState.IsButtonDown(button))
                {
                    if (lastGamepadState.IsButtonUp(button))
                    {
                        _eventEngine.Fire(
                            gameContext, 
                            new GamePadButtonPressEvent
                            {
                                Button = button, 
                                GamePadState = gamepadState, 
                                GamePadIndex = index
                            });
                    }
                    else
                    {
                        _eventEngine.Fire(
                            gameContext, 
                            new GamePadButtonHeldEvent
                            {
                                Button = button, 
                                GamePadState = gamepadState, 
                                GamePadIndex = index
                            });
                    }
                }
                else
                {
                    if (lastGamepadState.IsButtonDown(button))
                    {
                        _eventEngine.Fire(
                            gameContext, 
                            new GamePadButtonReleaseEvent
                            {
                                Button = button, 
                                GamePadState = gamepadState, 
                                GamePadIndex = index
                            });
                    }
                }
            }

            _lastGamePadStates[index] = gamepadState;
        }

        /// <summary>
        /// Updates and fires input events for the keyboard.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        private void UpdateKeyboard(IGameContext gameContext)
        {
            var keyboardState = Keyboard.GetState();

            var allKeys = Enum.GetValues(typeof(Keys));
            foreach (Keys key in allKeys)
            {
                if (keyboardState.IsKeyDown(key))
                {
                    _eventEngine.Fire(gameContext, new KeyHeldEvent { Key = key, KeyboardState = keyboardState });
                }

                var change = keyboardState.IsKeyChanged(this, key);

                if (change == KeyState.Down)
                {
                    _eventEngine.Fire(gameContext, new KeyPressEvent { Key = key, KeyboardState = keyboardState });
                }
                else if (change == KeyState.Up)
                {
                    _eventEngine.Fire(gameContext, new KeyReleaseEvent { Key = key, KeyboardState = keyboardState });
                }
            }
        }

        /// <summary>
        /// Updates and fires input events for the mouse.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        private void UpdateMouse(IGameContext gameContext)
        {
            var mouseState = Mouse.GetState();
            if (_lastMouseState == null)
            {
                _lastMouseState = mouseState;
                return;
            }

            var leftChange = mouseState.LeftChanged(this);
            var middleChange = mouseState.MiddleChanged(this);
            var rightChange = mouseState.RightChanged(this);

            if (leftChange == ButtonState.Pressed)
            {
                _eventEngine.Fire(
                    gameContext, 
                    new MousePressEvent { Button = MouseButton.Left, MouseState = mouseState });
            }

            if (middleChange == ButtonState.Pressed)
            {
                _eventEngine.Fire(
                    gameContext, 
                    new MousePressEvent { Button = MouseButton.Middle, MouseState = mouseState });
            }

            if (rightChange == ButtonState.Pressed)
            {
                _eventEngine.Fire(
                    gameContext, 
                    new MousePressEvent { Button = MouseButton.Right, MouseState = mouseState });
            }

            if (leftChange == ButtonState.Released)
            {
                _eventEngine.Fire(
                    gameContext,
                    new MouseReleaseEvent { Button = MouseButton.Left, MouseState = mouseState });
            }

            if (middleChange == ButtonState.Released)
            {
                _eventEngine.Fire(
                    gameContext,
                    new MouseReleaseEvent { Button = MouseButton.Middle, MouseState = mouseState });
            }

            if (rightChange == ButtonState.Released)
            {
                _eventEngine.Fire(
                    gameContext,
                    new MouseReleaseEvent { Button = MouseButton.Right, MouseState = mouseState });
            }

            if (mouseState.X != _lastMouseState.Value.X || mouseState.Y != _lastMouseState.Value.Y)
            {
                _eventEngine.Fire(
                    gameContext, 
                    new MouseMoveEvent
                    {
                        X = mouseState.X, 
                        Y = mouseState.Y, 
                        LastX = _lastMouseState.Value.X, 
                        LastY = _lastMouseState.Value.Y, 
                        MouseState = mouseState
                    });
            }

            if (_lastMouseState.HasValue)
            {
                if (mouseState.ScrollWheelValue != _lastMouseState.Value.ScrollWheelValue)
                {
                    _eventEngine.Fire(
                        gameContext,
                        new MouseScrollEvent { ScrollDelta = (_lastMouseState.Value.ScrollWheelValue - mouseState.ScrollWheelValue), MouseState = mouseState });
                }
            }

            _lastMouseState = mouseState;
        }

#if PLATFORM_ANDROID || PLATFORM_OUYA

		/// <summary>
		/// Updates and fires input events for a touch device.
		/// </summary>
		/// <param name="gameContext">
		/// The game context.
		/// </param>
		private void UpdateTouch(IGameContext gameContext)
        {
            var touchState = TouchPanel.GetState();

            foreach (var touch in touchState)
            {
                if (touch.State == TouchLocationState.Pressed)
                {
                    _eventEngine.Fire(
                        gameContext,
                        new TouchPressEvent
                        {
                            X = touch.Position.X,
                            Y = touch.Position.Y,
                            Pressure = touch.Pressure,
                            Id = touch.Id,
                            TouchLocationState = touch.State
                        });
                }
                else if (touch.State == TouchLocationState.Moved)
                {
                    _eventEngine.Fire(
                        gameContext,
                        new TouchHeldEvent
                        {
                            X = touch.Position.X,
                            Y = touch.Position.Y,
                            Pressure = touch.Pressure,
                            Id = touch.Id,
                            TouchLocationState = touch.State
                        });
                }
                else if (touch.State == TouchLocationState.Released)
                {
                    _eventEngine.Fire(
                        gameContext,
                        new TouchReleaseEvent
                        {
                            X = touch.Position.X,
                            Y = touch.Position.Y,
                            Pressure = touch.Pressure,
                            Id = touch.Id,
                            TouchLocationState = touch.State
                        });
                }
            }
		}

#endif
    }
}