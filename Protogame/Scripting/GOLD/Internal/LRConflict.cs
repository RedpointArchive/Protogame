// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD.Internal
{
    internal enum LRConflict
    {
        ShiftShift = 1, 

        // Never happens
        ShiftReduce = 2, 

        ReduceReduce = 3, 

        AcceptReduce = 4, 

        // Never happens with this implementation
        None = 5
    }
}