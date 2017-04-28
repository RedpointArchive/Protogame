using System;
using System.IO;
using System.Threading.Tasks;

namespace Protogame
{
    public interface IAssetFsFile
    {
        string Name { get; }

        string Extension { get; }

        DateTimeOffset ModificationTimeUtcTimestamp { get; }

        Task<Stream> GetContentStream();
    }
}
