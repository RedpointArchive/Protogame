namespace Protogame
{
    using System;

    /// <summary>
    /// The ai asset loader.
    /// </summary>
    public class AIAssetLoader : IAssetLoader
    {
        /// <summary>
        /// The can handle.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanHandle(dynamic data)
        {
            return typeof(AIAsset).IsAssignableFrom(data.type);
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
        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            var value = (AIAsset)Activator.CreateInstance(data.type);
            value.Name = name;
            return value;
        }
    }
}