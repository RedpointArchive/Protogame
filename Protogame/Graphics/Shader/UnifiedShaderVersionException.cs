using System;

namespace Protogame
{
    public class UnifiedShaderVersionException : Exception
    {
        public UnifiedShaderVersionException() : base("Unknown version declaration, are you missing '#version 1' at the start of your shader?")
        { }
    }
}