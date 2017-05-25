// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="I3DRenderUtilities"/>.
    /// </summary>
    /// <module>Graphics 3D</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.I3DRenderUtilities</interface_ref>
    public class Default3DRenderUtilities : I3DRenderUtilities
    {
        private readonly I2DRenderUtilities _twoDimensionalRenderUtilities;
        private readonly IRenderCache _renderCache;
        private readonly IRenderBatcher _renderBatcher;

        public Default3DRenderUtilities(I2DRenderUtilities renderUtilities, IRenderCache renderCache, IRenderBatcher renderBatcher)
        {
            _twoDimensionalRenderUtilities = renderUtilities;
            _renderCache = renderCache;
            _renderBatcher = renderBatcher;
        }
        
        public Vector2 MeasureText(IRenderContext context, string text, IAssetReference<FontAsset> font)
        {
            return _twoDimensionalRenderUtilities.MeasureText(context, text, font);
        }
        
        public void RenderLine(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Vector3 start, Vector3 end, Color color)
        {
            if (!context.IsCurrentRenderPass<I3DRenderPass>())
            {
                throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            }

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

            effect.LoadParameterSet(context, effectParameterSet);
            foreach (var pass in effect.NativeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                context.GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 1);
            }
        }

        public void RenderLine(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Vector3 start, Vector3 end, TextureAsset texture, Vector2 startUV, Vector2 endUV)
        {
            if (!context.IsCurrentRenderPass<I3DRenderPass>())
            {
                throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            }

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

            var semantic = effectParameterSet.GetSemantic<ITextureEffectSemantic>();
            if (semantic.Texture != texture.Texture)
            {
                semantic.Texture = texture.Texture;
            }

            effect.LoadParameterSet(context, effectParameterSet);
            foreach (var pass in effect.NativeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                context.GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 1);
            }
        }
        
        public void RenderRectangle(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Vector3 start, Vector3 end, Color color, bool filled = false)
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

            effect.LoadParameterSet(context, effectParameterSet);
            foreach (var pass in effect.NativeEffect.CurrentTechnique.Passes)
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
        
        public void RenderText(
            IRenderContext context,
            IEffect effect, 
            IEffectParameterSet effectParameterSet,
            Matrix matrix, 
            string text,
            IAssetReference<FontAsset> font, 
            HorizontalAlignment horizontalAlignment, 
            VerticalAlignment verticalAlignment,
            Color? textColor, 
            bool renderShadow,
            Color? shadowColor)
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

            RenderTexture(context, effect, effectParameterSet, matrix, texture, Color.White, flipHorizontally: false, flipVertically: false);
        }
        
        public void RenderTexture(
            IRenderContext context,
            IEffect effect, 
            IEffectParameterSet effectParameterSet, 
            Matrix matrix, 
            IAssetReference<TextureAsset> texture,
            Color? color, 
            bool flipHorizontally, 
            bool flipVertically, 
            Rectangle? sourceArea)
        {
            if (!context.IsCurrentRenderPass<I3DRenderPass>())
            {
                throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            }

            if (!texture.IsReady)
            {
                return;
            }

            RenderTexture(context, effect, effectParameterSet, matrix, texture.Asset.Texture, color, flipHorizontally, flipVertically, sourceArea);
        }
        
        private void RenderTexture(
            IRenderContext context,
            IEffect effect, 
            IEffectParameterSet effectParameterSet, 
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

            var semantic = effectParameterSet.GetSemantic<ITextureEffectSemantic>();
            if (semantic.Texture != texture)
            {
                semantic.Texture = texture;
            }

            var vertexes = new[]
            {
                new VertexPositionTexture(Vector3.Transform(new Vector3(0, 0, 0), matrix), new Vector2(0, 1)), 
                new VertexPositionTexture(Vector3.Transform(new Vector3(0, 1, 0), matrix), new Vector2(0, 0)), 
                new VertexPositionTexture(Vector3.Transform(new Vector3(1, 0, 0), matrix), new Vector2(1, 1)), 
                new VertexPositionTexture(Vector3.Transform(new Vector3(1, 1, 0), matrix), new Vector2(1, 0))
            };
            var indicies = new short[] { 1, 3, 0, 2 };

            context.GraphicsDevice.BlendState = BlendState.NonPremultiplied;

            effect.LoadParameterSet(context, effectParameterSet);
            foreach (var pass in effect.NativeEffect.CurrentTechnique.Passes)
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
        
        public void RenderCube(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix transform, Color color)
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

            _renderBatcher.QueueRequest(
                context,
                _renderBatcher.CreateSingleRequestFromState(
                    context,
                    effect,
                    effectParameterSet,
                    vertexes,
                    indicies,
                    PrimitiveType.TriangleList,
                    transform, 
                    null,
                    null));
        }
        
        public void RenderCube(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix transform, TextureAsset texture, Vector2 topLeftUV, Vector2 bottomRightUV)
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

            var semantic = effectParameterSet.GetSemantic<ITextureEffectSemantic>();
            if (semantic.Texture != texture.Texture)
            {
                semantic.Texture = texture.Texture;
            }

            var world = context.World;

            context.World = transform;

            effect.LoadParameterSet(context, effectParameterSet);
            foreach (var pass in effect.NativeEffect.CurrentTechnique.Passes)
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
        
        public void RenderPlane(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix transform, Color color)
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

            var world = context.World;

            context.World = transform;

            effect.LoadParameterSet(context, effectParameterSet);
            foreach (var pass in effect.NativeEffect.CurrentTechnique.Passes)
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

        public void RenderPlane(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix transform, TextureAsset texture, Vector2 topLeftUV, Vector2 bottomRightUV)
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

            var semantic = effectParameterSet.GetSemantic<ITextureEffectSemantic>();
            if (semantic.Texture != texture.Texture)
            {
                semantic.Texture = texture.Texture;
            }

            var world = context.World;

            context.World = transform;

            effect.LoadParameterSet(context, effectParameterSet);
            foreach (var pass in effect.NativeEffect.CurrentTechnique.Passes)
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

        public void RenderCircle(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix transform, Vector2 center, int radius, Color color, bool filled = false)
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

            var world = context.World;

            context.World = transform;

            effect.LoadParameterSet(context, effectParameterSet);
            foreach (var pass in effect.NativeEffect.CurrentTechnique.Passes)
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