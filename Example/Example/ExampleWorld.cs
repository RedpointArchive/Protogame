using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Input;
using Protogame.Platformer;

namespace Example
{
    public class ExampleWorld : PlayerWorld
    {
        private bool m_CanJump;

        public ExampleGame Game { get; set; }
        
        public override void DrawBelow(GameContext context)
        {
            // Clear the screen.
            context.Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        }

        public override void DrawAbove(GameContext context)
        {
            XnaGraphics gr = new XnaGraphics(context);
            gr.DrawStringCentered(Tileset.TILESET_PIXEL_WIDTH / 2, 20, "Example Game!");

            base.DrawAbove(context);
        }

        public override bool Update(GameContext context)
        {
            bool handle = base.Update(context);
            if (!handle) return false;

            // Cast first.
            Player player = this.Player as Player;

            // Handle if player exists.
            if (player != null)
            {
                // Send input.
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    player.MoveLeft(this);
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    player.MoveRight(this);
                if (Keyboard.GetState().IsKeyUp(Keys.Left) && Keyboard.GetState().IsKeyUp(Keys.Right))
                    player.MoveEnd();
                if (Keyboard.GetState().IsKeyDown(Keys.X))
                {
                    if (this.m_CanJump)
                        player.Jump(this);
                    this.m_CanJump = false;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.X))
                {
                    player.JumpEnd();
                    this.m_CanJump = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.C))
                    player.Action(this);
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                    this.LoadLevel(this.CurrentLevel);
            }

            // Continue entity updates.
            return true;
        }
    }
}
