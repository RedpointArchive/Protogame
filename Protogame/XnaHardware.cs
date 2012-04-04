using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class XnaHardware
    {
        private Game m_Game = null;

        public XnaHardware(Game game)
        {
            this.m_Game = game;
        }

        public bool ShowMouseCursor
        {
            get
            {
                return this.m_Game.IsMouseVisible;
            }
            set
            {
                this.m_Game.IsMouseVisible = value;
            }
        }

        public int MouseX
        {
            get { return Mouse.GetState().X; }
        }

        public int MouseY
        {
            get { return Mouse.GetState().Y; }
        }

        public bool MouseLeftPressed
        {
            get { return Mouse.GetState().LeftButton == ButtonState.Pressed; }
        }

        public bool JumpPressed
        {
            get { return Keyboard.GetState().IsKeyDown(Keys.X); }
        }
    }
}
