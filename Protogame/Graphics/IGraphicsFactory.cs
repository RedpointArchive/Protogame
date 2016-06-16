using System;
using Microsoft.Xna.Framework.Graphics;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The factory interface which is used to create render passes
    /// before they are added to the render pipeline.
    /// <para>
    /// Use these methods to construct render passes with the appropriate
    /// settings, and pass the resulting value into <see cref="IRenderPipeline.AddFixedRenderPass"/>
    /// or <see cref="IRenderPipeline.AppendTransientRenderPass"/>.
    /// </para>
    /// </summary>
    /// <module>Graphics</module>
    public interface IGraphicsFactory : IGenerateFactory
    {
        /// <summary>
        /// Creates a render pass in which graphics rendering is configured for an
        /// orthographic view.  When this render pass is active, the X and
        /// Y positions of entities map directly to the X and Y positions 
        /// of the game window, with (0, 0) being located in the top-left.
        /// <para>
        /// During this render pass, rendering operations are performed
        /// immediately on the rendering target.  To batch multiple
        /// texture render calls, use an <see cref="I2DBatchedRenderPass" />
        /// instead or in addition to this render pass.
        /// </para>
        /// </summary>
        /// <returns>A 2D render pass where rendering is performed directly.</returns>
        I2DDirectRenderPass Create2DDirectRenderPass();

        /// <summary>
        /// Creates a render pass in which graphics rendering is configured for an
        /// orthographic view.  When this render pass is active, the X and
        /// Y positions of entities map directly to the X and Y positions 
        /// of the game window, with (0, 0) being located in the top-left.
        /// <para>
        /// During this render pass, all texture render calls are batched
        /// together with a <see cref="SpriteBatch" />, and flushed at the
        /// end of the render pass.
        /// </para>
        /// </summary>
        /// <returns>A 2D render pass where rendering is batched together.</returns>
        I2DBatchedRenderPass Create2DBatchedRenderPass();

        /// <summary>
        /// Creates a render pass in which graphics rendering is configured for an
        /// orthographic view, and canvas entities will automatically render
        /// their canvases.  When this render pass is active, the X and
        /// Y positions of entities map directly to the X and Y positions 
        /// of the game window, with (0, 0) being located in the top-left.
        /// <para>
        /// During this render pass, all texture render calls are batched
        /// together with a <see cref="SpriteBatch" />, and flushed at the
        /// end of the render pass.
        /// </para>
        /// <para>
        /// This render pass is identical to <see cref="I2DBatchedRenderPass"/>,
        /// except it is given an explicit interface so that <see cref="CanvasEntity"/>
        /// knows when to render.
        /// </para>
        /// </summary>
        /// <returns>A 2D render pass where canvases are rendered.</returns>
        ICanvasRenderPass CreateCanvasRenderPass();
        
        [Obsolete("Use Create3DForwardRenderPass instead.", true)]
        I3DForwardRenderPass Create3DRenderPass();

        /// <summary>
        /// Creates a render pass in which forward rendering is used.
        /// </summary>
        /// <returns>A 3D render pass.</returns>
        I3DForwardRenderPass Create3DForwardRenderPass();

        /// <summary>
        /// Creates a render pass in which deferred rendering is used.
        /// </summary>
        /// <returns>A 3D render pass.</returns>
        I3DDeferredRenderPass Create3DDeferredRenderPass();

        /// <summary>
        /// Creates a render pass in which calls to <see cref="IDebugRenderer"/> and
        /// the state of physics objects are rendered to the screen.
        /// </summary>
        /// <returns>A debug render pass.</returns>
        IDebugRenderPass CreateDebugRenderPass();

        /// <summary>
        /// Creates a render pass which handles an in-game console.  You need to
        /// include a console render pass if you want custom commands with
        /// <see cref="ICommand"/> to work.
        /// </summary>
        /// <returns>A console render pass.</returns>
        IConsoleRenderPass CreateConsoleRenderPass();

        /// <summary>
        /// Creates a post-processing render pass which inverts all of the
        /// colors on the screen.
        /// </summary>
        /// <returns>A color inversion post-processing render pass.</returns>
        IInvertPostProcessingRenderPass CreateInvertPostProcessingRenderPass();

        /// <summary>
        /// Creates a post-processing render pass which applies a guassian
        /// blur filter to the screen.
        /// </summary>
        /// <returns>A guassian blur post-processing render pass.</returns>
        IBlurPostProcessingRenderPass CreateBlurPostProcessingRenderPass();

        /// <summary>
        /// Creates a post-processing render pass that uses a custom effect (shader).
        /// <para>
        /// This method is a quick way of creating new post-processing render
        /// passes based on custom shaders, without implementing <see cref="IRenderPass"/>.
        /// However, by using <see cref="ICustomPostProcessingRenderPass"/>, you
        /// don't obtain any strongly typed validation of shader usage, so it's
        /// preferable to implement a render pass for each new post-processing
        /// render pass you want to create.
        /// </para>
        /// </summary>
        /// <param name="effectAssetName">The name of the effect asset to use.</param>
        /// <returns>A custom post-processing render pass using the shader you specified.</returns>
        ICustomPostProcessingRenderPass CreateCustomPostProcessingRenderPass(string effectAssetName);

        /// <summary>
        /// Creates a post-processing render pass that uses a custom effect (shader).
        /// <para>
        /// This method is a quick way of creating new post-processing render
        /// passes based on custom shaders, without implementing <see cref="IRenderPass"/>.
        /// However, by using <see cref="ICustomPostProcessingRenderPass"/>, you
        /// don't obtain any strongly typed validation of shader usage, so it's
        /// preferable to implement a render pass for each new post-processing
        /// render pass you want to create.
        /// </para>
        /// </summary>
        /// <param name="effectAsset">The effect asset to use.</param>
        /// <returns>A custom post-processing render pass using the shader you specified.</returns>
        ICustomPostProcessingRenderPass CreateCustomPostProcessingRenderPass(EffectAsset effectAsset);

        /// <summary>
        /// Creates a post-processing render pass that uses a custom effect (shader).
        /// <para>
        /// This method is a quick way of creating new post-processing render
        /// passes based on custom shaders, without implementing <see cref="IRenderPass"/>.
        /// However, by using <see cref="ICustomPostProcessingRenderPass"/>, you
        /// don't obtain any strongly typed validation of shader usage, so it's
        /// preferable to implement a render pass for each new post-processing
        /// render pass you want to create.
        /// </para>
        /// </summary>
        /// <param name="effect">The effect to use.</param>
        /// <returns>A custom post-processing render pass using the shader you specified.</returns>
        ICustomPostProcessingRenderPass CreateCustomPostProcessingRenderPass(Effect effect);

        /// <summary>
        /// Creates a post-processing render pass which captures the current state
        /// of the render pipeline as a separate render target.  This is
        /// more expensive than <see cref="ICaptureInlinePostProcessingRenderPass"/>,
        /// but allows you to access the result at any time between the end of
        /// this render pass, and the begin of this render pass in the next
        /// frame.
        /// </summary>
        ICaptureCopyPostProcessingRenderPass CreateCaptureCopyPostProcessingRenderPass();
        
        /// <summary>
        /// Creates a post-processing render pass which captures the current state
        /// of the render pipeline as a separate render target.  This is
        /// cheaper than <see cref="ICaptureCopyPostProcessingRenderPass"/>, but
        /// you can only access the render target state in the action callback
        /// set on the render pass.  Modifying the render target, e.g. by performing
        /// any rendering at all, will modify the result of the render pipeline.
        /// </summary>
        ICaptureInlinePostProcessingRenderPass CreateCaptureInlinePostProcessingRenderPass();
    }
}

