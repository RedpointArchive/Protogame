using System;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// A render pass represents an evaluation of all entities within the
    /// render pipeline, or a post-processing render pass which applies to
    /// the rendered image of the game.
    /// </summary>
    /// <module>Graphics</module>
    public interface IRenderPass
    {
        /// <summary>
        /// Gets a value indicating whether this render pass applies a post-processing effect.
        /// <para>
        /// A standard (non-post-processing) render pass calls a Render method for all entities in the
        /// world, as well as RenderBelow and RenderAbove on the current world.
        /// </para>
        /// <para>
        /// If there are no post-processing render passes configured, then all standard render passes
        /// perform draw calls directly to the backbuffer.
        /// </para>
        /// <para>
        /// If there are post-processing render passes configured, then all standard render passes
        /// perform draw calls to an internal render target, which is used as the texture when rendering
        /// the screen triangles for the first post-processing pass.
        /// </para>
        /// <para>
        /// The result of each post-processing pass is directed to a render target, which is used as the
        /// texture for the next post-processing pass.  The exception is that the last post-processing
        /// render pass will draw to the back-buffer instead of an internal render target.
        /// </para>
        /// <para>
        /// When render passes are added or appended to the <see cref="IRenderContext"/>, the engine ensures
        /// that all post-processing render passes occur after all standard render passes.
        /// </para>
        /// </summary>
        /// <value><c>true</c> if this render pass is a post-processing render pass; otherwise, <c>false</c>.</value>
        bool IsPostProcessingPass { get; }

        /// <summary>
        /// Sets the technique name that should be used when effects are used within this render pass.
        /// Effects can have multiple techniques, each with different names.  When effects are pushed onto the 
        /// render context, the technique that matches the name requested by the render pass is the one
        /// selected for the effect.  This allows effects to be written that support both forward and
        /// deferred rendering, which supply different techniques for each.
        /// </summary>
        string EffectTechniqueName { get; }

        /// <summary>
        /// Begins the render pass.
        /// <para>
        /// During this method, the render pass implementation will configure the
        /// <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/> (which is available via
        /// <see cref="IRenderContext"/>) so that the correct shader and graphics settings are
        /// configured.  Before this method is called, the game engine will set up any render
        /// targets that are required for the render pipeline to operate.
        /// </para>
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="previousPass">
        ///     The previous render pass, or null if this is the first pass in the pipeline.
        /// </param>
        /// <param name="postProcessingSource">
        ///     If this is a post-processing render pass, this argument is set to the source texture
        ///     that is used as input for the shader.  As a general guide, you should pass this
        ///     texture as the source parameter to the <see cref="IGraphicsBlit.Blit"/> if you are
        ///     using that API.
        ///     <para>
        ///         If this is a standard render pass, this argument is always null.
        ///     </para>
        /// </param>
        void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource);

        /// <summary>
        /// Ends the render pass.
        /// <para>
        /// During this method, the render pass implementation will perform any remaining operations
        /// that need to occur before the next render pass runs.  It is not required that a render pass
        /// configure the graphics device back to it's original state; it is expected that each new render
        /// pass will configure all of the appropriate settings of the 
        /// <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/> when it runs.
        /// </para>
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="nextPass">
        ///     The next render pass, or null if this is the last pass in the pipeline.
        /// </param>
        void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass);
    }
}
