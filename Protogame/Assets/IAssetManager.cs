//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Protogame
{
    public interface IAssetManager
    {
        string Status { get; set; }
        bool IsRemoting { get; }

        void Dirty(string asset);
        IAsset Get(string asset);
        T Get<T>(string asset) where T : class, IAsset;
        IAsset[] GetAll();

        /// <summary>
        /// Saves the state of the asset back into memory so that future Get()
        /// calls will represent the asset in it's state.  This does not save the
        /// changes to permanent storage (see Bake() for that).
        /// </summary>
        /// <param name="asset">Asset.</param>
        void Save(IAsset asset);

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
        /// <param name="asset">Asset.</param>
        void Bake(IAsset asset);
    }
}

