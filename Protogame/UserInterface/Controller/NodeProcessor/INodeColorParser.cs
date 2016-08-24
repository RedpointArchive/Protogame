using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface INodeColorParser
    {
        Color? Parse(string text);
    }
}