namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The Tileset interface.
    /// </summary>
    public interface ITileset : IEntity
    {
        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="IEntity"/>.
        /// </returns>
        IEntity this[int x, int y] { get; set; }

        /// <summary>
        /// The set size.
        /// </summary>
        /// <param name="cellSize">
        /// The cell size.
        /// </param>
        /// <param name="tilesetSize">
        /// The tileset size.
        /// </param>
        void SetSize(Vector2 cellSize, Vector2 tilesetSize);
    }
}