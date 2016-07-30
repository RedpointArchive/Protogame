using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="I3DRenderUtilities"/>.
    /// </summary>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.I3DRenderUtilities</interface_ref>
    public class Default3DRenderUtilities : I3DRenderUtilities
    {
        private readonly I2DRenderUtilities _twoDimensionalRenderUtilities;
        private readonly IRenderCache _renderCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="Default3DRenderUtilities"/> class.
        /// </summary>
        /// <param name="renderUtilities">
        /// The _2 d render utilities.
        /// </param>
        public Default3DRenderUtilities(I2DRenderUtilities renderUtilities, IRenderCache renderCache)
        {
            _twoDimensionalRenderUtilities = renderUtilities;
            _renderCache = renderCache;
        }

        /// <summary>
        /// The measure text.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <returns>
        /// The <see cref="Vector2"/>.
        /// </returns>
        public Vector2 MeasureText(IRenderContext context, string text, FontAsset font)
        {
            return _twoDimensionalRenderUtilities.MeasureText(context, text, font);
        }

        /// <summary>
        /// The render line.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public void RenderLine(IRenderContext context, Vector3 start, Vector3 end, Color color)
        {
            if (!context.IsCurrentRenderPass<I3DRenderPass>())
            {
                throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            }

            context.EnableVertexColors();

            var vertexes = _renderCache.GetOrSet(
                "renderline3dvb:" + start + ":" + end + ":" + color,
                () =>
                {
                    var vb = new VertexBuffer(context.GraphicsDevice, VertexPositionColor.VertexDeclaration, 2,
                        BufferUsage.WriteOnly);
                    vb.SetData(new[] { new VertexPositionColor(start, color), new VertexPositionColor(end, color) });
                    return vb;
                });
            var indicies = _renderCache.GetOrSet(
                "renderline3dib",
                () =>
                {
                    var ib = new IndexBuffer(context.GraphicsDevice, IndexElementSize.SixteenBits, 2,
                        BufferUsage.WriteOnly);
                    ib.SetData(new short[] { 0, 1 });
                    return ib;
                });

            context.GraphicsDevice.SetVertexBuffer(vertexes);
            context.GraphicsDevice.Indices = indicies;

            foreach (var pass in context.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                context.GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 1);
            }
        }

        /// <summary>
        /// Renders a 3D line using texture UVs.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        /// <param name="start">
        /// The start of the line.
        /// </param>
        /// <param name="end">
        /// The end of the line.
        /// </param>
        /// <param name="texture">
        /// The texture to use.
        /// </param>
        /// <param name="startUV">
        /// The UV for the start of the line.
        /// </param>
        /// <param name="endUV">
        /// The UV for the end of the line.
        /// </param> 
        public void RenderLine(
            IRenderContext context,
            Vector3 start, 
            Vector3 end,
            TextureAsset texture,
            Vector2 startUV,
            Vector2 endUV)
        {
            if (!context.IsCurrentRenderPass<I3DRenderPass>())
            {
                throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            }

            context.EnableTextures();
            context.SetActiveTexture(texture.Texture);

            var vertexes = _renderCache.GetOrSet(
                "renderlinetex3dvb:" + start + ":" + end + ":" + startUV + ":" + endUV,
                () =>
                {
                    var vb = new VertexBuffer(context.GraphicsDevice, VertexPositionTexture.VertexDeclaration, 2,
                        BufferUsage.WriteOnly);
                    vb.SetData(new[] { new VertexPositionTexture(start, startUV), new VertexPositionTexture(end, endUV) });
                    return vb;
                });
            var indicies = _renderCache.GetOrSet(
                "renderline3dib",
                () =>
                {
                    var ib = new IndexBuffer(context.GraphicsDevice, IndexElementSize.SixteenBits, 2,
                        BufferUsage.WriteOnly);
                    ib.SetData(new short[] { 0, 1 });
                    return ib;
                });

            context.GraphicsDevice.SetVertexBuffer(vertexes);
            context.GraphicsDevice.Indices = indicies;

            foreach (var pass in context.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                context.GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 1);
            }
        }

        /// <summary>
        /// The render rectangle.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="filled">
        /// The filled.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public void RenderRectangle(
            IRenderContext context, 
            Vector3 start, 
            Vector3 end, 
            Color color, 
            bool filled = false)
        {
            if (!context.IsCurrentRenderPass<I3DRenderPass>())
            {
                throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            }

            var vertexes = new[]
            {
                new VertexPositionColor(start, color), 
                new VertexPositionColor(new Vector3(start.X, end.Y, start.Z), color), 
                new VertexPositionColor(new Vector3(end.X, start.Y, end.Z), color), new VertexPositionColor(end, color)
            };
            var indicies = new short[] { 0, 1, 2, 3 };

            foreach (var pass in context.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                context.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineStrip, 
                    vertexes, 
                    0, 
                    4, 
                    indicies, 
                    0, 
                    3);
            }
        }

        /// <summary>
        /// The render text.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <param name="horizontalAlignment">
        /// The horizontal alignment.
        /// </param>
        /// <param name="verticalAlignment">
        /// The vertical alignment.
        /// </param>
        /// <param name="textColor">
        /// The text color.
        /// </param>
        /// <param name="renderShadow">
        /// The render shadow.
        /// </param>
        /// <param name="shadowColor">
        /// The shadow color.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public void RenderText(
            IRenderContext context, 
            Matrix matrix, 
            string text, 
            FontAsset font, 
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, 
            VerticalAlignment verticalAlignment = VerticalAlignment.Top, 
            Color? textColor = null, 
            bool renderShadow = true, 
            Color? shadowColor = null)
        {
            if (!context.IsCurrentRenderPass<I3DRenderPass>())
            {
                throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            }

            var targets = context.GraphicsDevice.GetRenderTargets();
            var size = MeasureText(context, text, font);

            var temporary = new RenderTarget2D(
                context.GraphicsDevice, 
                (int)size.X, 
                (int)size.Y, 
                false, 
                SurfaceFormat.Color, 
                DepthFormat.None, 
                0, 
                RenderTargetUsage.DiscardContents);
            context.GraphicsDevice.SetRenderTarget(temporary);
            context.GraphicsDevice.Clear(Color.Transparent);

            using (var spriteBatch = new SpriteBatch(context.GraphicsDevice))
            {
                _twoDimensionalRenderUtilities.RenderText(
                    context, 
                    new Vector2(0, 0), 
                    text, 
                    font, 
                    HorizontalAlignment.Left, 
                    VerticalAlignment.Top, 
                    textColor, 
                    renderShadow, 
                    shadowColor);
                spriteBatch.End();
            }

            var texture = (Texture2D)temporary;
            context.GraphicsDevice.SetRenderTarget(null);

            context.GraphicsDevice.BlendState = BlendState.Opaque;
            context.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            context.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            context.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            RenderTexture(context, matrix, texture, Color.White, flipHorizontally: false, flipVertically: false);
        }

        /// <summary>
        /// The render texture.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="texture">
        /// The texture.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="flipHorizontally">
        /// The flip horizontally.
        /// </param>
        /// <param name="flipVertically">
        /// The flip vertically.
        /// </param>
        /// <param name="sourceArea">
        /// The source area.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public void RenderTexture(
            IRenderContext context, 
            Matrix matrix, 
            TextureAsset texture, 
            Color? color = null, 
            bool flipHorizontally = false, 
            bool flipVertically = false, 
            Rectangle? sourceArea = null)
        {
            if (!context.IsCurrentRenderPass<I3DRenderPass>())
            {
                throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            }

            RenderTexture(context, matrix, texture.Texture, color, flipHorizontally, flipVertically, sourceArea);
        }

        /// <summary>
        /// The render texture.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="texture">
        /// The texture.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="flipHorizontally">
        /// The flip horizontally.
        /// </param>
        /// <param name="flipVertically">
        /// The flip vertically.
        /// </param>
        /// <param name="sourceArea">
        /// The source area.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        private void RenderTexture(
            IRenderContext context, 
            Matrix matrix, 
            Texture2D texture, 
            Color? color = null, 
            bool flipHorizontally = false, 
            bool flipVertically = false, 
            Rectangle? sourceArea = null)
        {
            if (color != null)
            {
                throw new NotSupportedException();
            }

            if (flipHorizontally)
            {
                throw new NotSupportedException();
            }

            if (flipVertically)
            {
                throw new NotSupportedException();
            }

            if (sourceArea != null)
            {
                throw new NotSupportedException();
            }

            context.EnableTextures();
            context.SetActiveTexture(texture);

            var vertexes = new[]
            {
                new VertexPositionTexture(Vector3.Transform(new Vector3(0, 0, 0), matrix), new Vector2(0, 1)), 
                new VertexPositionTexture(Vector3.Transform(new Vector3(0, 1, 0), matrix), new Vector2(0, 0)), 
                new VertexPositionTexture(Vector3.Transform(new Vector3(1, 0, 0), matrix), new Vector2(1, 1)), 
                new VertexPositionTexture(Vector3.Transform(new Vector3(1, 1, 0), matrix), new Vector2(1, 0))
            };
            var indicies = new short[] { 1, 3, 0, 2 };

            context.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            foreach (var pass in context.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                context.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleStrip, 
                    vertexes, 
                    0, 
                    vertexes.Length, 
                    indicies, 
                    0, 
                    vertexes.Length - 2);
            }

            context.GraphicsDevice.BlendState = BlendState.Opaque;
        }

        /// <summary>
        /// Renders a 3D cube from 0, 0, 0 to 1, 1, 1, applying the specified transformation.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        /// <param name="transform">
        /// The transformation to apply.
        /// </param>
        /// <param name="color">
        /// The color of the cube.
        /// </param>
        public void RenderCube(IRenderContext context, Matrix transform, Color color)
        {
            if (!context.IsCurrentRenderPass<I3DRenderPass>())
            {
                throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            }
            
            var vertexes = _renderCache.GetOrSet(
                "rendercube3dvb:" + color,
                () =>
                {
                    var vb = new VertexBuffer(context.GraphicsDevice, VertexPositionNormalColor.VertexDeclaration, 24, BufferUsage.WriteOnly);
                    vb.SetData(new[]
                    {
                        new VertexPositionNormalColor(new Vector3(0, 0, 0), new Vector3(-1, 0, 0), color),
                        new VertexPositionNormalColor(new Vector3(0, 0, 1), new Vector3(-1, 0, 0), color),
                        new VertexPositionNormalColor(new Vector3(0, 1, 0), new Vector3(-1, 0, 0), color),
                        new VertexPositionNormalColor(new Vector3(0, 1, 1), new Vector3(-1, 0, 0), color),

                        new VertexPositionNormalColor(new Vector3(1, 0, 0), new Vector3(1, 0, 0), color),
                        new VertexPositionNormalColor(new Vector3(1, 0, 1), new Vector3(1, 0, 0), color),
                        new VertexPositionNormalColor(new Vector3(1, 1, 0), new Vector3(1, 0, 0), color),
                        new VertexPositionNormalColor(new Vector3(1, 1, 1), new Vector3(1, 0, 0), color),

                        new VertexPositionNormalColor(new Vector3(0, 0, 0), new Vector3(0, -1, 0), color),
                        new VertexPositionNormalColor(new Vector3(0, 0, 1), new Vector3(0, -1, 0), color),
                        new VertexPositionNormalColor(new Vector3(0, 1, 0), new Vector3(0, 1, 0), color),
                        new VertexPositionNormalColor(new Vector3(0, 1, 1), new Vector3(0, 1, 0), color),

                        new VertexPositionNormalColor(new Vector3(1, 0, 0), new Vector3(0, -1, 0), color),
                        new VertexPositionNormalColor(new Vector3(1, 0, 1), new Vector3(0, -1, 0), color),
                        new VertexPositionNormalColor(new Vector3(1, 1, 0), new Vector3(0, 1, 0), color),
                        new VertexPositionNormalColor(new Vector3(1, 1, 1), new Vector3(0, 1, 0), color),

                        new VertexPositionNormalColor(new Vector3(0, 0, 0), new Vector3(0, 0, -1), color),
                        new VertexPositionNormalColor(new Vector3(0, 0, 1), new Vector3(0, 0, 1), color),
                        new VertexPositionNormalColor(new Vector3(0, 1, 0), new Vector3(0, 0, -1), color),
                        new VertexPositionNormalColor(new Vector3(0, 1, 1), new Vector3(0, 0, 1), color),

                        new VertexPositionNormalColor(new Vector3(1, 0, 0), new Vector3(0, 0, -1), color),
                        new VertexPositionNormalColor(new Vector3(1, 0, 1), new Vector3(0, 0, 1), color),
                        new VertexPositionNormalColor(new Vector3(1, 1, 0), new Vector3(0, 0, -1), color),
                        new VertexPositionNormalColor(new Vector3(1, 1, 1), new Vector3(0, 0, 1), color)
                    });
                    return vb;
                });
            var indicies = _renderCache.GetOrSet(
                "rendercube3dib",
                () =>
                {
                    var ib = new IndexBuffer(context.GraphicsDevice, IndexElementSize.SixteenBits, 36, BufferUsage.WriteOnly);
                    ib.SetData(new short[]
                    {
                        0, 2, 1,
                        3, 1, 2,

                        4, 5, 6,
                        7, 6, 5,

                        0 + 8, 1 + 8, 4 + 8,
                        5 + 8, 4 + 8, 1 + 8,

                        2 + 8, 6 + 8, 3 + 8,
                        7 + 8, 3 + 8, 6 + 8,

                        0 + 16, 4 + 16, 2 + 16,
                        6 + 16, 2 + 16, 4 + 16,

                        1 + 16, 3 + 16, 5 + 16,
                        7 + 16, 5 + 16, 3 + 16
                    });
                    return ib;
                });

            context.EnableVertexColors();

            var world = context.World;

            context.World = transform;
            context.GraphicsDevice.SetVertexBuffer(vertexes);
            context.GraphicsDevice.Indices = indicies;

            foreach (var pass in context.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                context.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    indicies.IndexCount / 3);
            }

            context.World = world;
        }

        /// <summary>
        /// Renders a 3D cube from 0, 0, 0 to 1, 1, 1, applying the specified transformation, with the
        /// given texture and using the specified UV coordinates for each face of the cube.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        /// <param name="transform">
        /// The transformation to apply.
        /// </param>
        /// <param name="texture">
        /// The texture to render on the cube.
        /// </param>
        /// <param name="topLeftUV">
        /// The top-left UV coordinate.
        /// </param>
        /// <param name="bottomRightUV">
        /// The bottom-right UV coordinate.
        /// </param>
        public void RenderCube(IRenderContext context, Matrix transform, TextureAsset texture, Vector2 topLeftUV, Vector2 bottomRightUV)
        {
            if (!context.IsCurrentRenderPass<I3DRenderPass>())
            {
                throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            }

            var vertexes = new[]
            {
                new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(-1, 0, 0), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 0, 1), new Vector3(-1, 0, 0), new Vector2(topLeftUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 1, 0), new Vector3(-1, 0, 0), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 1, 1), new Vector3(-1, 0, 0), new Vector2(bottomRightUV.X, bottomRightUV.Y)),

                new VertexPositionNormalTexture(new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector2(topLeftUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 1, 0), new Vector3(1, 0, 0), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 1, 1), new Vector3(1, 0, 0), new Vector2(bottomRightUV.X, bottomRightUV.Y)),

                new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, -1, 0), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 0, 1), new Vector3(0, -1, 0), new Vector2(topLeftUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 1, 1), new Vector3(0, 1, 0), new Vector2(topLeftUV.X, bottomRightUV.Y)),

                new VertexPositionNormalTexture(new Vector3(1, 0, 0), new Vector3(0, -1, 0), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 0, 1), new Vector3(0, -1, 0), new Vector2(bottomRightUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 1, 0), new Vector3(0, 1, 0), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 1, 1), new Vector3(0, 1, 0), new Vector2(bottomRightUV.X, bottomRightUV.Y)),

                new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 0, -1), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 1, 0), new Vector3(0, 0, -1), new Vector2(topLeftUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 1, 1), new Vector3(0, 0, 1), new Vector2(topLeftUV.X, bottomRightUV.Y)),

                new VertexPositionNormalTexture(new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 1, 0), new Vector3(0, 0, -1), new Vector2(bottomRightUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 1, 1), new Vector3(0, 0, 1), new Vector2(bottomRightUV.X, bottomRightUV.Y))
            };

            var indicies = new short[]
            {
                0, 2, 1,
                3, 1, 2,

                4, 5, 6, 
                7, 6, 5,

                0 + 8, 1 + 8, 4 + 8,
                5 + 8, 4 + 8, 1 + 8,

                2 + 8, 6 + 8, 3 + 8,
                7 + 8, 3 + 8, 6 + 8,

                0 + 16, 4 + 16, 2 + 16,
                6 + 16, 2 + 16, 4 + 16,

                1 + 16, 3 + 16, 5 + 16,
                7 + 16, 5 + 16, 3 + 16
            };

            context.EnableTextures();
            context.SetActiveTexture(texture.Texture);

            var world = context.World;

            context.World = transform;

            foreach (var pass in context.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                context.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    vertexes,
                    0,
                    vertexes.Length,
                    indicies,
                    0,
                    indicies.Length / 3);
            }

            context.World = world;
        }

        /// <summary>
        /// Renders a 2D plane from 0, 0, 0 to 1, 0, 1, applying the specified transformation.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        /// <param name="transform">
        /// The transformation to apply.
        /// </param>
        /// <param name="color">
        /// The color of the plane.
        /// </param>
        public void RenderPlane(IRenderContext context, Matrix transform, Color color)
        {
            if (!context.IsCurrentRenderPass<I3DRenderPass>())
            {
                throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            }

            var vertexes = new[]
            {
                new VertexPositionColor(new Vector3(0, 0, 0), color),
                new VertexPositionColor(new Vector3(0, 0, 1), color),
                new VertexPositionColor(new Vector3(1, 0, 0), color),
                new VertexPositionColor(new Vector3(1, 0, 1), color)
            };

            var indicies = new short[]
            {
                0, 2, 1,
                3, 1, 2
            };

            context.EnableVertexColors();

            var world = context.World;

            context.World = transform;

            foreach (var pass in context.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                context.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    vertexes,
                    0,
                    vertexes.Length,
                    indicies,
                    0,
                    indicies.Length / 3);
            }

            context.World = world;
        }

        public void RenderPlane(IRenderContext context, Matrix transform, TextureAsset texture, Vector2 topLeftUV,
            Vector2 bottomRightUV)
        {
            if (!context.IsCurrentRenderPass<I3DRenderPass>())
            {
                throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            }

            var vertexes = new[]
            {
                new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, -1, 0), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 0, 1), new Vector3(0, -1, 0), new Vector2(topLeftUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 0, 0), new Vector3(0, -1, 0), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 0, 1), new Vector3(0, -1, 0), new Vector2(bottomRightUV.X, bottomRightUV.Y))
            };

            var indicies = new short[]
            {
                0, 2, 1,
                3, 1, 2
            };

            context.EnableTextures();
            context.SetActiveTexture(texture.Texture);

            var world = context.World;

            context.World = transform;

            foreach (var pass in context.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                context.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    vertexes,
                    0,
                    vertexes.Length,
                    indicies,
                    0,
                    indicies.Length / 3);
            }

            context.World = world;
        }

        public void RenderCircle(IRenderContext context,
            Matrix transform, Vector2 center, int radius, Color color, bool filled = false)
        {
            if (!context.IsCurrentRenderPass<I3DRenderPass>())
            {
                throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            }

            var points = 8;

            double angle = MathHelper.TwoPi / points;

            var vertexesList = new List<VertexPositionColor>();
            var indicesList = new List<short>();

            vertexesList.Add(new VertexPositionColor(new Vector3(center, 0), color));

            for (int i = 1; i <= points; i++)
            {
                var pos = new Vector2(
                    (float)Math.Round(Math.Sin(angle * i), 4) * radius,
                    (float)Math.Round(Math.Cos(angle * i), 4) * radius);
                
                vertexesList.Add(new VertexPositionColor(new Vector3(center + pos, 0), color));
            }

            if (filled)
            {
                for (int i = 1; i < points; i++)
                {
                    indicesList.Add(0);
                    indicesList.Add((short)(i + 1));
                    indicesList.Add((short) i);
                }
                indicesList.Add(0);
                indicesList.Add(1);
                indicesList.Add((short) points);
            }
            else
            {
                for (int i = 1; i <= points; i++)
                {
                    indicesList.Add((short) i);
                }
                indicesList.Add(1);
            }

            var vertexes = vertexesList.ToArray();
            var indicies = indicesList.ToArray();

            context.EnableVertexColors();

            var world = context.World;

            context.World = transform;

            foreach (var pass in context.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                context.GraphicsDevice.DrawUserIndexedPrimitives(
                    filled ? PrimitiveType.TriangleList : PrimitiveType.LineStrip,
                    vertexes,
                    0,
                    vertexes.Length,
                    indicies,
                    0,
                    indicies.Length / 3);
            }

            context.World = world;
        }
    }
}