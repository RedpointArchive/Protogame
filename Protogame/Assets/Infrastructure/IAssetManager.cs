namespace Protogame
{
    /// <summary>
    /// The AssetManager interface.
    /// </summary>
    public interface IAssetManager
    {
        /// <summary>
        /// Gets a value indicating whether is remoting.
        /// </summary>
        /// <value>
        /// The is remoting.
        /// </value>
        bool IsRemoting { get; }

        /// <summary>
        /// Performs a save operation and then bakes the asset to disk, saving
        /// the new state of the asset permanently.
        /// </summary>
        /// <remarks>
        /// The reason for the difference between saving and baking is that when
        /// using the asset manager connected to the game, you need to be able to
        /// save the changes so they appear in the game, but you may not be ready
        /// to save those changes permanently (the main reason for real-time updating
        /// of assets is to allow users to experiment with assets on the fly).
        /// </remarks>
        /// <param name="asset">
        /// Asset.
        /// </param>
        void Bake(IAsset asset);

        /// <summary>
        /// The dirty.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        void Dirty(string asset);

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T Get<T>(string asset) where T : class, IAsset;

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IAsset[]"/>.
        /// </returns>
        IAsset[] GetAll();

        string[] GetAllNames();

        /// <summary>
        /// The get unresolved.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        IAsset GetUnresolved(string asset);

        /// <summary>
        /// Recompiles the given asset, regardless of whether it already has compiled
        /// data associated with it.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        void Recompile(IAsset asset);

        /// <summary>
        /// Saves the state of the asset back into memory so that future Get()
        /// calls will represent the asset in it's state.  This does not save the
        /// changes to permanent storage (see Bake() for that).
        /// </summary>
        /// <param name="asset">
        /// Asset.
        /// </param>
        void Save(IAsset asset);

        /// <summary>
        /// The try get.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T TryGet<T>(string asset) where T : class, IAsset;
    }
}