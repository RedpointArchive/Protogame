using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protogame
{
    public class ProtogameException : Exception
    {
        public ProtogameException() : base("An exception occurred inside Protogame.") { }
        public ProtogameException(string message) : base(message) { }
        public ProtogameException(Exception innerException) : base("An exception occurred inside Protogame.", innerException) { }
        public ProtogameException(string message, Exception innerException) : base(message, innerException) { }
    }
}
