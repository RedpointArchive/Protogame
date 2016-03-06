using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// The base class for entities which support having components.
    /// <para>
    /// Componentized entities are entities which automatically support the attachment
    /// of updatable and renderable components (<see cref="IUpdatableComponent"/> and
    /// <see cref="IRenderableComponent"/> respectively).
    /// </para>
    /// <para>
    /// By allowing the registration of components against entities, you can re-use
    /// additional behaviour and presentation on entities without deriving new classes,
    /// or implementing full dependency injected services (with appropriate bindings).
    /// You can register components on a componetized entity by calling
    /// <see cref="ComponentizedObject.RegisterComponent"/> on this object.
    /// </para>
    /// <para>
    /// You can also support additional component types and callbacks through the use
    /// of <see cref="ComponentizedObject.RegisterCallable{T}"/>; refer to the
    /// <see cref="ComponentizedObject"/> documentation for more information on how to
    /// register additional callable component methods.
    /// </para>
    /// </summary>
    public class ComponentizedEntity : ComponentizedObject, IEntity
    {
        /// <summary>
        /// The component callable for handling updatable components.
        /// </summary>
        private IComponentCallable<IGameContext, IUpdateContext> _update;

        /// <summary>
        /// The component callable for handling renderable components.
        /// </summary>
        private IComponentCallable<IGameContext, IRenderContext> _render;

        /// <summary>
        /// Initializes a new <see cref="ComponentizedEntity"/>.
        /// <para>
        /// Componentized entities are entities which automatically support the attachment
        /// of updatable and renderable components (<see cref="IUpdatableComponent"/> and
        /// <see cref="IRenderableComponent"/> respectively).
        /// </para>
        /// </summary>
        public ComponentizedEntity()
        {
            LocalMatrix = Matrix.Identity;

            _update = RegisterCallable<IUpdatableComponent, IGameContext, IUpdateContext>((t, g, u) => t.Update(this, g, u));
            _render = RegisterCallable<IRenderableComponent, IGameContext, IRenderContext>((t, g, r) => t.Render(this, g, r));

            RegisterPrivateComponent(this); // Register this as IPositionComponent.
        }

        /// <summary>
        /// Renders the entity.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="renderContext">The current render context.</param>
        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            _render.Invoke(gameContext, renderContext);
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="updateContext">The current update context.</param>
        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            _update.Invoke(gameContext, updateContext);
        }

        public Matrix LocalMatrix { get; set; }

        public Matrix GetFinalMatrix()
        {
            return LocalMatrix;
        }
    }
}
