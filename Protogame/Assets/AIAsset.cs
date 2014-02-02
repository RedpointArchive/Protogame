namespace Protogame
{
    using System;

    /// <summary>
    /// The ai asset.
    /// </summary>
    public abstract class AIAsset : MarshalByRefObject, IAsset
    {
        /// <summary>
        /// Gets a value indicating whether compiled only.
        /// </summary>
        /// <value>
        /// The compiled only.
        /// </value>
        public bool CompiledOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether source only.
        /// </summary>
        /// <value>
        /// The source only.
        /// </value>
        public bool SourceOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        public abstract void Render(IEntity entity, IGameContext gameContext, IRenderContext renderContext);

        /// <summary>
        /// The resolve.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(AIAsset)))
            {
                return this as T;
            }

            throw new InvalidOperationException("Asset already resolved to AIAsset.");
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="updateContext">
        /// The update context.
        /// </param>
        public abstract void Update(IEntity entity, IGameContext gameContext, IUpdateContext updateContext);

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public abstract void Update(IServerEntity entity);
    }
}