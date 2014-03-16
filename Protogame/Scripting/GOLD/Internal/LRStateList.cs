// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD.Internal
{
    using System.Collections;

    internal class LRStateList : ArrayList
    {
        public LRStateList()
        {
            this.InitialState = 0;
        }

        internal LRStateList(int size)
        {
            this.ReDimension(size);
            this.InitialState = 0;
        }

        public short InitialState { get; set; }

        public new LRState this[int index]
        {
            get
            {
                return (LRState)base[index];
            }

            set
            {
                base[index] = value;
            }
        }

        public int Add(ref LRState item)
        {
            return this.Add(item);
        }

        internal void ReDimension(int size)
        {
            // Increase the size of the array to Size empty elements.
            int n;

            this.Clear();
            for (n = 0; n <= size - 1; n++)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                this.Add(null);
            }
        }
    }
}