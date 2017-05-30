using System;

namespace Protogame
{
    public class DefaultBaseDirectory : IBaseDirectory
    {
        public DefaultBaseDirectory()
        {
            FullPath = Environment.CurrentDirectory;
        }

        public string FullPath { get; }
    }
}
