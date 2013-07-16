//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <remarks>
    /// Sourced from http://social.msdn.microsoft.com/forums/en-US/xnaframework/thread/7e5cc12f-3b75-49ef-8da9-c00a75139a21/.
    /// </remarks>
    class DepthSpriteBatch
    {
        private VertexPositionColorTexture[] m_Vertices;
        private short[] m_Indices;
        private int m_VertexCount = 0;
        private int m_IndexCount = 0;
        private Texture2D m_Texture;
        private VertexDeclaration m_Declaration;
        private GraphicsDevice m_Device;

        //  these should really be properties
        public Matrix World
        {
            get;
            set;
        }

        public Matrix View
        {
            get;
            set;
        }

        public Matrix Projection
        {
            get;
            set;
        }

        public Effect Effect
        {
            get;
            set;
        }

        public DepthSpriteBatch(GraphicsDevice device)
        {
            this.m_Device = device;
            this.m_Vertices = new VertexPositionColorTexture[256];
            this.m_Indices = new short[m_Vertices.Length * 3 / 2];
        }

        public void ResetMatrices(int width, int height)
        {
            this.World = Matrix.Identity;
            this.View = new Matrix(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, -1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, -1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
            this.Projection = Matrix.CreateOrthographicOffCenter(
                0, width, -height, 0, 0, 1);
        }

        public void Draw(Texture2D texture, Vector2 dst, Color color, float z)
        {
            this.Draw(texture, texture.Bounds, new Rectangle((int)dst.X, (int)dst.Y, texture.Bounds.Width, texture.Bounds.Height), color, z);
        }

        public void Draw(Texture2D texture, Rectangle dst, Color color, float z)
        {
            this.Draw(texture, texture.Bounds, dst, color, z);
        }

        public void Draw(Texture2D texture, Rectangle srcRectangle, Rectangle dstRectangle, Color color, float z)
        {
            //  if the texture changes, we flush all queued sprites.
            if (this.m_Texture != null && this.m_Texture != texture)
                this.Flush();
            this.m_Texture = texture;

            //  ensure space for my vertices and indices.
            this.EnsureSpace(6, 4);

            //  add the new indices
            m_Indices[m_IndexCount++] = (short)(m_VertexCount + 0);
            m_Indices[m_IndexCount++] = (short)(m_VertexCount + 1);
            m_Indices[m_IndexCount++] = (short)(m_VertexCount + 3);
            m_Indices[m_IndexCount++] = (short)(m_VertexCount + 1);
            m_Indices[m_IndexCount++] = (short)(m_VertexCount + 2);
            m_Indices[m_IndexCount++] = (short)(m_VertexCount + 3);

            // add the new vertices
            m_Vertices[m_VertexCount++] = new VertexPositionColorTexture(
                new Vector3(dstRectangle.Left, dstRectangle.Top, z)
                , color, GetUV(srcRectangle.Left, srcRectangle.Top));
            m_Vertices[m_VertexCount++] = new VertexPositionColorTexture(
                new Vector3(dstRectangle.Right, dstRectangle.Top, z)
                , color, GetUV(srcRectangle.Right, srcRectangle.Top));
            m_Vertices[m_VertexCount++] = new VertexPositionColorTexture(
                new Vector3(dstRectangle.Right, dstRectangle.Bottom, z)
                , color, GetUV(srcRectangle.Right, srcRectangle.Bottom));
            m_Vertices[m_VertexCount++] = new VertexPositionColorTexture(
                new Vector3(dstRectangle.Left, dstRectangle.Bottom, z)
                , color, GetUV(srcRectangle.Left, srcRectangle.Bottom));

            //  we premultiply all vertices times the world matrix.
            //  the world matrix changes alot and we don't want to have to flush
            //  every time it changes.
            Matrix world = this.World;
            for (int i = m_VertexCount - 4; i < m_VertexCount; i++)
                Vector3.Transform(ref m_Vertices[i].Position, ref world, out m_Vertices[i].Position);
        }

        Vector2 GetUV(float x, float y)
        {
            return new Vector2(x / (float)m_Texture.Width, y / (float)m_Texture.Height);
        }

        void EnsureSpace(int indexSpace, int vertexSpace)
        {
            if (m_IndexCount + indexSpace >= m_Indices.Length)
                Array.Resize(ref m_Indices, Math.Max(m_IndexCount + indexSpace, m_Indices.Length * 2));
            if (m_VertexCount + vertexSpace >= m_Vertices.Length)
                Array.Resize(ref m_Vertices, Math.Max(m_VertexCount + vertexSpace, m_Vertices.Length * 2));
        }

        public void Flush()
        {
            if (this.m_VertexCount > 0)
            {
                Effect effect = this.Effect;
                //  set the only parameter this effect takes.
                effect.Parameters["MatrixTransform"].SetValue(this.View * this.Projection);
                effect.Parameters["Texture"].SetValue(this.m_Texture);

                effect.CurrentTechnique.Passes[0].Apply();

                m_Device.BlendState = BlendState.AlphaBlend;
                m_Device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
                    PrimitiveType.TriangleList, this.m_Vertices, 0, this.m_VertexCount,
                    this.m_Indices, 0, this.m_IndexCount / 3);

                this.m_VertexCount = 0;
                this.m_IndexCount = 0;
            }
        }

    }
}
