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

    public class SymbolList
    {
        // CANNOT inherit, must hide methods that edit the list
        private readonly ArrayList m_Array;

        internal SymbolList()
        {
            this.m_Array = new ArrayList();
        }

        internal SymbolList(int size)
        {
            this.m_Array = new ArrayList();
            this.ReDimension(size);
        }

        [Description("Returns the symbol with the specified index.")]
        public Symbol this[int index]
        {
            get
            {
                if (index >= 0 & index < this.m_Array.Count)
                {
                    return (Symbol)this.m_Array[index];
                }

                return null;
            }

            internal set
            {
                this.m_Array[index] = value;
            }
        }

        [Description("Returns the total number of symbols in the list.")]
        public int Count()
        {
            return this.m_Array.Count;
        }

        [Description("Returns a list of the symbol names in BNF format.")]
        public string Text(string separator, bool alwaysDelimitTerminals)
        {
            var result = string.Empty;
            int n;

            for (n = 0; n <= this.m_Array.Count - 1; n++)
            {
                var sym = (Symbol)this.m_Array[n];
                result += (n == 0 ? string.Empty : separator) + sym.Text(alwaysDelimitTerminals);
            }

            return result;
        }

        [Description("Returns a list of the symbol names in BNF format.")]
        public string Text()
        {
            return this.Text(", ", false);
        }

        public override string ToString()
        {
            return this.Text();
        }

        internal int Add(Symbol item)
        {
            return this.m_Array.Add(item);
        }

        internal void Clear()
        {
            this.m_Array.Clear();
        }

        internal Symbol GetFirstOfType(SymbolType type)
        {
            Symbol result = null;

            var found = false;
            short n = 0;
            while ((!found) & n < this.m_Array.Count)
            {
                var sym = (Symbol)this.m_Array[n];
                if (sym.Type == type)
                {
                    found = true;
                    result = sym;
                }

                n += 1;
            }

            return result;
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