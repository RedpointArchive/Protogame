// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD.Internal
{
    internal class CharacterRange
    {
        public CharacterRange(ushort start, ushort end)
        {
            this.Start = start;
            this.End = end;
        }

        public ushort End { get; set; }

        public ushort Start { get; set; }
    }
}