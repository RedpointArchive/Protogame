using System.Collections.Generic;

namespace Protogame
{
    public interface IHasLights
    {
        IEnumerable<ILight> GetLights();
    }
}
