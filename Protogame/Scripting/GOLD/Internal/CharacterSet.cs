// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD.Internal
{
    using System.Collections;

    internal class CharacterSet : ArrayList
    {
        public new CharacterRange this[int index]
        {
            get
            {
                return (CharacterRange)base[index];
            }

            set
            {
                base[index] = value;
            }
        }

        public int Add(ref CharacterRange item)
        {
            return this.Add(item);
        }

        public bool Contains(int charCode)
        {
            // This procedure searchs the set to deterimine if the CharCode is in one
            // of the ranges - and, therefore, the set.
            // The number of ranges in any given set are relatively small - rarely 
            // exceeding 10 total. As a result, a simple linear search is sufficient 
            // rather than a binary search. In fact, a binary search overhead might
            // slow down the search!
            var found = false;
            var n = 0;

            while ((n < this.Count) & (!found))
            {
                var range = (CharacterRange)base[n];

                found = charCode >= range.Start & charCode <= range.End;
                n += 1;
            }

            return found;
        }
    }
}