using System;

namespace Protogame
{
    internal class UnifiedShaderLexingException : Exception
    {
        public UnifiedShaderLexingException(string message, int lineNumber, int columnNumber) : base(message + " at USL source code line " + lineNumber + ", " + columnNumber)
        {
        }
    }
}