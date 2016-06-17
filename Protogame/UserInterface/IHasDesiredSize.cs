using System;

namespace Protogame
{
    public interface IHasDesiredSize
    {
        int? GetDesiredWidth(ISkin skin);

        int? GetDesiredHeight(ISkin skin);
    }
}

