// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD
{
    public class DataIndex
    {
        private readonly TokenList m_TokenList;

        public DataIndex(TokenList tokenList)
        {
            this.m_TokenList = tokenList;
        }

        public object this[int index]
        {
            get
            {
                return this.m_TokenList[index].Data;
            }

            set
            {
                this.m_TokenList[index].Data = value;
            }
        }
    }
}