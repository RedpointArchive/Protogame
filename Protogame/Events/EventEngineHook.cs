using System;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class EventEngineHook : IEngineHook
    {
        private readonly IEventEngine m_EventEngine;
        private Keys[] m_LastPressedKeys;
        private MouseState? m_LastMouseState;
        
        public EventEngineHook(IEventEngine eventEngine)
        {
            this.m_EventEngine = eventEngine;
        }
        
        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.UpdateKeyboard(gameContext);
            this.UpdateMouse(gameContext);
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }
        
        private void UpdateKeyboard(IGameContext gameContext)
        {
            var keyboardState = Keyboard.GetState();
            if (this.m_LastPressedKeys == null)
            {
                this.m_LastPressedKeys = keyboardState.GetPressedKeys();
                return;
            }
            
            var allKeys = Enum.GetValues(typeof(Keys));
            foreach (Keys key in allKeys)
            {
                if (keyboardState.IsKeyDown(key))
                    this.m_EventEngine.Fire(gameContext, new KeyHeldEvent { Key = key, KeyboardState = keyboardState });
                if (keyboardState.IsKeyPressed(key))
                    this.m_EventEngine.Fire(gameContext, new KeyPressEvent { Key = key, KeyboardState = keyboardState });
            }
        }
        
        private void UpdateMouse(IGameContext gameContext)
        {
            var mouseState = Mouse.GetState();
            if (this.m_LastMouseState == null)
            {
                this.m_LastMouseState = mouseState;
                return;
            }
            
            if (mouseState.LeftPressed(this))
            {
                this.m_EventEngine.Fire(gameContext, new MousePressEvent { Button = MouseButton.Left, MouseState = mouseState });
            }
            if (mouseState.RightPressed(this))
            {
                this.m_EventEngine.Fire(gameContext, new MousePressEvent { Button = MouseButton.Right, MouseState = mouseState });
            }
            if (mouseState.MiddlePressed(this))
            {
                this.m_EventEngine.Fire(gameContext, new MousePressEvent { Button = MouseButton.Middle, MouseState = mouseState });
            }
            
            if (mouseState.X != this.m_LastMouseState.Value.X ||
                mouseState.Y != this.m_LastMouseState.Value.Y)
            {
                this.m_EventEngine.Fire(gameContext, new MouseMoveEvent
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
    }
}

