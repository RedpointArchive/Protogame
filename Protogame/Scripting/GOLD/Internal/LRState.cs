// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD.Internal
{
    using System.Collections;

    internal class LRState : ArrayList
    {
        public LRAction this[short index]
        {
            get
            {
                return (LRAction)base[index];
            }

            set
            {
                base[index] = value;
            }
        }

        public LRAction this[Symbol sym]
        {
            get
            {
                int index = this.IndexOf(sym);
                if (index != -1)
                {
                    return (LRAction)base[index];
                }

                return null;
            }

            set
            {
                int index = this.IndexOf(sym);
                if (index != -1)
                {
                    base[index] = value;
                }
            }
        }

        public void Add(LRAction action)
        {
            base.Add(action);
        }

        public short IndexOf(Symbol item)
        {
            // Returns the index of SymbolIndex in the table, -1 if not found
            short index = 0;

            short n = 0;
            var found = false;
            while ((!found) & n < this.Count)
            {
                if (item.Equals(((LRAction)base[n]).Symbol))
                {
                    index = n;
                    found = true;
                }

                n += 1;
            }

            if (found)
            {
                return index;
            }

            return -1;
        }
    }
}