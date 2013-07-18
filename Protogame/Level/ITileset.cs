using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface ITileset : IEntity
    {
        void SetSize(Vector2 cellSize, Vector2 tilesetSize);
        IEntity this[int x, int y] { get; set; }
    }
}

