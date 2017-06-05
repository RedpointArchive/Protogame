using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    /// <summary>
    /// The implementation of <see cref="IRenderContext" /> which provides additional APIs
    /// for interacting with the render pipeline.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IRenderContext</interface_ref>
    public class RenderPipelineRenderContext : IRenderContext
    {
        /// <summary>
        /// The rendering pipeline.
        /// </summary>
        private readonly IRenderPipeline _renderPipeline;
        
        private readonly IEffectSemantic[] _availableSemantics;
        
        /// <summary>
        /// The current stack of render targets.
        /// </summary>
        private readonly Stack<RenderTargetBinding[]> m_RenderTargets = new Stack<RenderTargetBinding[]>();

        /// <summary>
        /// The current bounding frustum.
        /// </summary>
        private BoundingFrustum m_BoundingFrustum;

        /// <summary>
        /// Stores the last camera position.
        /// </summary>
        private Vector3? _lastCameraPosition;

        /// <summary>
        /// Stores the last camera look at vector.
        /// </summary>
        private Vector3? _lastCameraLookAt;

        private Matrix _globalView;
        private Matrix _globalProjection;
        private Matrix _globalWorld;

        /// <summary>
        /// Initializes a new instance of the <see cref="Protogame.RenderPipelineRenderContext"/> class.
        /// </summary>
        /// <param name="renderPipeline">The render pipeline shared with the <see cref="RenderPipelineWorldManager"/> instance.</param>
        /// <param name="availableSemantics">The available effect semantics in the game.</param>
        public RenderPipelineRenderContext(
            IRenderPipeline renderPipeline,
            IEffectSemantic[] availableSemantics)
        {
            _renderPipeline = renderPipeline;
            _availableSemantics = availableSemantics;
        }

        /// <summary>
        /// Gets the bounding frustum for the current view and projection matrixes.
        /// </summary>
        /// <value>
        /// The bounding frustum for the current view and projection matrixes.
        /// </value>
        public BoundingFrustum BoundingFrustum
        {
            get
            {
                return this.m_BoundingFrustum;
            }
        }

        /// <summary>
        /// Gets the associated graphics device.
        /// </summary>
        /// <value>
        /// The graphics device.
        /// </value>
        public GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the world manager is currently
        /// rendering a 3D context.  For games using a 2D world manager, this will
        /// always be false.  For 3D games, world managers will do an initial 3D pass
        /// followed by a 2D pass that is rendered on top (for UI, etc.) In a 3D
        /// context, only I3DRenderUtilities can be used; in a 2D context, only
        /// I2DRenderUtilities can be used.
        /// </summary>
        /// <value>
        /// Whether the rendering context is currently a 3D context.
        /// </value>
        public bool Is3DContext { get; set; }

        /// <summary>
        /// Gets a value indicating whether the game is currently rendering
        /// in either the render pipeline or the backbuffer.  This value will always
        /// be true within any method call that occurs below <see cref="Game.Draw"/>
        /// in the call stack.  When you are rendering, certain operations can not
        /// be performed, in particular, operations which reset the graphics device
        /// like resizing the game window.
        /// </summary>
        public bool IsRendering { get; set; }

        /// <summary>
        /// Gets or sets the last known camera position.  The value of this property is
        /// set internally by cameras so that the camera position is known when lighting
        /// effects are applied.  Setting this property from user code will not actually
        /// update the camera position or modify projection parameters; it will only
        /// impact the way lights are rendered.
        /// </summary>
        public Vector3 CameraPosition
        {
            get
            {
                if (_lastCameraPosition == null)
                {
                    throw new InvalidOperationException(
                        "The camera position has not been set, which means this value has no " +
                        "retrievable value.  Either use one of the builtin cameras (such as " +
                        "IFirstPersonCamera), or set this property when you are configuring " +
                        "the view matrix.");
                }

                return _lastCameraPosition.Value;
            }
            set
            {
                _lastCameraPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets the last known camera look at vector.  The value of this property is
        /// set internally by cameras so that the camera look at vector is known when lighting
        /// effects are applied.  Setting this property from user code will not actually
        /// update the camera look at vector or modify projection parameters; it will only
        /// impact the way lights are rendered.
        /// </summary>
        public Vector3 CameraLookAt
        {
            get
            {
                if (_lastCameraPosition == null)
                {
                    throw new InvalidOperationException(
                        "The camera look at vector has not been set, which means this value has no " +
                        "retrievable value.  Either use one of the builtin cameras (such as " +
                        "IFirstPersonCamera), or set this property when you are configuring " +
                        "the view matrix.");
                }

                return _lastCameraPosition.Value;
            }
            set
            {
                _lastCameraPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets the projection matrix for 3D rendering.
        /// </summary>
        /// <value>
        /// The projection matrix for 3D rendering.
        /// </value>
        public Matrix Projection
        {
            get { return _globalProjection; }
            set
            {
                _globalProjection = value;
                this.RecalculateBoundingFrustum();
            }
        }

        /// <summary>
        /// Gets a texture representing a single white pixel.
        /// </summary>
        /// <value>
        /// The single white pixel.
        /// </value>
        public Texture2D SingleWhitePixel { get; private set; }

        /// <summary>
        /// Gets a sprite batch associated with the current device, upon which 2D rendering is performed.
        /// </summary>
        /// <value>
        /// The sprite batch.
        /// </value>
        public SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// Gets or sets the view matrix for 3D rendering.
        /// </summary>
        /// <value>
        /// The view matrix for 3D rendering.
        /// </value>
        public Matrix View
        {
            get { return _globalView; }
            set
            {
                _globalView = value;
                this.RecalculateBoundingFrustum();
            }
        }

        /// <summary>
        /// Gets or sets the world matrix for 3D rendering.
        /// </summary>
        /// <value>
        /// The world matrix for 3D rendering.
        /// </value>
        public Matrix World
        {
            get { return _globalWorld; }
            set { _globalWorld = value; }
        }
        
        /// <summary>
        /// Pops the current render target from the current rendering context.  If there are no more render targets
        /// in the stack after this call, then the rendering will default back to rendering to the back buffer.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if the current render target does not match the top of the stack.  This indicates that there
        /// is other code calling SetRenderTarget, and changing the render target with this method may corrupt
        /// the rendering state.
        /// </exception>
        public void PopRenderTarget()
        {
            if (this.m_RenderTargets.Count == 0)
            {
                return;
            }

            var expectedTargets = this.m_RenderTargets.Peek();
            var currentTargets = this.GraphicsDevice.GetRenderTargets();
            if (currentTargets.Length != expectedTargets.Length)
            {
                throw new InvalidOperationException(
                    "Current render targets do not match last render targets "
                    + "pushed onto the stack with PushRenderTarget.  Ensure there "
                    + "is no code manually calling SetRenderTarget and not restoring "
                    + "it at the end of it's execution.");
            }

            for (var i = 0; i < currentTargets.Length; i++)
            {
                var expected = expectedTargets[i];
                var current = currentTargets[i];

                if (current.RenderTarget != expected.RenderTarget)
                {
                    throw new InvalidOperationException(
                        "Current render target does not match last render target "
                        + "pushed onto the stack with PushRenderTarget.  Ensure there "
                        + "is no code manually calling SetRenderTarget and not restoring "
                        + "it at the end of it's execution.");
                }
            }

            this.m_RenderTargets.Pop();

            if (this.m_RenderTargets.Count == 0)
            {
                this.GraphicsDevice.SetRenderTargets(null);
            }
            else
            {
                this.GraphicsDevice.SetRenderTargets(this.m_RenderTargets.Peek());
            }
        }

        /// <summary>
        /// Push a render target onto the current rendering context, making it
        /// the active target for rendering.  By using the PushRenderTarget / PopRenderTarget
        /// methods, this allows you to safely chain render target switches, without risk
        /// of losing the previous render target.  An example of where this can be used is
        /// if you want to capture the next frame, you can simply start with a PushRenderTarget
        /// and as long as all other render target switching uses these methods or respects the
        /// previous render target, then everything will be captured as intended.
        /// </summary>
        /// <param name="renderTarget">
        /// The render target instance to make active.
        /// </param>
        public void PushRenderTarget(RenderTargetBinding renderTarget)
        {
            this.m_RenderTargets.Push(new[] { renderTarget });
            this.GraphicsDevice.SetRenderTargets(renderTarget);
        }

        /// <summary>
        /// Push an array of render targets onto the current rendering context, making them
        /// the active target for rendering.  By using the PushRenderTarget / PopRenderTarget
        /// methods, this allows you to safely chain render target switches, without risk
        /// of losing the previous render target.
        /// </summary>
        /// <param name="renderTargets">
        /// The render targets to make active.
        /// </param>
        public void PushRenderTarget(params RenderTargetBinding[] renderTargets)
        {
            this.m_RenderTargets.Push(renderTargets);
            this.GraphicsDevice.SetRenderTargets(renderTargets);
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Render(IGameContext context)
        {
            this.GraphicsDevice = context.Game.GraphicsDevice;

            if (this.SpriteBatch == null)
            {
                this.SpriteBatch = new SpriteBatch(context.Game.GraphicsDevice);
            }

            if (this.SingleWhitePixel == null)
            {
                this.SingleWhitePixel = new Texture2D(context.Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                this.SingleWhitePixel.SetData(new[] { Color.White });
            }

            PostPreRender(context);
        }

        public void PostPreRender(IGameContext context)
        { 
            // Update the MouseRay property of the game context.
            int mouseX, mouseY;
            context.Window.GetCursorPosition(out mouseX, out mouseY);
            var mouse = new Vector2(mouseX, mouseY);
            var nearSource = new Vector3(mouse, 0f);
            var farSource = new Vector3(mouse, 1f);

            var nearPoint = this.GraphicsDevice.Viewport.Unproject(
                nearSource, 
                this.Projection, 
                this.View, 
                this.World);

            var farPoint = this.GraphicsDevice.Viewport.Unproject(
                farSource, 
                this.Projection, 
                this.View, 
                this.World);

            var direction = farPoint - nearPoint;
            direction.Normalize();

            if (float.IsNaN(direction.X) || float.IsNaN(direction.Y) || float.IsNaN(direction.Z))
            {
                direction = Vector3.Zero;
            }

            context.MouseRay = new Ray(nearPoint, direction);

            // Calculate the MouseHorizontalPlane and MouseVerticalPlane properties of
            // the game context if we were able to resolve the mouse ray.
            if (direction != Vector3.Zero)
            {
                var nearSourceHorizontal = new Vector3(mouseX + 1, mouseY, 0f);
                var nearPointHorizontal = this.GraphicsDevice.Viewport.Unproject(
                    nearSourceHorizontal,
                    this.Projection,
                    this.View,
                    this.World);

                context.MouseVerticalPlane = new Plane(nearSource, nearSource + direction, nearPointHorizontal);

                var nearSourceVertical = new Vector3(mouseX, mouseY + 1, 0f);
                var nearPointVertical = this.GraphicsDevice.Viewport.Unproject(
                    nearSourceVertical,
                    this.Projection,
                    this.View,
                    this.World);

                context.MouseHorizontalPlane = new Plane(nearSource, nearSource + direction, nearPointVertical);
            }
        }

        /// <summary>
        /// The recalculate bounding frustum.
        /// </summary>
        private void RecalculateBoundingFrustum()
        {
            this.m_BoundingFrustum = new BoundingFrustum(this.View * this.Projection);
        }
        
        /// <summary>
        /// Adds the specified render pass to the render pipeline
        /// permanently.  This render pass will take effect after the
        /// start of the next frame.
        /// </summary>
        /// <returns>
        /// The render pass that was given to this function.  This return
        /// value is for convenience only, so that you may construct and add
        /// a render pass in a single statement, while obtaining a reference to
        /// it if you need to modify it's values or call <see cref="RemoveFixedRenderPass"/>
        /// later.  The render pass is not modified by this function.
        /// </returns>
        /// <param name="renderPass">The render pass to add.</param>
        public IRenderPass AddFixedRenderPass(IRenderPass renderPass)
        {
            return _renderPipeline.AddFixedRenderPass(renderPass);
        }

        /// <summary>
        /// Removes the specified render pass from the render pipeline.
        /// </summary>
        /// <param name="renderPass">The render pass to remove.</param>
        public void RemoveFixedRenderPass(IRenderPass renderPass)
        {
            _renderPipeline.RemoveFixedRenderPass(renderPass);
        }

        /// <summary>
        /// Append the specified render pass to the render pipeline
        /// for this frame only.  This is method that allows you to temporarily
        /// add additional render passes to a frame.
        /// <para>
        /// If all standard (non-post-processing) render passes have finished
        /// post-processing has begun and this method is given a standard render
        /// pass, it will have no effect.
        /// </para>
        /// <para>
        /// Render passes that were appended can not be removed with
        /// <see cref="RemoveFixedRenderPass"/>.
        /// </para>
        /// </summary>
        /// <returns>
        /// The render pass that was given to this function.  This return
        /// value is for convenience only, so that you may construct and add
        /// a render pass in a single statement, while obtaining a reference to
        /// it if you need to modify it's value.  The render pass is not
        /// modified by this function.
        /// </returns>
        /// <param name="renderPass">The render pass to add.</param>
        public IRenderPass AppendTransientRenderPass(IRenderPass renderPass)
        {
            return _renderPipeline.AppendTransientRenderPass(renderPass);
        }

        /// <summary>
        /// Gets the current render pass that is being used.
        /// </summary>
        /// <value>The current render pass that is being used.</value>
        public IRenderPass CurrentRenderPass
        {
            get
            {
                return _renderPipeline.GetCurrentRenderPass();
            }
        }

        /// <summary>
        /// Returns whether or not the current render pass is of the specified type.
        /// </summary>
        /// <typeparam name="T">The type to check the render pass against.</typeparam>
        /// <returns>Whether or not the current render pass is of the specified type.</returns>
        public bool IsCurrentRenderPass<T>() where T : class, IRenderPass
        {
            return CurrentRenderPass is T;
        }

        /// <summary>
        /// Returns whether or not the current render pass is of the specified type.  Outputs
        /// the casted render pass to currentRenderPass.
        /// </summary>
        /// <typeparam name="T">The type to check the render pass against.</typeparam>
        /// <param name="currentRenderPass">The current render pass casted to the specified type.</param>
        /// <returns>Whether or not the current render pass is of the specified type.</returns>
        public bool IsCurrentRenderPass<T>(out T currentRenderPass) where T : class, IRenderPass
        {
            currentRenderPass = CurrentRenderPass as T;
            return currentRenderPass != null;
        }

        /// <summary>
        /// Returns the current render pass as the type T.
        /// </summary>
        /// <typeparam name="T">The type of render pass to return.</typeparam>
        /// <returns>The current render pass as the type T.</returns>
        public T GetCurrentRenderPass<T>() where T : class, IRenderPass
        {
            if (!(CurrentRenderPass is T))
            {
                throw new InvalidOperationException("The current render pass is not of type " + typeof(T).FullName);
            }

            return (T)CurrentRenderPass;
        }

        /// <summary>
        /// Returns whether this is the first render pass being performed.  You can use
        /// this method to isolate render logic that should only occur once per
        /// frame (such as appending transient render passes).
        /// </summary>
        /// <returns>Whether this is the first render pass being performed.</returns>
        public bool IsFirstRenderPass()
        {
            return _renderPipeline.IsFirstRenderPass();
        }
    }
}

