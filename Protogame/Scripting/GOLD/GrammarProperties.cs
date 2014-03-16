// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD
{
    public class GrammarProperties
    {
        private const int PropertyCount = 8;

        private readonly string[] m_Property = new string[PropertyCount + 1];

        internal GrammarProperties()
        {
            int n;

            for (n = 0; n <= PropertyCount - 1; n++)
            {
                this.m_Property[n] = string.Empty;
            }
        }

        private enum PropertyIndex
        {
            Name = 0, 

            Version = 1, 

            Author = 2, 

            About = 3, 

            CharacterSet = 4, 

            CharacterMapping = 5, 

            GeneratedBy = 6, 

            GeneratedDate = 7
        }

        public string About
        {
            get
            {
                return this.m_Property[(int)PropertyIndex.About];
            }
        }

        public string Author
        {
            get
            {
                return this.m_Property[(int)PropertyIndex.Author];
            }
        }

        public string CharacterMapping
        {
            get
            {
                return this.m_Property[(int)PropertyIndex.CharacterMapping];
            }
        }

        public string CharacterSet
        {
            get
            {
                return this.m_Property[(int)PropertyIndex.CharacterSet];
            }
        }

        public string GeneratedBy
        {
            get
            {
                return this.m_Property[(int)PropertyIndex.GeneratedBy];
            }
        }

        public string GeneratedDate
        {
            get
            {
                return this.m_Property[(int)PropertyIndex.GeneratedDate];
            }
        }

        public string Name
        {
            get
            {
                return this.m_Property[(int)PropertyIndex.Name];
            }
        }

        public string Version
        {
            get
            {
                return this.m_Property[(int)PropertyIndex.Version];
            }
        }

        internal void SetValue(int index, string value)
        {
            if (index >= 0 & index < PropertyCount)
            {
                this.m_Property[index] = value;
            }
        }
    }
}