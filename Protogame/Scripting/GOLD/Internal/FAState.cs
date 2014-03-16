// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD.Internal
{
    internal class FAState
    {
        public FAState(Symbol accept)
        {
            this.Accept = accept;
            this.Edges = new FAEdgeList();
        }

        public FAState()
        {
            this.Accept = null;
            this.Edges = new FAEdgeList();
        }

        public Symbol Accept { get; set; }

        public FAEdgeList Edges { get; set; }
    }
}