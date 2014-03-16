// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD.Internal
{
    internal class LRAction
    {
        public LRAction(Symbol symbol, LRActionType type, short value)
        {
            this.Symbol = symbol;
            this.Type = type;
            this.Value = value;
        }

        public Symbol Symbol { get; set; }

        public LRActionType Type { get; set; }

        // shift to state, reduce rule, goto state
        public short Value { get; set; }
    }
}