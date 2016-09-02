using Jitter;
using Jitter.Collision.Shapes;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class FirstPersonControllerNetworkablePhysicsComponent : IUpdatableComponent, IServerUpdatableComponent, IRenderableComponent, IEnabledComponent, IFirstPersonControllerComponent, INetworkedComponent
    {
        private readonly IPhysicsEngine _physicsEngine;
        private readonly IDebugRenderer _debugRenderer;
        private readonly IConsoleHandle _consoleHandle;
        private readonly INetworkMessageSerialization _networkMessageSerialization;
        private readonly NetworkSynchronisationComponent _networkSynchronisationComponent;

        private JitterWorld _jitterWorld;
        private CapsuleShape _shape;
        private IHasTransform _entity;

        public FirstPersonControllerNetworkablePhysicsComponent(
            IPhysicsEngine physicsEngine,
            IDebugRenderer debugRenderer,
            IConsoleHandle consoleHandle,
            INetworkMessageSerialization networkMessageSerialization,
            [FromParent, RequireExisting] NetworkSynchronisationComponent networkSynchronisationComponent)
        {
            _physicsEngine = physicsEngine;
            _debugRenderer = debugRenderer;
            _consoleHandle = consoleHandle;
            _networkMessageSerialization = networkMessageSerialization;
            _networkSynchronisationComponent = networkSynchronisationComponent;
            _shape = new CapsuleShape(2.5f, 1f);
            MovementSpeed = 1f;

            Enabled = true;
        }

        public bool Enabled { get; set; }

        public float MovementSpeed { get; set; }

        public float Radius
        {
            get { return _shape.Radius; }
            set { _shape.Radius = value; }
        }

        public float Length
        {
            get { return _shape.Length; }
            set { _shape.Length = value; }
        }

        public void MoveInDirection(Vector3 direction)
        {
            if (direction == Vector3.Zero)
            {
                return;
            }

            direction.Normalize();

            var serverMessage = new InputPredictMessage
            {
                MovementDirX = direction.X,
                MovementDirY = direction.Y,
                MovementDirZ = direction.Z,
            };
            _networkSynchronisationComponent.EnqueuePredictedOperation(ApplyMovement, serverMessage);
            ApplyMovement(serverMessage);
        }

        private void ApplyMovement(InputPredictMessage message)
        {
            if (_entity == null)
            {
                _consoleHandle.LogError("Attempted to apply FPS movement, but had no entity.");
                return;
            }
            
            _entity.Transform.LocalPosition += new Vector3(message.MovementDirX, message.MovementDirY, message.MovementDirZ) * (MovementSpeed / 10f);
        }

        public void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext)
        {
            if (!Enabled)
            {
                return;
            }

            _entity = entity;
        }

        public void Update(ComponentizedEntity entity, IServerContext serverContext, IUpdateContext updateContext)
        {
            if (!Enabled)
            {
                return;
            }

            _entity = entity;
        }

        public void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext)
        {
            if (!Enabled)
            {
                return;
            }
        }

        public bool ReceiveMessage(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext,
            MxDispatcher dispatcher, MxClient server, byte[] payload, uint protocolId)
        {
            return false;
        }

        public bool ReceiveMessage(ComponentizedEntity entity, IServerContext serverContext, IUpdateContext updateContext,
            MxDispatcher dispatcher, MxClient client, byte[] payload, uint protocolId)
        {
            // Check to make sure the synchronisation component is in replay inputs mode.
            if (_networkSynchronisationComponent.ClientAuthoritiveMode != ClientAuthoritiveMode.ReplayInputs)
            {
                return false;
            }
            
            // Check to see if the message is coming from a client that has authority.
            if (_networkSynchronisationComponent.ClientOwnership != null && _networkSynchronisationComponent.ClientOwnership != client.Group)
            {
                // We don't trust this message.
                return false;
            }

            // Read the input predict message and verify it is for our entity.
            var predictMessage = _networkMessageSerialization.Deserialize(payload) as InputPredictMessage;
            if (predictMessage == null || predictMessage.EntityID != _networkSynchronisationComponent.NetworkID.Value)
            {
                return false;
            }
            
            // If we got to here, then this is a prediction message from the entity that this component
            // is attached to.  So apply the movement on the server.
            ApplyMovement(predictMessage);

            return false;
        }
    }
}
