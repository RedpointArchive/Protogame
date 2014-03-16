// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD.Internal
{
    using System.Collections;

    internal class TokenQueueStack
    {
        private readonly ArrayList m_Items;

        public TokenQueueStack()
        {
            this.m_Items = new ArrayList();
        }

        internal int Count
        {
            get
            {
                return this.m_Items.Count;
            }
        }

        public void Clear()
        {
            this.m_Items.Clear();
        }

        public Token Dequeue()
        {
            var result = (Token)this.m_Items[0];

            // Front of list
            this.m_Items.RemoveAt(0);

            return result;
        }

        public void Enqueue(ref Token theToken)
        {
            this.m_Items.Add(theToken);

            // End of list
        }

        public Token Pop()
        {
            return this.Dequeue();

            // Same as dequeue
        }

        public void Push(Token theToken)
        {
            this.m_Items.Insert(0, theToken);
        }

        public Token Top()
        {
            if (this.m_Items.Count >= 1)
            {
                return (Token)this.m_Items[0];
            }

            return null;
        }
    }
}