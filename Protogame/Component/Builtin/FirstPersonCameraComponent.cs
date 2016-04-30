using System;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class FirstPersonCameraComponent : IRenderableComponent, IHasMatrix
    {
        private readonly INode _node;

        private readonly IFirstPersonCamera _firstPersonCamera;

        public FirstPersonCameraComponent(INode node, IFirstPersonCamera firstPersonCamera)
        {
            _node = node;
            _firstPersonCamera = firstPersonCamera;
            HeadOffset = new Vector3(0, 5f, 0);
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
                var finalMatrix = entity.GetFinalMatrix();

                _firstPersonCamera.Apply(
                    renderContext,
                    Vector3.Transform(HeadOffset, finalMatrix),
                    Vector3.Transform(HeadOffset + Vector3.Forward, LocalMatrix));
            }
        }

        public Matrix LocalMatrix
        {
            get
            {
                return
                    Matrix.CreateFromYawPitchRoll(Yaw, Pitch, Roll);
            }
            set
            {
                throw new NotSupportedException("Configure the local matrix of a FirstPersonCameraComponent by using the Pitch, Yaw and Roll properties.");
            }
        }

        public Matrix GetFinalMatrix()
        {
            return this.GetDefaultFinalMatrixImplementation(_node);
        }
    }
}
