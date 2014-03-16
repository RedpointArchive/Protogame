// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD
{
    using System.ComponentModel;

    public class Production
    {
        private readonly SymbolList m_Handle;

        private readonly Symbol m_Head;

        private readonly short m_TableIndex;

        internal Production(Symbol head, short tableIndex)
        {
            this.m_Head = head;
            this.m_Handle = new SymbolList();
            this.m_TableIndex = tableIndex;
        }

        internal Production()
        {
            // Nothing
        }

        [Description("Returns the symbol list containing the handle (body) of the production.")]
        public SymbolList Handle()
        {
            return this.m_Handle;
        }

        [Description("Returns the head of the production.")]
        public Symbol Head()
        {
            return this.m_Head;
        }

        [Description("Returns the index of the production in the Production Table.")]
        public short TableIndex()
        {
            return this.m_TableIndex;
        }

        [Description("Returns the production in BNF.")]
        public string Text(bool alwaysDelimitTerminals = false)
        {
            return this.m_Head.Text() + " ::= " + this.m_Handle.Text(" ", alwaysDelimitTerminals);
        }

        public override string ToString()
        {
            return this.Text();
        }

        internal bool ContainsOneNonTerminal()
        {
            var result = false;

            if (this.m_Handle.Count() != 1)
            {
                return false;
            }
            
            if (this.m_Handle[0].Type == SymbolType.Nonterminal)
            {
                result = true;
            }

            return result;
        }
    }
}