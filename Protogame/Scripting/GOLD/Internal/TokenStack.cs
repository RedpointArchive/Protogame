// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD.Internal
{
    using System.Collections;

    internal class TokenStack
    {
        private readonly Stack m_Stack;

        public TokenStack()
        {
            this.m_Stack = new Stack();
        }

        internal int Count
        {
            get
            {
                return this.m_Stack.Count;
            }
        }

        public void Clear()
        {
            this.m_Stack.Clear();
        }

        public Token Pop()
        {
            return (Token)this.m_Stack.Pop();
        }

        public void Push(ref Token theToken)
        {
            this.m_Stack.Push(theToken);
        }

        public Token Top()
        {
            return (Token)this.m_Stack.Peek();
        }
    }
}