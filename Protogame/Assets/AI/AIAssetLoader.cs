namespace Protogame
{
    using System;
    using Protoinject;

    /// <summary>
    /// The asset loader for <see cref="AIAsset"/>.
    /// </summary>
    public class AIAssetLoader : IAssetLoader
    {
        /// <summary>
        /// Used to dependency inject AI assets as they are loaded.
        /// </summary>
        private readonly IKernel m_Kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AIAssetLoader"/> class.
        /// </summary>
        /// <param name="kernel">
        /// The dependency injection kernel.
        /// </param>
        public AIAssetLoader(IKernel kernel)
        {
            this.m_Kernel = kernel;
        }

        /// <summary>
        /// The can handle.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanHandle(IRawAsset data)
        {
            return typeof(AIAsset).IsAssignableFrom(data.GetProperty<Type>("Type"));
        }

        /// <summary>
        /// The can new.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanNew()
        {
            return false;
        }

        /// <summary>
        /// The get default.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The get new.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        public IAsset Handle(IAssetManager assetManager, string name, IRawAsset data)
        {
            var value = (AIAsset)this.m_Kernel.Get(data.GetProperty<Type>("Type"));
            value.Name = name;
            return value;
        }
    }
}