using Jitter.LinearMath;
using Protoinject;

namespace Protogame
{
    public class PhysicalRotationConstraintComponent : IUpdatableComponent
    {
        private readonly IPhysicalComponent _physicalComponent;
        private bool _wasEnabled = false;

        public PhysicalRotationConstraintComponent(
            [FromParent, RequireExisting] IPhysicalComponent physicalComponent)
        {
            _physicalComponent = physicalComponent;

            Enabled = true;
        }

        public bool Enabled { get; set; }

        public void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext)
        {
            if (Enabled)
            {
                if (!_wasEnabled)
                {
                    foreach (var rigidBody in _physicalComponent.RigidBodies)
                    {
                        rigidBody.SetMassProperties(JMatrix.Zero, 1f / rigidBody.Mass, true);
                    }

                    _wasEnabled = true;
                }
            }
            else
            {
                if (_wasEnabled)
                {
                    foreach (var rigidBody in _physicalComponent.RigidBodies)
                    {
                        rigidBody.SetMassProperties();
                    }

                    _wasEnabled = false;
                }
            }
        }
    }
}
