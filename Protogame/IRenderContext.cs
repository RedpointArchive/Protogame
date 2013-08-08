using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IRenderContext
    {
        /// <summary>
        /// The associated graphics device.
        /// </summary>
        GraphicsDevice GraphicsDevice { get; }
        
        /// <summary>
        /// A sprite batch associated with the current device, upon which 2D rendering is performed.
        /// </summary>
        SpriteBatch SpriteBatch { get; }
        
        /// <summary>
        /// A texture representing a single white pixel.
        /// </summary>
        Texture2D SingleWhitePixel { get; }
        
        /// <summary>
        /// The current active effect for rendering.
        /// </summary>
        Effect Effect { get; }
        
        /// <summary>
        /// The bounding frustrum for the current view and projection matrixes.
        /// </summary>
        /// <value>The bounding frustrum.</value>
        BoundingFrustum BoundingFrustrum { get; }
        
        /// <summary>
        /// The view matrix for 3D rendering.
        /// </summary>
        Matrix View { get; set; }
        
        /// <summary>
        /// The world matrix for 3D rendering.
        /// </summary>
        Matrix World { get; set; }
        
        /// <summary>
        /// The projection matrix for 3D rendering.
        /// </summary>
        Matrix Projection { get; set; }
        
        /// <summary>
        /// Whether the world manager is currently rendering a 3D context.  For games using a 2D
        /// world manager, this will always be false.  For 3D games, world managers will do an
        /// initial 3D pass followed by a 2D pass that is rendered on top (for UI, etc.)
        ///
        /// In a 3D context, only I3DRenderUtilities can be used; in a 2D context, only
        /// I2DRenderUtilities can be used.
        /// </summary>
        bool Is3DContext { get; set; }
        
        /// <summary>
        /// Called by the world manager to set up the render context at the beginning of a render.
        /// </summary>
        /// <param name="context">The current game context.</param>
        void Render(IGameContext context);
        
        /// <summary>
        /// Enables rendering with textures for the current effect.
        /// </summary>
        void EnableTextures();
        
        /// <summary>
        /// Enables rendering with plain colours for the current effect.
        /// </summary>
        void EnableVertexColors();
        
        /// <summary>
        /// Set the active texture used in rendering.
        /// </summary>
        /// <param name="texture">The active texture.</param>
        void SetActiveTexture(Texture2D texture);
    }
}

