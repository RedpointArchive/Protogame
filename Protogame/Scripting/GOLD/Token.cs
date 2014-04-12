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

    public class Token
    {
        private readonly Position m_Position = new Position();

        private object m_Data;

        private Symbol m_Parent;

        public Token(Symbol parent, object data)
        {
            this.m_Parent = parent;
            this.m_Data = data;
            this.State = 0;
        }

        internal Token()
        {
            this.m_Parent = null;
            this.m_Data = null;
            this.State = 0;
        }

        [Description("Returns/sets the object associated with the token.")]
        public object Data
        {
            get
            {
                return this.m_Data;
            }

            set
            {
                this.m_Data = value;
            }
        }

        [Description("Returns the parent symbol of the token.")]
        public Symbol Parent
        {
            get
            {
                return this.m_Parent;
            }

            internal set
            {
                this.m_Parent = value;
            }
        }

        internal short State { get; set; }

        [Description("Returns the line/column position where the token was read.")]
        public Position Position()
        {
            return this.m_Position;
        }

        [Description("Returns the symbol type associated with this token.")]
        public SymbolType Type()
        {
            return this.m_Parent.Type;
        }

        internal Group Group()
        {
            return this.m_Parent.Group;
        }
    }
}