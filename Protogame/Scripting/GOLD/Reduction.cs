// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD
{
    using System.ComponentModel;

    public class Reduction : TokenList
    {
        internal Reduction(int size)
        {
            this.ReDimension(size);
        }

        [Description("Returns/sets the parse data stored in the token. It is a shortcut to Item(Index).Token.")]
        public DataIndex Data
        {
            get
            {
                return new DataIndex(this);
            }
        }

        [Description("Returns the parent production.")]
        public Production Parent { get; internal set; }

        [Description("Returns/sets any additional user-defined data to this object.")]
        public object Tag { get; set; }

        internal void ReDimension(int size)
        {
            // Increase the size of the array to Size empty elements.
            int n;

            this.Clear();
            for (n = 0; n <= size - 1; n++)
            {
                this.Add(null);
            }
        }
    }
}