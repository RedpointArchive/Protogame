using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protogame.MultiLevel;

namespace Protogame.RTS
{
    public class UiManager
    {
        private int m_Tick = 0;
        private Vector2? m_LastMousePoint = null;
        private Vector2? m_CurrentMousePoint = null;
        private bool m_JustRightClicked = false;
        private List<string> m_ChatMessages = new List<string>();
        private List<double> m_GraphFrames = new List<double>();

        public UiManager()
        {
            this.Selected = new List<Unit>();
            this.Log("started ui manager.");
        }

        public List<Unit> Selected
        {
            get;
            private set;
        }

        public void Draw(RTSWorld world, GameContext context, XnaGraphics graphics)
        {
            this.Update(world, context);

            graphics.DrawStringLeft(0, 0, world.ActiveLevel.GetType().Name);
            graphics.DrawStringLeft(0, 32, this.m_Tick.ToString());

            if (this.m_LastMousePoint.HasValue && this.m_CurrentMousePoint.HasValue)
                graphics.DrawRectangle(this.m_LastMousePoint.Value, this.m_CurrentMousePoint.Value, Color.LimeGreen);

            foreach (Unit u in this.Selected)
            {
                Rectangle bb = new Rectangle((int)u.X, (int)u.Y - 1, u.Width + 1, u.Height + 1);
                if (u.Team != null)
                    graphics.DrawRectangle(bb, u.Team.Color);
                else
                    graphics.DrawRectangle(bb, Color.LimeGreen);
            }

            // Draw chat.
            int a = 16;
            for (int i = this.m_ChatMessages.Count - 1; i >= Math.Max(this.m_ChatMessages.Count - 11, 0); i--)
            {
                if (i < this.m_ChatMessages.Count)
                    graphics.DrawStringLeft(0, context.Graphics.GraphicsDevice.Viewport.Height - a, this.m_ChatMessages[i]);
                a += 16;
            }

            // Draw graph.
            if (this.m_GraphFrames.Count > 1)
            {
                for (int i = 1; i < this.m_GraphFrames.Count; i++)
                    graphics.DrawLine(i - 1, (float)this.m_GraphFrames[i - 1], i, (float)this.m_GraphFrames[i], 1, Color.Lime);
            }

            // Add frame information.
            if (this.m_GraphFrames.Count > 200)
                this.m_GraphFrames.RemoveAt(0);
            this.m_GraphFrames.Add(context.GameTime.ElapsedGameTime.TotalMilliseconds);

            this.m_Tick++;
        }

        private void Update(RTSWorld world, GameContext context)
        {
            MouseState mouse = Mouse.GetState();

            // Left mouse action.
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (!this.m_LastMousePoint.HasValue)
                    this.m_LastMousePoint = new Vector2(mouse.X, mouse.Y);
                else
                {
                    this.m_CurrentMousePoint = new Vector2(mouse.X, mouse.Y);

                    // Determine selected objects.
                    Rectangle coverage = new Rectangle(
                        (int)this.m_LastMousePoint.Value.X,
                        (int)this.m_LastMousePoint.Value.Y,
                        (int)this.m_CurrentMousePoint.Value.X - (int)this.m_LastMousePoint.Value.X,
                        (int)this.m_CurrentMousePoint.Value.Y - (int)this.m_LastMousePoint.Value.Y
                        ).Normalize();
                    this.Selected.Clear();
                    foreach (IMultiLevelEntity e in world.ActiveLevel.Entities)
                        if (e is Unit)
                        {
                            Unit u = e as Unit;
                            Rectangle bb = new Rectangle((int)u.X, (int)u.Y, u.Width, u.Height);
                            if (coverage.Intersects(bb))
                                this.Selected.Add(u);
                        }
                }
            }
            else
            {
                if (this.m_LastMousePoint != null && this.m_CurrentMousePoint != null)
                {
                    if (this.m_LastMousePoint.Value == this.m_CurrentMousePoint.Value)
                    {
                        // Left mouse action.
                        foreach (Unit u in this.Selected)
                        {
                            if (u.LocallyOwned)
                                u.LeftAction(world);
                        }
                    }
                }
                this.m_LastMousePoint = null;
                this.m_CurrentMousePoint = null;
            }

            // Right mouse action.
            if (mouse.RightButton == ButtonState.Pressed)
            {
                this.m_JustRightClicked = true;
            }
            else if (mouse.RightButton == ButtonState.Released && this.m_JustRightClicked)
            {
                this.Log("right action triggered on " + this.Selected.Count + " selected units.");
                foreach (Unit u in this.Selected)
                {
                    if (u.LocallyOwned)
                        u.RightAction(world);
                }
                this.m_JustRightClicked = false;
            }
        }

        public void Log(string msg)
        {
            this.m_ChatMessages.Add("(debug) " + msg);
        }
    }

    public static class RectangleExtensions
    {
        public static Rectangle Normalize(this Rectangle rect)
        {
            Rectangle result = rect;
            if (result.Width < 0)
            {
                result.X = rect.X + rect.Width;
                result.Width = -rect.Width;
            }
            if (result.Height < 0)
            {
                result.Y = rect.Y + rect.Height;
                result.Height = -rect.Height;
            }
            return result;
        }
    }
}

