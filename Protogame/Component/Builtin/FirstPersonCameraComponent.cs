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

        public void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.IsCurrentRenderPass<I3DRenderPass>())
            {
                var parentFinalTransform = (_node?.Parent?.UntypedValue as IHasTransform)?.FinalTransform;
                if (parentFinalTransform == null)
                {
                    return;
                }

                //var finalMatrix = this.GetFinalMatrix();

                _firstPersonCamera.Apply(
                    renderContext,
                    Vector3.Transform(HeadOffset, parentFinalTransform.AbsoluteMatrix),
                    Vector3.Transform(HeadOffset, parentFinalTransform.AbsoluteMatrix) + Vector3.Transform(Vector3.Forward, Transform.LocalMatrix));

                /*
                _firstPersonCamera.Apply(
                    renderContext,
                    Vector3.Transform(HeadOffset, finalMatrix),
                    Vector3.Transform(HeadOffset, finalMatrix) + (Vector3.Transform(Vector3.Forward, finalMatrix) - Vector3.Transform(Vector3.Zero, finalMatrix)),
                    Vector3.Transform(Vector3.Up, finalMatrix) - Vector3.Transform(Vector3.Zero, finalMatrix));
                    */
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
                throw new NotSupportedException("Configure the local matrix of a FirstPersonCameraComponent by using the Pitch, Yaw and Roll properties.");
            }
        }

        public IFinalTransform FinalTransform
        {
            get { return this.GetAttachedFinalTransformImplementation(_node); }
        }
    }
}
