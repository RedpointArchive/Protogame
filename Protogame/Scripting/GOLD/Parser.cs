// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using GOLD.Internal;

    public class Parser
    {
        private const string KVersion = "5.0";

        private readonly Position m_CurrentPosition = new Position();

        // ===== Symbols recognized by the system

        // ===== Used for Reductions & Errors
        // This ENTIRE list will available to the user
        private readonly SymbolList m_ExpectedSymbols = new SymbolList();

        private readonly TokenStack m_GroupStack = new TokenStack();

        // Tokens to be analyzed - Hybred object!
        private readonly TokenQueueStack m_InputTokens = new TokenQueueStack();

        private readonly TokenStack m_Stack = new TokenStack();

        // === Line and column information. 
        // Internal - so user cannot mess with values
        private readonly Position m_SysPosition = new Position();

        private CharacterSetList m_CharSetTable = new CharacterSetList();

        private int m_CurrentLALR;

        private FAStateList m_DFA = new FAStateList();

        // Last read terminal

        // ===== Grammar Attributes
        private GrammarProperties m_Grammar = new GrammarProperties();

        // ===== Lexical Groups
        private GroupList m_GroupTable = new GroupList();

        private bool m_HaveReduction;

        private LRStateList m_LRStates = new LRStateList();

        private string m_LookaheadBuffer;

        // ===== Productions
        private ProductionList m_ProductionTable = new ProductionList();

        private TextReader m_Source;

        private SymbolList m_SymbolTable = new SymbolList();

        private bool m_TablesLoaded;

        private bool m_TrimReductions;

        public Parser()
        {
            this.Restart();
            this.m_TablesLoaded = false;

            // ======= Default Properties
            this.m_TrimReductions = false;
        }

        private enum ParseResult
        {
            Accept = 1, 

            Shift = 2, 

            ReduceNormal = 3, 

            ReduceEliminated = 4, 

            // Trim
            SyntaxError = 5, 

            InternalError = 6
        }

        [Description("When the Parse() method returns a Reduce, this method will contain the current Reduction.")]
        public object CurrentReduction
        {
            get
            {
                return this.m_HaveReduction ? this.m_Stack.Top().Data : null;
            }

            set
            {
                if (this.m_HaveReduction)
                {
                    this.m_Stack.Top().Data = value;
                }
            }
        }

        [Description("Determines if reductions will be trimmed in cases where a production contains a single element.")]
        public bool TrimReductions
        {
            get
            {
                return this.m_TrimReductions;
            }

            set
            {
                this.m_TrimReductions = value;
            }
        }

        [Description("Library name and version.")]
        public string About()
        {
            return "GOLD Parser Engine; Version " + KVersion;
        }

        [Description("Current line and column being read from the source.")]
        public Position CurrentPosition()
        {
            return this.m_CurrentPosition;
        }

        [Description("If the Parse() function returns TokenRead, this method will return that last read token.")]
        public Token CurrentToken()
        {
            return this.m_InputTokens.Top();
        }

        [Description("Removes the next token from the input queue.")]
        public Token DiscardCurrentToken()
        {
            return this.m_InputTokens.Dequeue();
        }

        [Description("Added a token onto the end of the input queue.")]
        public void EnqueueInput(ref Token theToken)
        {
            this.m_InputTokens.Enqueue(ref theToken);
        }

        /// <summary>
        /// If the Parse() method returns a SyntaxError, this method will contain a list of the symbols the grammar expected to see.
        /// </summary>
        public SymbolList ExpectedSymbols()
        {
            return this.m_ExpectedSymbols;
        }

        [Description("Returns information about the current grammar.")]
        public GrammarProperties Grammar()
        {
            return this.m_Grammar;
        }

        [Description("Loads parse tables from the specified filename. Only EGT (version 5.0) is supported.")]
        public bool LoadTables(string path)
        {
            return this.LoadTables(new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)));
        }

        [Description("Loads parse tables from the specified BinaryReader. Only EGT (version 5.0) is supported.")]
        public bool LoadTables(BinaryReader reader)
        {
            var egt = new EGTReader();
            bool success;

            try
            {
                egt.Open(reader);

                this.Restart();
                success = true;
                while (!(egt.EndOfFile() || success == false))
                {
                    egt.GetNextRecord();

                    var recType = (EGTRecord)egt.RetrieveByte();

                    switch (recType)
                    {
                        case EGTRecord.Property:
                        {
                            // Index, Name, Value
                            var index = egt.RetrieveInt16();
                            egt.RetrieveString();

                            // Just discard
                            this.m_Grammar.SetValue(index, egt.RetrieveString());

                            break;
                        }

                        case EGTRecord.TableCounts:
                        {
                            // Symbol, CharacterSet, Rule, DFA, LALR
                            this.m_SymbolTable = new SymbolList(egt.RetrieveInt16());
                            this.m_CharSetTable = new CharacterSetList(egt.RetrieveInt16());
                            this.m_ProductionTable = new ProductionList(egt.RetrieveInt16());
                            this.m_DFA = new FAStateList(egt.RetrieveInt16());
                            this.m_LRStates = new LRStateList(egt.RetrieveInt16());
                            this.m_GroupTable = new GroupList(egt.RetrieveInt16());

                            break;
                        }

                        case EGTRecord.InitialStates:
                        {
                            // DFA, LALR
                            this.m_DFA.InitialState = (short)egt.RetrieveInt16();
                            this.m_LRStates.InitialState = (short)egt.RetrieveInt16();

                            break;
                        }

                        case EGTRecord.Symbol:
                        {
                            // #, Name, Kind
                            var index = (short)egt.RetrieveInt16();
                            var name = egt.RetrieveString();
                            var type = (SymbolType)egt.RetrieveInt16();

                            this.m_SymbolTable[index] = new Symbol(name, type, index);

                            break;
                        }

                        case EGTRecord.Group:
                        {
                            // #, Name, Container#, Start#, End#, Tokenized, Open Ended, Reserved, Count, (Nested Group #...) 
                            var g = new Group();
                            int n;

                            var with1 = g;
                            var index = egt.RetrieveInt16();

                            // # 
                            with1.Name = egt.RetrieveString();
                            with1.Container = this.SymbolTable()[egt.RetrieveInt16()];
                            with1.Start = this.SymbolTable()[egt.RetrieveInt16()];
                            with1.End = this.SymbolTable()[egt.RetrieveInt16()];

                            with1.Advance = (Group.AdvanceMode)egt.RetrieveInt16();
                            with1.Ending = (Group.EndingMode)egt.RetrieveInt16();
                            egt.RetrieveEntry();

                            // Reserved
                            var count = egt.RetrieveInt16();
                            for (n = 1; n <= count; n++)
                            {
                                with1.Nesting.Add(egt.RetrieveInt16());
                            }

                            // === Link back
                            g.Container.Group = g;
                            g.Start.Group = g;
                            g.End.Group = g;

                            this.m_GroupTable[index] = g;

                            break;
                        }

                        case EGTRecord.CharRanges:
                        {
                            // #, Total Sets, RESERVED, (Start#, End#  ...)
                            var index = egt.RetrieveInt16();
                            egt.RetrieveInt16();

                            // Codepage
                            egt.RetrieveInt16();
                            egt.RetrieveEntry();

                            // Reserved
                            this.m_CharSetTable[index] = new CharacterSet();
                            while (!egt.RecordComplete())
                            {
                                this.m_CharSetTable[index].Add(
                                    new CharacterRange((ushort)egt.RetrieveInt16(), (ushort)egt.RetrieveInt16()));
                            }

                            break;
                        }

                        case EGTRecord.Production:
                        {
                            // #, ID#, Reserved, (Symbol#,  ...)
                            var index = egt.RetrieveInt16();
                            var headIndex = egt.RetrieveInt16();
                            egt.RetrieveEntry();

                            // Reserved
                            this.m_ProductionTable[index] = new Production(this.m_SymbolTable[headIndex], (short)index);

                            while (!egt.RecordComplete())
                            {
                                var symIndex = egt.RetrieveInt16();
                                this.m_ProductionTable[index].Handle().Add(this.m_SymbolTable[symIndex]);
                            }

                            break;
                        }

                        case EGTRecord.DFAState:
                        {
                            // #, Accept?, Accept#, Reserved (CharSet#, Target#, Reserved)...
                            var index = egt.RetrieveInt16();
                            var accept = egt.RetrieveBoolean();
                            var acceptIndex = egt.RetrieveInt16();
                            egt.RetrieveEntry();

                            // Reserved
                            if (accept)
                            {
                                this.m_DFA[index] = new FAState(this.m_SymbolTable[acceptIndex]);
                            }
                            else
                            {
                                this.m_DFA[index] = new FAState();
                            }

                            // (Edge chars, Target#, Reserved)...
                            while (!egt.RecordComplete())
                            {
                                var setIndex = egt.RetrieveInt16();

                                // Char table index
                                var target = egt.RetrieveInt16();

                                // Target
                                egt.RetrieveEntry();

                                // Reserved
                                this.m_DFA[index].Edges.Add(new FAEdge(this.m_CharSetTable[setIndex], target));
                            }

                            break;
                        }

                        case EGTRecord.LRState:
                        {
                            // #, Reserved (Symbol#, Action, Target#, Reserved)...
                            var index = egt.RetrieveInt16();
                            egt.RetrieveEntry();

                            // Reserved
                            this.m_LRStates[index] = new LRState();

                            // (Symbol#, Action, Target#, Reserved)...
                            while (!egt.RecordComplete())
                            {
                                var symIndex = egt.RetrieveInt16();
                                var action = egt.RetrieveInt16();
                                var target = egt.RetrieveInt16();
                                egt.RetrieveEntry();

                                // Reserved
                                this.m_LRStates[index].Add(
                                    new LRAction(this.m_SymbolTable[symIndex], (LRActionType)action, (short)target));
                            }

                            break;
                        }

                        default:

                            // RecordIDComment
                            throw new ParserException(
                                "File Error. A record of type '" + (char)recType
                                + "' was read. This is not a valid code.");
                    }
                }

                egt.Close();
            }
            catch (Exception ex)
            {
                throw new ParserException(ex.Message, ex, "LoadTables");
            }

            this.m_TablesLoaded = success;

            return success;
        }

        [Description("Opens a string for parsing.")]
        public bool Open(ref string text)
        {
            return this.Open(new StringReader(text));
        }

        [Description("Opens a text stream for parsing.")]
        public bool Open(TextReader reader)
        {
            var start = new Token();

            this.Restart();
            this.m_Source = reader;

            // === Create stack top item. Only needs state
            start.State = this.m_LRStates.InitialState;
            this.m_Stack.Push(ref start);

            return true;
        }

        /// <summary>
        /// Performs a parse action on the input. This method is typically used in a loop until either grammar is accepted or an error occurs.
        /// </summary>
        public ParseMessage Parse()
        {
            var message = default(ParseMessage);

            if (!this.m_TablesLoaded)
            {
                return ParseMessage.NotLoadedError;
            }

            // ===================================
            // Loop until breakable event
            // ===================================
            var done = false;
            while (!done)
            {
                Token read;
                if (this.m_InputTokens.Count == 0)
                {
                    read = this.ProduceToken();
                    this.m_InputTokens.Push(read);

                    message = ParseMessage.TokenRead;
                    done = true;
                }
                else
                {
                    read = this.m_InputTokens.Top();
                    this.m_CurrentPosition.Copy(read.Position());

                    // Update current position

                    // Runaway group
                    if (this.m_GroupStack.Count != 0)
                    {
                        message = ParseMessage.GroupError;
                        done = true;
                    }
                    else if (read.Type() == SymbolType.Noise)
                    {
                        // Just discard. These were already reported to the user.
                        this.m_InputTokens.Pop();
                    }
                    else if (read.Type() == SymbolType.Error)
                    {
                        message = ParseMessage.LexicalError;
                        done = true;

                        // Finally, we can parse the token.
                    }
                    else
                    {
                        var action = this.ParseLALR(ref read);

                        // SAME PROCEDURE AS v1
                        switch (action)
                        {
                            case ParseResult.Accept:
                                message = ParseMessage.Accept;
                                done = true;

                                break;
                            case ParseResult.InternalError:
                                message = ParseMessage.InternalError;
                                done = true;

                                break;
                            case ParseResult.ReduceNormal:
                                message = ParseMessage.Reduction;
                                done = true;

                                break;
                            case ParseResult.Shift:

                                // ParseToken() shifted the token on the front of the Token-Queue. 
                                // It now exists on the Token-Stack and must be eliminated from the queue.
                                this.m_InputTokens.Dequeue();

                                break;
                            case ParseResult.SyntaxError:
                                message = ParseMessage.SyntaxError;
                                done = true;

                                break;
                        }
                    }
                }
            }

            return message;
        }

        [Description("Returns a list of Productions recognized by the grammar.")]
        public ProductionList ProductionTable()
        {
            return this.m_ProductionTable;
        }

        [Description("Pushes the token onto the top of the input queue. This token will be analyzed next.")]
        public void PushInput(ref Token theToken)
        {
            this.m_InputTokens.Push(theToken);
        }

        [Description("Restarts the parser. Loaded tables are retained.")]
        public void Restart()
        {
            this.m_CurrentLALR = this.m_LRStates.InitialState;

            // === Lexer
            this.m_SysPosition.Column = 0;
            this.m_SysPosition.Line = 0;
            this.m_CurrentPosition.Line = 0;
            this.m_CurrentPosition.Column = 0;

            this.m_HaveReduction = false;

            this.m_ExpectedSymbols.Clear();
            this.m_InputTokens.Clear();
            this.m_Stack.Clear();
            this.m_LookaheadBuffer = string.Empty;

            // ==== V4
            this.m_GroupStack.Clear();
        }

        [Description("Returns a list of Symbols recognized by the grammar.")]
        public SymbolList SymbolTable()
        {
            return this.m_SymbolTable;
        }

        [Description("Returns true if parse tables were loaded.")]
        public bool TablesLoaded()
        {
            return this.m_TablesLoaded;
        }

        internal void Clear()
        {
            this.m_SymbolTable.Clear();
            this.m_ProductionTable.Clear();
            this.m_CharSetTable.Clear();
            this.m_DFA.Clear();
            this.m_LRStates.Clear();

            this.m_Stack.Clear();
            this.m_InputTokens.Clear();

            this.m_Grammar = new GrammarProperties();

            this.m_GroupStack.Clear();
            this.m_GroupTable.Clear();

            this.Restart();
        }

        private void ConsumeBuffer(int charCount)
        {
            // Consume/Remove the characters from the front of the buffer. 
            if (charCount > this.m_LookaheadBuffer.Length)
            {
                return;
            }

            // Count Carriage Returns and increment the internal column and line
            // numbers. This is done for the Developer and is not necessary for the
            // DFA algorithm.
            int n;
            for (n = 0; n <= charCount - 1; n++)
            {
                switch (this.m_LookaheadBuffer[n])
                {
                    case '\n':
                        this.m_SysPosition.Line += 1;
                        this.m_SysPosition.Column = 0;
                        break;
                    case '\r':
                        break;

                        // Ignore, LF is used to inc line to be UNIX friendly
                    default:
                        this.m_SysPosition.Column += 1;
                        break;
                }
            }

            this.m_LookaheadBuffer = this.m_LookaheadBuffer.Remove(0, charCount);
        }

        private string Lookahead(int charIndex)
        {
            // Return single char at the index. This function will also increase 
            // buffer if the specified character is not present. It is used 
            // by the DFA algorithm.

            // Check if we must read characters from the Stream
            if (charIndex > this.m_LookaheadBuffer.Length)
            {
                var readCount = charIndex - this.m_LookaheadBuffer.Length;
                int n;
                for (n = 1; n <= readCount; n++)
                {
                    this.m_LookaheadBuffer += (char)this.m_Source.Read();
                }
            }

            // If the buffer is still smaller than the index, we have reached
            // the end of the text. In this case, return a null string - the DFA
            // code will understand.
            return charIndex <= this.m_LookaheadBuffer.Length ? this.m_LookaheadBuffer[charIndex - 1].ToString(CultureInfo.InvariantCulture) : string.Empty;
        }

        private string LookaheadBuffer(int count)
        {
            // Return Count characters from the lookahead buffer. DO NOT CONSUME
            // This is used to create the text stored in a token. It is disgarded
            // separately. Because of the design of the DFA algorithm, count should
            // never exceed the buffer length. The If-Statement below is fault-tolerate
            // programming, but not necessary.
            if (count > this.m_LookaheadBuffer.Length)
            {
                count = Convert.ToInt32(this.m_LookaheadBuffer);
            }

            return this.m_LookaheadBuffer.Substring(0, count);
        }

        private Token LookaheadDFA()
        {
            // This function implements the DFA for th parser's lexer.
            // It generates a token which is used by the LALR state
            // machine.
            var target = 0;
            var result = new Token();

            // ===================================================
            // Match DFA token
            // ===================================================
            var done = false;
            int currentDFA = this.m_DFA.InitialState;
            var currentPosition = 1;

            // Next byte in the input Stream
            var lastAcceptState = -1;

            // We have not yet accepted a character string
            var lastAcceptPosition = -1;

            var ch = this.Lookahead(1);

            // NO MORE DATA
            // ReSharper disable once PossibleNullReferenceException
            if (!(string.IsNullOrEmpty(ch) | ch[0] == 65535))
            {
                while (!done)
                {
                    // This code searches all the branches of the current DFA state
                    // for the next character in the input Stream. If found the
                    // target state is returned.
                    ch = this.Lookahead(currentPosition);

                    // End reached, do not match
                    bool found;
                    if (string.IsNullOrEmpty(ch))
                    {
                        found = false;
                    }
                    else
                    {
                        var n = 0;
                        found = false;
                        while (n < this.m_DFA[currentDFA].Edges.Count & !found)
                        {
                            var edge = this.m_DFA[currentDFA].Edges[n];

                            // ==== Look for character in the Character Set Table
                            if (edge.Characters.Contains(ch[0]))
                            {
                                found = true;
                                target = edge.Target;

                                // .TableIndex
                            }

                            n += 1;
                        }
                    }

                    // This block-if statement checks whether an edge was found from the current state. If so, the state and current
                    // position advance. Otherwise it is time to exit the main loop and report the token found (if there was one). 
                    // If the LastAcceptState is -1, then we never found a match and the Error Token is created. Otherwise, a new 
                    // token is created using the Symbol in the Accept State and all the characters that comprise it.
                    if (found)
                    {
                        // This code checks whether the target state accepts a token.
                        // If so, it sets the appropiate variables so when the
                        // algorithm in done, it can return the proper token and
                        // number of characters.

                        // NOT is very important!
                        if (this.m_DFA[target].Accept != null)
                        {
                            lastAcceptState = target;
                            lastAcceptPosition = currentPosition;
                        }

                        currentDFA = target;
                        currentPosition += 1;

                        // No edge found
                    }
                    else
                    {
                        done = true;

                        // Lexer cannot recognize symbol
                        if (lastAcceptState == -1)
                        {
                            result.Parent = this.m_SymbolTable.GetFirstOfType(SymbolType.Error);
                            result.Data = this.LookaheadBuffer(1);

                            // Create Token, read characters
                        }
                        else
                        {
                            result.Parent = this.m_DFA[lastAcceptState].Accept;
                            result.Data = this.LookaheadBuffer(lastAcceptPosition);

                            // Data contains the total number of accept characters
                        }
                    }

                    // DoEvents
                }
            }
            else
            {
                // End of file reached, create End Token
                result.Data = string.Empty;
                result.Parent = this.m_SymbolTable.GetFirstOfType(SymbolType.End);
            }

            // ===================================================
            // Set the new token's position information
            // ===================================================
            // Notice, this is a copy, not a linking of an instance. We don't want the user 
            // to be able to alter the main value indirectly.
            result.Position().Copy(this.m_SysPosition);

            return result;
        }

        private ParseResult ParseLALR(ref Token nextToken)
        {
            // This function analyzes a token and either:
            // 1. Makes a SINGLE reduction and pushes a complete Reduction object on the m_Stack
            // 2. Accepts the token and shifts
            // 3. Errors and places the expected symbol indexes in the Tokens list
            // The Token is assumed to be valid and WILL be checked
            // If an action is performed that requires controlt to be returned to the user, the function returns true.
            // The Message parameter is then set to the type of action.
            var result = default(ParseResult);
            var parseAction = this.m_LRStates[this.m_CurrentLALR][nextToken.Parent];

            // Work - shift or reduce
            if (parseAction != null)
            {
                this.m_HaveReduction = false;

                // Will be set true if a reduction is made
                // 'Debug.WriteLine("Action: " & ParseAction.Text)
                switch (parseAction.Type)
                {
                    case LRActionType.Accept:
                        this.m_HaveReduction = true;
                        result = ParseResult.Accept;

                        break;
                    case LRActionType.Shift:
                        this.m_CurrentLALR = parseAction.Value;
                        nextToken.State = (short)this.m_CurrentLALR;
                        this.m_Stack.Push(ref nextToken);
                        result = ParseResult.Shift;

                        break;
                    case LRActionType.Reduce:

                        // Produce a reduction - remove as many tokens as members in the rule & push a nonterminal token
                        var prod = this.m_ProductionTable[parseAction.Value];

                        // ======== Create Reduction
                        Token head;
                        short n;
                        if (this.m_TrimReductions && prod.ContainsOneNonTerminal())
                        {
                            // The current rule only consists of a single nonterminal and can be trimmed from the
                            // parse tree. Usually we create a new Reduction, assign it to the Data property
                            // of Head and push it on the m_Stack. However, in this case, the Data property of the
                            // Head will be assigned the Data property of the reduced token (i.e. the only one
                            // on the m_Stack).
                            // In this case, to save code, the value popped of the m_Stack is changed into the head.
                            head = this.m_Stack.Pop();
                            head.Parent = prod.Head();

                            result = ParseResult.ReduceEliminated;

                            // Build a Reduction
                        }
                        else
                        {
                            this.m_HaveReduction = true;
                            var newReduction = new Reduction(prod.Handle().Count());

                            var with2 = newReduction;
                            with2.Parent = prod;
                            for (n = (short)(prod.Handle().Count() - 1); n >= 0; n += -1)
                            {
                                with2[n] = this.m_Stack.Pop();
                            }

                            head = new Token(prod.Head(), newReduction);
                            result = ParseResult.ReduceNormal;
                        }

                        // ========== Goto
                        var index = this.m_Stack.Top().State;

                        // ========= If n is -1 here, then we have an Internal Table Error!!!!
                        n = this.m_LRStates[index].IndexOf(prod.Head());
                        if (n != -1)
                        {
                            this.m_CurrentLALR = this.m_LRStates[index][n].Value;

                            head.State = (short)this.m_CurrentLALR;
                            this.m_Stack.Push(ref head);
                        }
                        else
                        {
                            result = ParseResult.InternalError;
                        }

                        break;
                }
            }
            else
            {
                // === Syntax Error! Fill Expected Tokens
                this.m_ExpectedSymbols.Clear();

                // .Count - 1
                foreach (LRAction action in this.m_LRStates[this.m_CurrentLALR])
                {
                    switch (action.Symbol.Type)
                    {
                        case SymbolType.Content:
                        case SymbolType.End:
                        case SymbolType.GroupStart:
                        case SymbolType.GroupEnd:
                            this.m_ExpectedSymbols.Add(action.Symbol);
                            break;
                    }
                }

                result = ParseResult.SyntaxError;
            }

            return result;

            // Very important
        }

        private Token ProduceToken()
        {
            // ** VERSION 5.0 **
            // This function creates a token and also takes into account the current
            // lexing mode of the parser. In particular, it contains the group logic. 
            // A stack is used to track the current "group". This replaces the comment
            // level counter. Also, text is appended to the token on the top of the 
            // stack. This allows the group text to returned in one chunk.
            var done = false;
            Token result = null;

            while (!done)
            {
                var read = this.LookaheadDFA();

                // The logic - to determine if a group should be nested - requires that the top of the stack 
                // and the symbol's linked group need to be looked at. Both of these can be unset. So, this section
                // sets a Boolean and avoids errors. We will use this boolean in the logic chain below. 
                bool nestGroup;
                if (read.Type() == SymbolType.GroupStart)
                {
                    nestGroup = this.m_GroupStack.Count == 0
                                || this.m_GroupStack.Top().Group().Nesting.Contains(read.Group().TableIndex);
                }
                else
                {
                    nestGroup = false;
                }

                // =================================
                // Logic chain
                // =================================
                if (nestGroup)
                {
                    this.ConsumeBuffer(read.Data.Length);
                    this.m_GroupStack.Push(ref read);
                }
                else if (this.m_GroupStack.Count == 0)
                {
                    // The token is ready to be analyzed.             
                    this.ConsumeBuffer(read.Data.Length);
                    result = read;
                    done = true;
                }
                else if (ReferenceEquals(this.m_GroupStack.Top().Group().End, read.Parent))
                {
                    // End the current group
                    var pop = this.m_GroupStack.Pop();

                    // === Ending logic
                    if (pop.Group().Ending == Group.EndingMode.Closed)
                    {
                        pop.Data += read.Data;

                        // Append text
                        this.ConsumeBuffer(read.Data.Length);

                        // Consume token
                    }

                    // We are out of the group. Return pop'd token (which contains all the group text)
                    if (this.m_GroupStack.Count == 0)
                    {
                        pop.Parent = pop.Group().Container;

                        // Change symbol to parent
                        result = pop;
                        done = true;
                    }
                    else
                    {
                        this.m_GroupStack.Top().Data += pop.Data;

                        // Append group text to parent
                    }
                }
                else if (read.Type() == SymbolType.End)
                {
                    // EOF always stops the loop. The caller function (Parse) can flag a runaway group error.
                    result = read;
                    done = true;
                }
                else
                {
                    // We are in a group, Append to the Token on the top of the stack.
                    // Take into account the Token group mode  
                    var top = this.m_GroupStack.Top();

                    if (top.Group().Advance == Group.AdvanceMode.Token)
                    {
                        top.Data += read.Data;

                        // Append all text
                        this.ConsumeBuffer(read.Data.Length);
                    }
                    else
                    {
                        top.Data += read.Data[0];

                        // Append one character
                        this.ConsumeBuffer(1);
                    }
                }
            }

            return result;
        }
    }
}