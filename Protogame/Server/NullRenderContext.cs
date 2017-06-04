using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class NullRenderContext : IRenderContext
    {
        public BoundingFrustum BoundingFrustum { get; }
        public IEffect Effect { get; }
        public GraphicsDevice GraphicsDevice { get; }
        public bool Is3DContext { get; set; }
        public bool IsRendering { get; set; }
        public Vector3 CameraPosition { get; set; }
        public Vector3 CameraLookAt { get; set; }
        public Matrix Projection { get; set; }
        public Texture2D SingleWhitePixel { get; }
        public SpriteBatch SpriteBatch { get; }
        public Matrix View { get; set; }
        public Matrix World { get; set; }
        public void EnableTextures()
        {
            throw new NotSupportedException();
        }

        public void EnableVertexColors()
        {
            throw new NotSupportedException();
        }

        public IEffect PopEffect()
        {
            throw new NotSupportedException();
        }

        public void PopRenderTarget()
        {
            throw new NotSupportedException();
        }

        public void PushEffect(IEffect effect)
        {
            throw new NotSupportedException();
        }

        public void PushRenderTarget(RenderTargetBinding renderTarget)
        {
            throw new NotSupportedException();
        }

        public void PushRenderTarget(params RenderTargetBinding[] renderTargets)
        {
            throw new NotSupportedException();
        }

        public void Render(IGameContext context)
        {
            throw new NotSupportedException();
        }

        public void PostPreRender(IGameContext context)
        {
            throw new NotSupportedException();
        }

        public void SetActiveTexture(Texture2D texture)
        {
            throw new NotSupportedException();
        }

        public IRenderPass AddFixedRenderPass(IRenderPass renderPass)
        {
            throw new NotSupportedException();
        }

        public void RemoveFixedRenderPass(IRenderPass renderPass)
        {
            throw new NotSupportedException();
        }

        public IRenderPass AppendTransientRenderPass(IRenderPass renderPass)
        {
            throw new NotSupportedException();
        }

        public IRenderPass CurrentRenderPass { get; }
        public bool IsCurrentRenderPass<T>() where T : class, IRenderPass
        {
            throw new NotSupportedException();
        }

        public bool IsCurrentRenderPass<T>(out T currentRenderPass) where T : class, IRenderPass
        {
            throw new NotSupportedException();
        }

        public T GetCurrentRenderPass<T>() where T : class, IRenderPass
        {
            throw new NotSupportedException();
        }

        public bool IsFirstRenderPass()
        {
            throw new NotSupportedException();
        }
    }
}