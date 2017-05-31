namespace Protogame
{
    /// <summary>
    /// Provides the base directory of the game.  You should always use this interface instead of
    /// <see cref="System.Environment.CurrentDirectory"/>, as this value is correct when the game is
    /// running inside an editor.
    /// </summary>
    public interface IBaseDirectory
    {
        /// <summary>
        /// The full directory path to the game; this directory is the base where the content directory
        /// will then be searched from.
        /// </summary>
        string FullPath { get; }
    }
}
