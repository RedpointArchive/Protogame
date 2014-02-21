namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
#if PLATFORM_ANDROID || PLATFORM_OUYA
	using Microsoft.Xna.Framework.Input.Touch;
#endif

    /// <summary>
    /// A game engine hook that raised appropriate input events as they occur.
    /// </summary>
    public class EventEngineHook : IEngineHook
    {
        /// <summary>
        /// The event engine through which we raise events.
        /// </summary>
        private readonly IEventEngine<IGameContext> m_EventEngine;

        /// <summary>
        /// A dictionary of the last game pad state for each player.  We use this to detect
        /// the difference between button press and button held events.
        /// </summary>
        private readonly Dictionary<PlayerIndex, GamePadState> m_LastGamePadStates;

        /// <summary>
        /// The last mouse state.  We use this to detect mouse move events and only fire them
        /// when appropriate.
        /// </summary>
        private MouseState? m_LastMouseState;

        /// <summary>
        /// Determines if we are using the gamepad.
        /// </summary>
        private bool m_GamepadEnabled = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventEngineHook"/> class.
        /// </summary>
        /// <param name="eventEngine">
        /// The event engine to raise events on.
        /// </param>
        public EventEngineHook(IEventEngine<IGameContext> eventEngine)
        {
            this.m_EventEngine = eventEngine;
            this.m_LastGamePadStates = new Dictionary<PlayerIndex, GamePadState>();
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
            this.UpdateKeyboard(gameContext);
            this.UpdateMouse(gameContext);

            if (this.m_GamepadEnabled)
            {
                try
                {
                    this.UpdateGamepad(gameContext);
                }
                catch (DllNotFoundException)
                {
                    this.m_GamepadEnabled = false;
                }
            }

#if PLATFORM_ANDROID || PLATFORM_OUYA
            this.UpdateTouch(gameContext);
#endif
        }

        /// <summary>
        /// Iterates through all of the connected game pads, updates their state and fires input events.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        private void UpdateGamepad(IGameContext gameContext)
        {
            foreach (var index in new[] { PlayerIndex.One, PlayerIndex.Two, PlayerIndex.Three, PlayerIndex.Four })
            {
                var gamepadState = GamePad.GetState(index);
                if (gamepadState.IsConnected)
                {
                    this.UpdateGamepadSingle(gameContext, index, gamepadState);
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
        private void UpdateGamepadSingle(IGameContext gameContext, PlayerIndex index, GamePadState gamepadState)
        {
            var lastGamepadState = new GamePadState();
            if (this.m_LastGamePadStates.ContainsKey(index))
            {
                lastGamepadState = this.m_LastGamePadStates[index];
            }

            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                if (gamepadState.IsButtonDown(button))
                {
                    if (lastGamepadState.IsButtonUp(button))
                    {
                        this.m_EventEngine.Fire(
                            gameContext, 
                            new GamePadButtonPressEvent
                            {
                                Button = button, 
                                GamePadState = gamepadState, 
                                PlayerIndex = index
                            });
                    }
                    else
                    {
                        this.m_EventEngine.Fire(
                            gameContext, 
                            new GamePadButtonHeldEvent
                            {
                                Button = button, 
                                GamePadState = gamepadState, 
                                PlayerIndex = index
                            });
                    }
                }
                else
                {
                    if (lastGamepadState.IsButtonDown(button))
                    {
                        this.m_EventEngine.Fire(
                            gameContext, 
                            new GamePadButtonReleaseEvent
                            {
                                Button = button, 
                                GamePadState = gamepadState, 
                                PlayerIndex = index
                            });
                    }
                }
            }

            this.m_LastGamePadStates[index] = gamepadState;
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
                    this.m_EventEngine.Fire(gameContext, new KeyHeldEvent { Key = key, KeyboardState = keyboardState });
                }

                if (keyboardState.IsKeyPressed(this, key))
                {
                    this.m_EventEngine.Fire(gameContext, new KeyPressEvent { Key = key, KeyboardState = keyboardState });
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
            if (this.m_LastMouseState == null)
            {
                this.m_LastMouseState = mouseState;
                return;
            }

            var leftChange = mouseState.LeftChanged(this);
            var middleChange = mouseState.MiddleChanged(this);
            var rightChange = mouseState.RightChanged(this);

            if (leftChange == ButtonState.Pressed)
            {
                this.m_EventEngine.Fire(
                    gameContext, 
                    new MousePressEvent { Button = MouseButton.Left, MouseState = mouseState });
            }

            if (middleChange == ButtonState.Pressed)
            {
                this.m_EventEngine.Fire(
                    gameContext, 
                    new MousePressEvent { Button = MouseButton.Middle, MouseState = mouseState });
            }

            if (rightChange == ButtonState.Pressed)
            {
                this.m_EventEngine.Fire(
                    gameContext, 
                    new MousePressEvent { Button = MouseButton.Right, MouseState = mouseState });
            }

            if (leftChange == ButtonState.Released)
            {
                this.m_EventEngine.Fire(
                    gameContext,
                    new MouseReleaseEvent { Button = MouseButton.Left, MouseState = mouseState });
            }

            if (middleChange == ButtonState.Released)
            {
                this.m_EventEngine.Fire(
                    gameContext,
                    new MouseReleaseEvent { Button = MouseButton.Middle, MouseState = mouseState });
            }

            if (rightChange == ButtonState.Released)
            {
                this.m_EventEngine.Fire(
                    gameContext,
                    new MouseReleaseEvent { Button = MouseButton.Right, MouseState = mouseState });
            }

            if (mouseState.X != this.m_LastMouseState.Value.X || mouseState.Y != this.m_LastMouseState.Value.Y)
            {
                this.m_EventEngine.Fire(
                    gameContext, 
                    new MouseMoveEvent
                    {
                        X = mouseState.X, 
                        Y = mouseState.Y, 
                        LastX = this.m_LastMouseState.Value.X, 
                        LastY = this.m_LastMouseState.Value.Y, 
                        MouseState = mouseState
                    });
            }

            this.m_LastMouseState = mouseState;
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
                this.m_EventEngine.Fire(
                    gameContext, 
                    new TouchPressEvent
                    {
                        X = touch.Position.X, 
                        Y = touch.Position.Y, 
                        Pressure = touch.Pressure,
                        TouchLocationState = touch.State
                    });
            }
		}

#endif
    }
}