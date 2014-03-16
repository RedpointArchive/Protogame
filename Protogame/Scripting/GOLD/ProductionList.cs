// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD
{
    using System.Collections;
    using System.ComponentModel;

    public class ProductionList
    {
        // Cannot inherit, must hide methods that change the list
        private readonly ArrayList m_Array;

        internal ProductionList()
        {
            this.m_Array = new ArrayList();
        }

        internal ProductionList(int size)
        {
            this.m_Array = new ArrayList();
            this.ReDimension(size);
        }

        [Description("Returns the production with the specified index.")]
        public Production this[int index]
        {
            get
            {
                return (Production)this.m_Array[index];
            }

            internal set
            {
                this.m_Array[index] = value;
            }
        }

        [Description("Returns the total number of productions in the list.")]
        public int Count()
        {
            return this.m_Array.Count;
        }

        internal int Add(Production item)
        {
            return this.m_Array.Add(item);
        }

        internal void Clear()
        {
            this.m_Array.Clear();
        }

        internal void ReDimension(int size)
        {
            // Increase the size of the array to Size empty elements.
            int n;

            this.m_Array.Clear();
            for (n = 0; n <= size - 1; n++)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                this.m_Array.Add(null);
            }
        }
    }
}