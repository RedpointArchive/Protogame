// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD
{
    public class Position
    {
        internal Position()
        {
            this.Line = 0;
            this.Column = 0;
        }

        public int Column { get; set; }

        public int Line { get; set; }

        internal void Copy(Position pos)
        {
            this.Column = pos.Column;
            this.Line = pos.Line;
        }
    }
}