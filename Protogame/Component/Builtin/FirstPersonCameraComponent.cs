using System;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class FirstPersonCameraComponent : IRenderableComponent, IHasTransform
    {
        private readonly INode _node;

        private readonly IFirstPersonCamera _firstPersonCamera;

        public FirstPersonCameraComponent(INode node, IFirstPersonCamera firstPersonCamera)
        {
            _node = node;
            _firstPersonCamera = firstPersonCamera;
            HeadOffset = new Vector3(0, 0, 0);
            Enabled = true;
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

        public void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext)
        {
            if (Enabled && renderContext.IsCurrentRenderPass<I3DRenderPass>())
            {
                var parentFinalTransform = (_node?.Parent?.UntypedValue as IHasTransform)?.FinalTransform;
                if (parentFinalTransform == null)
                {
                    return;
                }

                var sourceLocal = HeadOffset;
                var lookAtLocal = HeadOffset + Vector3.Transform(Vector3.Forward, Transform.LocalMatrix);
                var upLocal = HeadOffset + Vector3.Up;
                _firstPersonCamera.Apply(
                    renderContext,
                    Vector3.Transform(sourceLocal, parentFinalTransform.AbsoluteMatrix),
                    Vector3.Transform(lookAtLocal, parentFinalTransform.AbsoluteMatrix),
                    Vector3.Transform(upLocal, parentFinalTransform.AbsoluteMatrix) - Vector3.Transform(sourceLocal, parentFinalTransform.AbsoluteMatrix));/*
                    Vector3.Transform(Vector3.Up, parentFinalTransform.AbsoluteMatrix) - Vector3.Transform(Vector3.Zero, parentFinalTransform.AbsoluteMatrix));*/
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
            set
            {
                throw new NotSupportedException("Configure the transform of a FirstPersonCameraComponent by using the Pitch, Yaw and Roll properties.");
            }
        }

        public IFinalTransform FinalTransform
        {
            get { return this.GetAttachedFinalTransformImplementation(_node); }
        }

        /// <summary>
        /// Returns the world space directional vector that indicates the direction in which the camera will looking.  This
        /// roughly translates to what is "forward" for the player.
        /// <para>
        /// If the camera is angled upward, this vector will include that upward component.  For a value more suited towards
        /// movement calculations, see <see cref="GetWorldSpaceLateralLookAtVector"/>.
        /// </para>
        /// </summary>
        /// <returns>The world space look at vector.</returns>
        public Vector3 GetWorldSpaceLookAtVector()
        {
            var parentFinalTransform = (_node?.Parent?.UntypedValue as IHasTransform)?.FinalTransform;
            if (parentFinalTransform == null)
            {
                return Vector3.Forward;
            }

            var sourceLocal = HeadOffset;
            var lookAtLocal = HeadOffset + Vector3.Transform(Vector3.Forward, Transform.LocalMatrix);
            return Vector3.Transform(lookAtLocal, parentFinalTransform.AbsoluteMatrix) -
                   Vector3.Transform(sourceLocal, parentFinalTransform.AbsoluteMatrix);
        }

        /// <summary>
        /// Returns the world space directional vector that indicates the direction in which the camera will looking, ignoring
        /// pitch and roll aspects of the camera.  This roughly translates to what is "forward" for the player.
        /// </summary>
        /// <returns>The world space look at vector, without pitch and roll.</returns>
        public Vector3 GetWorldSpaceLateralLookAtVector()
        {
            var parentFinalTransform = (_node?.Parent?.UntypedValue as IHasTransform)?.FinalTransform;
            if (parentFinalTransform == null)
            {
                return Vector3.Forward;
            }

            var sourceLocal = HeadOffset;
            var lookAtLocal = HeadOffset + Vector3.Transform(Vector3.Forward, Matrix.CreateFromYawPitchRoll(Yaw, 0, 0));
            return Vector3.Transform(lookAtLocal, parentFinalTransform.AbsoluteMatrix) -
                   Vector3.Transform(sourceLocal, parentFinalTransform.AbsoluteMatrix);
        }
    }
}
