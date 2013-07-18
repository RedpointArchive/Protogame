using System.Collections.Generic;
using System.IO;

namespace Protogame
{
    public interface ILevelReader
    {
        IEnumerable<IEntity> Read(Stream stream);
    }
}

