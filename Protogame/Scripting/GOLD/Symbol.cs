// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD
{
    using System.ComponentModel;
    using GOLD.Internal;

    public class Symbol
    {
        private readonly string m_Name;

        private readonly short m_TableIndex;

        private SymbolType m_Type;

        internal Symbol()
        {
            // Nothing
        }

        internal Symbol(string name, SymbolType type, short tableIndex)
        {
            this.m_Name = name;
            this.m_Type = type;
            this.m_TableIndex = tableIndex;
        }

        [Description("Returns the type of the symbol.")]
        public SymbolType Type
        {
            get
            {
                return this.m_Type;
            }

            internal set
            {
                this.m_Type = value;
            }
        }

        internal Group Group { get; set; }

        [Description("Returns the name of the symbol.")]
        public string Name()
        {
            return this.m_Name;
        }

        [Description("Returns the index of the symbol in the Symbol Table,")]
        public short TableIndex()
        {
            return this.m_TableIndex;
        }

        [Description("Returns the text representing the text in BNF format.")]
        public string Text(bool alwaysDelimitTerminals)
        {
            string result;

            switch (this.m_Type)
            {
                case SymbolType.Nonterminal:
                    result = "<" + this.Name() + ">";
                    break;
                case SymbolType.Content:
                    result = this.LiteralFormat(this.Name(), alwaysDelimitTerminals);
                    break;
                default:
                    result = "(" + this.Name() + ")";
                    break;
            }

            return result;
        }

        [Description("Returns the text representing the text in BNF format.")]
        public string Text()
        {
            return this.Text(false);
        }

        public override string ToString()
        {
            return this.Text();
        }

        private string LiteralFormat(string source, bool forceDelimit)
        {
            if (source == "'")
            {
                return "''";
            }

            short n = 0;
            while (n < source.Length & (!forceDelimit))
            {
                var ch = source[n];
                forceDelimit = !(char.IsLetter(ch) | ch == '.' | ch == '_' | ch == '-');
                n += 1;
            }

            if (forceDelimit)
            {
                return "'" + source + "'";
            }

            return source;
        }
    }
}