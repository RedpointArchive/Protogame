using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame.Particles;

namespace Protogame
{
    public sealed class WorldManager
    {
        private void DrawSpriteAt(GameContext context, float x, float y, int width, int height, string image, Color color, bool flipX)
        {
            if (flipX)
                context.SpriteBatch.Draw(
                    context.Textures[image],
                    new Rectangle((int)x, (int)y, width, height),
                    null,
                    color.ToPremultiplied(),
                    0,
                    new Vector2(0, 0),
                    SpriteEffects.FlipHorizontally,
                    0
                    );
            else
                context.SpriteBatch.Draw(
                    context.Textures[image],
                    new Rectangle((int)x, (int)y, width, height),
                    color.ToPremultiplied()
                    );
        }

        private void HandleRenderOfEntity(GameContext context, IEntity a)
        {
            this.DrawSpriteAt(context, (float)a.X, (float)a.Y, a.Width, a.Height, a.Image, a.Color, a.ImageFlipX);

            // Check to see if this entity is residing on an edge of the screen.
            if (a.X >= Tileset.TILESET_PIXEL_WIDTH) a.X -= Tileset.TILESET_PIXEL_WIDTH;
            if (a.X < 0) a.X += Tileset.TILESET_PIXEL_WIDTH;
            if (a.X > Tileset.TILESET_PIXEL_WIDTH - a.Width)
            {
                // Draw a mirror image on the left side of the screen.
                this.DrawSpriteAt(context, (float)a.X - Tileset.TILESET_PIXEL_WIDTH, (float)a.Y, a.Width, a.Height, a.Image, a.Color, a.ImageFlipX);
            }
        }

        public void Draw(GameContext context)
        {
            // Clear the screen.
            context.SpriteBatch.Begin();

            // Draw world below.
            context.World.DrawBelow(context);

            // Render all of the tiles.
            for (int z = 0; z < Tileset.TILESET_DEPTH; z += 1)
            {
                for (int x = 0; x < Tileset.TILESET_WIDTH; x += 1)
                {
                    for (int y = 0; y < Tileset.TILESET_HEIGHT; y += 1)
                    {
                        Tile t = context.World.Tileset[x, y, z];
                        if (t == null) continue;
                        if (t.Image == null) continue;
                        context.SpriteBatch.Draw(
                            context.Textures[t.Image],
                            new Rectangle((int)t.X, (int)t.Y, t.Width, t.Height),
                            (t.TX != -1 && t.TY != -1) ? new Rectangle?(new Rectangle(t.TX * t.Width, t.TY * t.Height, t.Width, t.Height)) : null,
                            Color.White
                            );
                    }
                }
            }

            // Render all of the actors.
            foreach (IEntity a in context.World.Entities)
                if ((a is ParticleEntity) && (a as ParticleEntity).Definition.RenderMode == ParticleMode.Background)
                    this.HandleRenderOfEntity(context, a);
            foreach (IEntity a in context.World.Entities)
                if (a.Image != null && !(a is ParticleEntity))
                    this.HandleRenderOfEntity(context, a);
            foreach (IEntity a in context.World.Entities)
                if ((a is ParticleEntity) && (a as ParticleEntity).Definition.RenderMode == ParticleMode.Foreground)
                    this.HandleRenderOfEntity(context, a);
            XnaGraphics gr = new XnaGraphics(context);
            foreach (IEntity a in context.World.Entities)
                a.Draw(context.World, gr);

            // Draw world above.
            context.World.DrawAbove(context);

            // Finish rendering.
            context.SpriteBatch.End();
        }

        public void Update(GameContext context)
        {
            bool handle = context.World.Update(context);
            if (!handle) return;

            // Update all of the actors.
            foreach (IEntity a in context.World.Entities.ToArray())
                a.Update(context.World);

            // Update tick.
            context.World.Tick += 1;
        }
    }
}
