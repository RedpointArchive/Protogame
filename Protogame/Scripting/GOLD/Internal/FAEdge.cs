// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD.Internal
{
    internal class FAEdge
    {
        // Characters to advance on	
        public CharacterSet Characters { get; set; }

        // FAState
        public int Target { get; set; }

        public FAEdge(CharacterSet charSet, int target)
        {
            this.Characters = charSet;
            this.Target = target;
        }

        public FAEdge()
        {
            // Nothing for now
        }
    }
}