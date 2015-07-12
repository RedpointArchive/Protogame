using System;
using Protogame;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ninject;

namespace ProtogameEditor
{
    public class EditorWorld : IWorld
    {
        private readonly I2DRenderUtilities _2DrenderUtilities;

        private readonly I3DRenderUtilities _renderUtilities;

        private readonly IAssetManager _assetManager;

        private readonly FontAsset _defaultFont;

        private int _count;

        private bool _clicked;

        public EditorWorld(
            IKernel kernel,
            I2DRenderUtilities twodRenderUtilities,
            I3DRenderUtilities renderUtilities,
            IAssetManagerProvider assetManagerProvider)
        {
            this.Entities = new List<IEntity>();
            this._2DrenderUtilities = twodRenderUtilities;
            this._renderUtilities = renderUtilities;
            this._assetManager = assetManagerProvider.GetAssetManager();
            this._defaultFont = this._assetManager.Get<FontAsset>("font.Default");
            this._count = 0;

            var entity = kernel.Get<CubeEntity>();
            entity.X = 3.05f;
            entity.Y = 0;
            entity.Z = 2.1f;
            this.Entities.Add(entity);
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                renderContext.GraphicsDevice.Clear(new Color(63, 63, 63));

                var pp = renderContext.GraphicsDevice.PresentationParameters;
                renderContext.Projection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    pp.BackBufferWidth / (float)pp.BackBufferHeight,
                    1f,
                    5000f);
                renderContext.View = Matrix.CreateLookAt(
                    new Vector3(10, 10, 10),
                    Vector3.Zero,
                    Vector3.Up);
                renderContext.World = Matrix.Identity;

                var size = 15;

                if (this._clicked)
                {
                    size = 30;
                }

                for (var x = -size; x <= size; x++)
                {
                    _renderUtilities.RenderLine(
                        renderContext,
                        new Vector3(x, 0, -size),
                        new Vector3(x, 0, size),
                        new Color(127, 127, 127));
                }

                for (var z = -size; z <= size; z++)
                {
                    _renderUtilities.RenderLine(
                        renderContext,
                        new Vector3(-size, 0, z),
                        new Vector3(size, 0, z),
                        new Color(127, 127, 127));
                }
            }
            else
            {
                this._2DrenderUtilities.RenderText(
                    renderContext,
                    new Vector2(50, 50),
                    "test " + this._count + " " + Mouse.GetState().X + " " + Mouse.GetState().Y,
                    this._defaultFont);
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this._count++;

            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                this._clicked = true;
            }
        }

        public IList<IEntity> Entities
        {
            get;
            private set;
        }

        public void Dispose()
        {
        }
    }
}

