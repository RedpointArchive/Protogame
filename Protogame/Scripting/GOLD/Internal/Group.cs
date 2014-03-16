// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD.Internal
{
    internal class Group
    {
        internal Group()
        {
            this.Advance = AdvanceMode.Character;
            this.Ending = EndingMode.Closed;
            this.Nesting = new IntegerList();
        }

        public enum AdvanceMode
        {
            Token = 0, 

            Character = 1
        }

        public enum EndingMode
        {
            Open = 0, 

            Closed = 1
        }

        internal AdvanceMode Advance { get; set; }

        internal Symbol Container { get; set; }

        internal Symbol End { get; set; }

        internal EndingMode Ending { get; set; }

        internal string Name { get; set; }

        internal IntegerList Nesting { get; set; }

        internal Symbol Start { get; set; }

        internal short TableIndex { get; set; }
    }
}