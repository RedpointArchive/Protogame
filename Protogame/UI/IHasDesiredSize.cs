using System;

namespace Protogame
{
    public interface IHasDesiredSize
    {
        int? DesiredWidth { get; }

        int? DesiredHeight { get; }
    }
}

