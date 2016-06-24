using System;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class FirstPersonCameraComponent : IPrerenderableComponent, IHasTransform
    {
        private readonly INode _node;
        private readonly IDebugRenderer _debugRenderer;

        private readonly IFirstPersonCamera _firstPersonCamera;

        public FirstPersonCameraComponent(
            INode node,
            IDebugRenderer debugRenderer,
            IFirstPersonCamera firstPersonCamera)
        {
            _node = node;
            _debugRenderer = debugRenderer;
            _firstPersonCamera = firstPersonCamera;
            HeadOffset = new Vector3(0, 0, 0);
            Enabled = true;
            DebugEnabled = false;
        }

        /// <summary>
        /// The head offset from the parent's matrix.
        /// </summary>
        public Vector3 HeadOffset { get; set; }

        /// <summary>
        /// The yaw of the camera, that is, how much it has turned to the left or right.
        /// </summary>
        public float Yaw { get; set; }

        /// <summary>
        /// The pitch of the camera, that is, how much it is facing up or down.
        /// </summary>
        public float Pitch { get; set; }

        /// <summary>
        /// The roll of the camera, that is, how much it is tilted along the view.
        /// </summary>
        public float Roll { get; set; }

        /// <summary>
        /// Whether or not this camera will configure the current render matrices.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Whether or not this camera will render debug information about it's view configuration.
        /// </summary>
        public bool DebugEnabled { get; set; }

        public void Prerender(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext)
        {
            var enabled = Enabled && renderContext.IsCurrentRenderPass<I3DRenderPass>();
            var debugEnabled = DebugEnabled && renderContext.IsCurrentRenderPass<IDebugRenderPass>();
            if (enabled || DebugEnabled)
            {
                var parentFinalTransform = (_node?.Parent?.UntypedValue as IHasTransform)?.FinalTransform;
                if (parentFinalTransform == null)
                {
                    return;
                }

                var sourceLocal = HeadOffset;
                var lookAtLocal = HeadOffset + Vector3.Transform(Vector3.Forward, Transform.LocalMatrix);
                var upLocal = HeadOffset + Vector3.Up;

                var sourceAbsolute = Vector3.Transform(sourceLocal, parentFinalTransform.AbsoluteMatrix);
                var lookAtAbsolute = Vector3.Transform(lookAtLocal, parentFinalTransform.AbsoluteMatrix);
                var upAbsolute = Vector3.Transform(upLocal, parentFinalTransform.AbsoluteMatrix) -
                                 Vector3.Transform(sourceLocal, parentFinalTransform.AbsoluteMatrix);

                if (enabled)
                {
                    _firstPersonCamera.Apply(
                        renderContext,
                        sourceAbsolute,
                        lookAtAbsolute,
                        upAbsolute);
                }
                else if (debugEnabled)
                {
                    _debugRenderer.RenderDebugLine(
                        renderContext,
                        sourceAbsolute,
                        lookAtAbsolute,
                        Color.Cyan,
                        Color.Cyan);
                    _debugRenderer.RenderDebugLine(
                        renderContext,
                        sourceAbsolute,
                        sourceAbsolute + upAbsolute,
                        Color.Cyan,
                        Color.Lime);
                }
            }
        }

        public ITransform Transform
        {
            get
            {
                var transform = new DefaultTransform();
                transform.SetFromCustomMatrix(Matrix.CreateFromYawPitchRoll(Yaw, Pitch, Roll));
                return transform;
            }
        }

        public IFinalTransform FinalTransform
        {
            get { return this.GetAttachedFinalTransformImplementation(_node); }
        }

        /// <summary>
        /// Computes a world space directional vector based on a directional vector local to the camera, using the 
        /// camera's view matrix.  If you pass <see cref="Vector3.Forward"/> to this method, you'll get a world space
        /// vector that roughly translates to what is "forward" for the player.
        /// <para>
        /// If the camera is angled upward, this vector will include that upward component.  For a value more suited towards
        /// movement calculations, see <see cref="GetWorldSpaceLateralLookAtVector"/>.
        /// </para>
        /// </summary>
        /// <returns>The world space look at vector.</returns>
        public Vector3 ComputeWorldSpaceVectorFromLocalSpace(Vector3 local)
        {
            var parentFinalTransform = (_node?.Parent?.UntypedValue as IHasTransform)?.FinalTransform;
            if (parentFinalTransform == null)
            {
                return Vector3.Forward;
            }

            var sourceLocal = HeadOffset;
            var lookAtLocal = HeadOffset + Vector3.Transform(local, Transform.LocalMatrix);
            return Vector3.Transform(lookAtLocal, parentFinalTransform.AbsoluteMatrix) -
                   Vector3.Transform(sourceLocal, parentFinalTransform.AbsoluteMatrix);
        }

        /// <summary>
        /// Computes a world space directional vector based on a directional vector local to the camera, using only the
        /// yaw component of the camera's view matrix.  If you pass <see cref="Vector3.Forward"/> to this method, you'll get a world space
        /// vector that roughly translates to what is "laterally forward" for the player.
        /// </summary>
        /// <returns>The world space look at vector, without pitch and roll.</returns>
        public Vector3 ComputeWorldSpaceLateralVectorFromLocalSpace(Vector3 local)
        {
            var parentFinalTransform = (_node?.Parent?.UntypedValue as IHasTransform)?.FinalTransform;
            if (parentFinalTransform == null)
            {
                return Vector3.Forward;
            }

            var sourceLocal = HeadOffset;
            var lookAtLocal = HeadOffset + Vector3.Transform(local, Matrix.CreateFromYawPitchRoll(Yaw, 0, 0));
            return Vector3.Transform(lookAtLocal, parentFinalTransform.AbsoluteMatrix) -
                   Vector3.Transform(sourceLocal, parentFinalTransform.AbsoluteMatrix);
        }
    }
}
