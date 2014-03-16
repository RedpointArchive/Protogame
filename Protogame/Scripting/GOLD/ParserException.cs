// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD
{
    using System;

    public class ParserException : Exception
    {
        internal ParserException(string message)
            : base(message)
        {
            this.Method = string.Empty;
        }

        internal ParserException(string message, Exception inner, string method)
            : base(message, inner)
        {
            this.Method = method;
        }

        public string Method { get; set; }
    }
}