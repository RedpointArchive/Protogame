// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD.Internal
{
    using System.Collections;

    internal class FAEdgeList : ArrayList
    {
        public new FAEdge this[int index]
        {
            get
            {
                return (FAEdge)base[index];
            }

            set
            {
                base[index] = value;
            }
        }

        public int Add(FAEdge edge)
        {
            return base.Add(edge);
        }
    }
}