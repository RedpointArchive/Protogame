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

    public class TokenList
    {
        // Don't inherit - hide array modifying methods
        private readonly ArrayList m_Array;

        internal TokenList()
        {
            this.m_Array = new ArrayList();
        }

        [Description("Returns the token with the specified index.")]
        public Token this[int index]
        {
            get
            {
                return (Token)this.m_Array[index];
            }

            internal set
            {
                this.m_Array[index] = value;
            }
        }

        [Description("Returns the total number of tokens in the list.")]
        public int Count()
        {
            return this.m_Array.Count;
        }

        internal int Add(Token item)
        {
            return this.m_Array.Add(item);
        }

        internal void Clear()
        {
            this.m_Array.Clear();
        }
    }
}