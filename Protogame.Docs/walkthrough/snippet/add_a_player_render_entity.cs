using System;
using Microsoft.Xna.Framework;
using Protogame;

namespace MyProject
{
    public class PlayerEntity : IEntity
    {
        // This is the field where we keep a reference to the 2D render utilities.
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        // This is the field where we keep a reference to the player texture we will render.
        private readonly TextureAsset m_PlayerTexture;

        // This is the player constructor.  Both parameters are automatically dependency
        // injected when we call CreatePlayerEntity on the entity factory.
        public PlayerEntity(
            I2DRenderUtilities twodRenderUtilities,
            IAssetManagerProvider assetManagerProvider)
        {
            // Keep the 2D render utilities around for later.
            this.m_2DRenderUtilities = twodRenderUtilities;

            // Some implementations might assign the asset manager to a field, depending on
            // whether or not they need to look up assets during the update or render
            // loops.  In this case we just need access to one texture, so we just keep
            // it in a local variable for easy access.
            var assetManager = assetManagerProvider.GetAssetManager();

            // Retrieve the player texture.
            this.m_PlayerTexture = assetManager.Get<TextureAsset>("texture.Player");
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            // This check is not strictly required when using a 2D world manager, but it
            // is recommended in case you want to change to a 3D world manager later on
            // down the track.
            //
            // You can not use I2DRenderUtilities when the render context is 3D (and
            // vica-versa; you can not use I3DRenderUtilities when the render context
            // is 2D).
            if (renderContext.Is3DContext)
            {
                return;
            }

            // Render the texture at our current position.
            this.m_2DRenderUtilities.RenderTexture(
                renderContext,
                new Vector2(this.X, this.Y),
                this.m_PlayerTexture);
        }
    }
}