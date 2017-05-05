using System;
using System.Collections.Generic;

namespace Protogame
{
    public interface IReadableSerializedAsset : IDisposable
    {
        byte[] GetByteArray(string property);

        IReadOnlyCollection<string> Dependencies { get; }
    }
}
