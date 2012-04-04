using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protogame
{
    public interface IBoundingBox
    {
        float X { get; }
        float Y { get; }
        int Width { get; }
        int Height { get; }
        float XSpeed { get; }
        float YSpeed { get; }
    }
}
