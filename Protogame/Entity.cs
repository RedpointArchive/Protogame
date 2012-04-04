using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class Entity : IBoundingBox, IEntity
    {
        private int m_ImageIndex = 0;
        private int m_ImageFrameAlarm = 0;

        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string[] Images { get; set; }
        public Color Color { get; set; }
        public bool ImageFlipX { get; set; }
        public float XSpeed { get; set; }
        public float YSpeed { get; set; }
        public int ImageSpeed { get; set; }

        public Entity()
        {
            this.Color = Color.White;
            this.ImageSpeed = 1;
        }

        public virtual string Image
        {
            get
            {
                if (this.m_ImageIndex >= this.Images.Length)
                    this.m_ImageIndex = 0;
                return this.Images[this.m_ImageIndex];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public T CollidesAt<T>(World world, int x, int y) where T : Entity
        {
            return Helpers.CollidesAt<T>(this, world, x, y);
        }

        public string[] GetTexture(string name)
        {
            return new string[] { name };
        }

        public string[] GetTextureAnim(string name, int end)
        {
            List<string> result = new List<string>();
            for (int i = 1; i <= end; i += 1)
                result.Add(name + i);
            return result.ToArray();
        }

        public string[] GetTextureAnimRandom(string name, int end)
        {
            List<string> result = new List<string>();
            for (int i = 1; i <= end; i += 1)
                result.Add(name + i);
            result.Shuffle();
            return result.ToArray();
        }

        public virtual void Update(World world)
        {
            if (this.m_ImageFrameAlarm == 0)
            {
                this.m_ImageIndex++;
                this.m_ImageFrameAlarm = this.ImageSpeed - 1;
            }
            else
                this.m_ImageFrameAlarm -= 1;
            if (this.m_ImageIndex >= this.Images.Length)
                this.m_ImageIndex = 0;
        }

        public virtual void Draw(World world, XnaGraphics graphics)
        {
        }

        public void PerformAlignment(int cellAlignment, int maxAdjust, Action simulateLeft, Action simulateRight)
        {
            // Perform cell alignment.
            float rem = this.X % Tileset.TILESET_CELL_WIDTH;
            if (rem > Tileset.TILESET_CELL_WIDTH - cellAlignment ||
                rem < cellAlignment)
            {
                float targetX = (float)Math.Round(this.X / Tileset.TILESET_CELL_WIDTH) * Tileset.TILESET_CELL_WIDTH;
                if (Math.Abs(targetX - this.X) > maxAdjust)
                {
                    if (targetX > this.X)
                        simulateRight();
                    else if (targetX < this.X)
                        simulateLeft();
                }
                else
                    this.X = targetX;
            }
        }
    }
}
