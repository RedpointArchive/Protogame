namespace Protogame
{
    /// <summary>
    /// Indicates the target that the physics engine will synchronise to.
    /// </summary>
    /// <module>Physics</module>
    public enum PhysicsTarget
    {
        /// <summary>
        /// Indicates that the physics system should synchronise position and rotation changes
        /// made by the physics engine against the component owner (usually the entity) that owns
        /// this component.  When this option is selected, the position and rotation of the entity
        /// changes.
        /// </summary>
        ComponentOwner,

        /// <summary>
        /// Indicates that the physics system should synchronise position and rotation changes
        /// made by the physics engine against the component itself.  When this option is selected,
        /// the position and rotation of the component changes (in the <c>Transform</c> property
        /// of the component).
        /// </summary>
        Component
    }
}