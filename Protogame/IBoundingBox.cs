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
        float Width { get; }
        float Height { get; }
        #if LEGACY
        float XSpeed { get; }
        float YSpeed { get; }
        #endif
    }
}
