using System.IO;

namespace Protogame
{
    public interface IRemoteClientReaderWriter
    {
        BinaryReader Reader { get; }

        BinaryWriter Writer { get; }
    }
}