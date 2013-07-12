using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class Camera
    {
        private int m_Width;
        private int m_Height;

        public Camera(int width, int height)
        {
            this.m_Width = width;
            this.m_Height = height;
        }

        public float X
        {
            get;
            set;
        }

        public float Y
        {
            get;
            set;
        }

        public int Width
        {
            get
            {
                return this.m_Width;
            }
        }

        public int Height
        {
            get
            {
                return this.m_Height;
            }
        }

        public Matrix GetTransformationMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-this.X, -this.Y, 0));
        }
    }
}
