#if FALSE

using System;
using Protogame;

namespace ProtogameAssetTool
{
    internal class BuiltinAssetFsLayer : LocalFilesystemAssetFsLayer
    {
        public BuiltinAssetFsLayer() : base(Environment.CurrentDirectory)
        {
        }
    }
}

#endif