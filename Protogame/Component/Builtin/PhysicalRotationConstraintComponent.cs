using System;
using System.Collections.Generic;
using System.Linq;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Protoinject;

namespace Protogame
{
    public class PhysicalRotationConstraintComponent : IUpdatableComponent, IServerUpdatableComponent, IEnabledComponent
    {
        private readonly IPhysicalComponent _physicalComponent;
        private Dictionary<RigidBody, bool> _lastUpdated;
        private bool _enabled;

        public PhysicalRotationConstraintComponent(
            [FromParent, RequireExisting] IPhysicalComponent physicalComponent)
        {
            _physicalComponent = physicalComponent;
            _lastUpdated = new Dictionary<RigidBody, bool>();

            Enabled = true;
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; _lastUpdated.Clear(); }
        }

        public void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext)
        {
            Update();
        }

        public void Update(ComponentizedEntity entity, IServerContext serverContext, IUpdateContext updateContext)
        {
            Update();
        }

        private void Update()
        {
            if (Enabled)
            {
                foreach (var rigidBody in _physicalComponent.RigidBodies)
                {
                    if (!_lastUpdated.ContainsKey(rigidBody) || _lastUpdated[rigidBody] == false)
                    {
                        rigidBody.SetMassProperties(JMatrix.Zero, 1f/rigidBody.Mass, true);
                        _lastUpdated[rigidBody] = true;
                    }
                }
            }
            else
            {
                foreach (var rigidBody in _physicalComponent.RigidBodies)
                {
                    if (_lastUpdated.ContainsKey(rigidBody) || _lastUpdated[rigidBody] == false)
                    {
                        rigidBody.SetMassProperties();
                        _lastUpdated[rigidBody] = true;
                    }
                }
            }
        }
    }
}
