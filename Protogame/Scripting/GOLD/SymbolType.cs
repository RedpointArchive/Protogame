// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD
{
    public enum SymbolType
    {
        Nonterminal = 0, 

        // Nonterminal 
        Content = 1, 

        // Passed to the parser
        Noise = 2, 

        // Ignored by the parser
        End = 3, 

        // End character (EOF)
        GroupStart = 4, 

        // Group start  
        GroupEnd = 5, 

        // Group end   
        // Note: There is no value 6. CommentLine was deprecated.
        Error = 7

        // Error symbol
    }
}