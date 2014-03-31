using System;
using System.Collections.Generic;

namespace LogicControl
{
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using GOLD;

    public class LogicParser
    {
        public List<LogicStructure> Structures { get; private set; }

        public List<LogicFunction> Functions { get; private set; }

        public void Parse(string code)
        {
            this.Structures = new List<LogicStructure>();
            this.Functions = new List<LogicFunction>();

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Protogame.Scripting.LogicControl.Grammar.egt");

            var parser = new Parser();
            if (stream != null)
            {
                using (var reader = new BinaryReader(stream))
                {
                    parser.LoadTables(reader);
                }
            }
            else
            {
                throw new InvalidOperationException();
            }

            parser.Open(ref code);

            Reduction root = null;
            var done = false;
            while (!done)
            {
                var response = parser.Parse();

                switch (response)
                {
                    case GOLD.ParseMessage.LexicalError:
                        throw new Exception("Lexical Error:\n" +
                                      "Position: " + parser.CurrentPosition().Line + ", " + parser.CurrentPosition().Column + "\n" +
                                      "Read: " + parser.CurrentToken().Data);
                    case GOLD.ParseMessage.SyntaxError:
                        throw new Exception("Syntax Error:\n" +
                                      "Position: " + parser.CurrentPosition().Line + ", " + parser.CurrentPosition().Column + "\n" +
                                      "Read: " + parser.CurrentToken().Data + "\n" +
                                      "Expecting: " + parser.ExpectedSymbols().Text());
                    case GOLD.ParseMessage.Accept:
                        root = (GOLD.Reduction)parser.CurrentReduction;
                        done = true;
                        break;

                    case GOLD.ParseMessage.InternalError:
                        throw new Exception("Internal error");
                    case GOLD.ParseMessage.NotLoadedError:
                        throw new Exception("Tables not loaded");
                    case GOLD.ParseMessage.GroupError:
                        throw new Exception("Runaway group");
                }
            }

            this.TraverseNodes(root);
        }

        private void TraverseNodes(Reduction root)
        {
            while (true)
            {
                var globalStatement = (Reduction)root[0].Data;
                var declaration = (Reduction)globalStatement[0].Data;

                switch (declaration.Parent.Head().Name())
                {
                    case "StructureDeclaration":
                        this.TraverseStructureDeclaration(declaration);
                        break;
                    case "FunctionDeclaration":
                        this.TraverseFunctionDeclaration(declaration);
                        break;
                }

                if (root.Count() == 1)
                {
                    return;
                }

                root = (Reduction)root[1].Data;
            }
        }

        private void TraverseStructureDeclaration(Reduction structureDeclaration)
        {
            var root = structureDeclaration;

            var name = (string)root[1].Data;
            var fields = (Reduction)root[3].Data;

            var logicFields = new List<LogicField>();

            Reduction field;

            while (fields.Count() == 2)
            {
                field = (Reduction)fields[0].Data;

                logicFields.Add(new LogicField
                {
                    Name = (string)field[1].Data,
                    Type = (string)((Reduction)field[0].Data)[0].Data,
                    Semantic = (string)((Reduction)field[2].Data)[1].Data
                });

                fields = (Reduction)fields[1].Data;
            }

            field = (Reduction)fields[0].Data;

            logicFields.Add(new LogicField
            {
                Name = (string)field[1].Data,
                Type = (string)((Reduction)field[0].Data)[0].Data,
                Semantic = (string)((Reduction)field[2].Data)[1].Data
            });

            this.Structures.Add(new LogicStructure { Name = name, Fields = logicFields });
        }

        private void TraverseFunctionDeclaration(Reduction functionDeclaration)
        {
            var returnType = (string)((Reduction)functionDeclaration[0].Data)[0].Data;
            var name = (string)functionDeclaration[1].Data;
            var parameterDeclarations = (Reduction)functionDeclaration[3].Data;
            var optionalSemantic = (Reduction)functionDeclaration[5].Data;
            var statements = functionDeclaration.Count() >= 9 ? (Reduction)functionDeclaration[7].Data : null;

            this.Functions.Add(
                new LogicFunction
                {
                    Name = name,
                    ReturnSemantic = this.ParseOptionalSemantic(optionalSemantic),
                    ReturnType = returnType,
                    Parameters = this.ParseParameters(parameterDeclarations),
                    Statements = statements == null ? new List<LogicStatement>() : this.ParseStatements(statements) 
                });
        }

        private List<LogicParameter> ParseParameters(Reduction parameterDeclarations)
        {
            var parameters = new List<LogicParameter>();

            if (parameterDeclarations.Count() == 0)
            {
                return parameters;
            }

            while (true)
            {
                var parameter = (Reduction)parameterDeclarations[0].Data;

                var type = (string)((Reduction)parameter[0].Data)[0].Data;
                var name = (string)parameter[1].Data;
                var optionalSemantic = this.ParseOptionalSemantic((Reduction)parameter[2].Data);

                parameters.Add(new LogicParameter
                {
                    Index = parameters.Count,
                    Type = type,
                    Name = name,
                    Semantic = optionalSemantic
                });

                if (parameterDeclarations.Count() == 1)
                {
                    return parameters;
                }

                parameterDeclarations = (Reduction)parameterDeclarations[2].Data;
            }
        }

        private string ParseOptionalSemantic(Reduction optionalSemantic)
        {
            if (optionalSemantic.Count() == 0)
            {
                return null;
            }
            else
            {
                return (string)((Reduction)optionalSemantic[0].Data)[1].Data;
            }
        }

        private List<LogicStatement> ParseStatements(Reduction inputStatements)
        {
            var statements = new List<LogicStatement>();

            while (true)
            {
                var statement = (Reduction)inputStatements[0].Data;

                statements.Add(this.ParseStatement(statement));

                if (inputStatements.Count() == 1)
                {
                    return statements;
                }

                inputStatements = (Reduction)inputStatements[1].Data;
            }
        }

        private LogicStatement ParseBlockStatement(Reduction statement)
        {
            if (statement.Count() != 3)
            {
                return new BlockLogicStatement(new List<LogicStatement>());
            }

            return new BlockLogicStatement(this.ParseStatements((Reduction)statement[1].Data));
        }

        private LogicStatement ParseReturnStatement(Reduction statement)
        {
            return new ReturnLogicStatement(this.ParseExpression((Reduction)statement[1].Data));
        }

        private LogicExpression ParseExpression(Reduction expression)
        {
            if (expression.Count() == 3)
            {
                return new ComparisonLogicExpression(
                    this.ParseExpression((Reduction)expression[0].Data),
                    (string)expression[1].Data,
                    this.ParseAddExpression((Reduction)expression[2].Data));
            }
            
            return this.ParseAddExpression((Reduction)expression[0].Data);
        }

        private LogicExpression ParseAddExpression(Reduction expression)
        {
            if (expression.Count() == 3)
            {
                return new AdditionLogicExpression(
                    this.ParseAddExpression((Reduction)expression[0].Data),
                    (string)expression[1].Data,
                    this.ParseMultiplyExpression((Reduction)expression[2].Data));
            }

            return this.ParseMultiplyExpression((Reduction)expression[0].Data);
        }

        private LogicExpression ParseMultiplyExpression(Reduction expression)
        {
            if (expression.Count() == 3)
            {
                return new MultiplyLogicExpression(
                    this.ParseMultiplyExpression((Reduction)expression[0].Data),
                    (string)expression[1].Data,
                    this.ParseUnaryExpression((Reduction)expression[2].Data));
            }

            return this.ParseUnaryExpression((Reduction)expression[0].Data);
        }

        private LogicExpression ParseUnaryExpression(Reduction expression)
        {
            if (expression.Count() == 2)
            {
                return new UnaryLogicExpression(
                    (string)expression[0].Data,
                    this.ParseLookupExpression((Reduction)expression[1].Data));
            }

            return this.ParseLookupExpression((Reduction)expression[0].Data);
        }

        private LogicExpression ParseLookupExpression(Reduction expression)
        {
            if (expression.Count() == 3)
            {
                return new LookupLogicExpression(
                    this.ParseLookupExpression((Reduction)expression[0].Data),
                    (string)expression[2].Data);
            }

            return this.ParseValueExpression((Reduction)expression[0].Data);
        }

        private LogicExpression ParseValueExpression(Reduction expression)
        {
            if (expression.Count() == 1)
            {
                switch (expression[0].Parent.Name())
                {
                    case "Id":
                        return new IdentifierLogicExpression(
                            (string)expression[0].Data);
                    case "StringLiteral":
                        return new ConstantLogicExpression(
                            this.ParseString((string)expression[0].Data));
                    case "NumberLiteral":
                        return new ConstantLogicExpression(
                            float.Parse((string)expression[0].Data, CultureInfo.InvariantCulture.NumberFormat));
                    case "BooleanLiteral":
                        return new ConstantLogicExpression(
                            (string)((Reduction)expression[0].Data)[0].Data == "true");
                }
            }

            if (expression.Count() == 3)
            {
                return this.ParseExpression((Reduction)expression[1].Data);
            }

            if (expression.Count() == 4)
            {
                return new FunctionCallLogicExpression(
                    (string)((Reduction)expression[0].Data)[0].Data,
                    this.ParseArguments((Reduction)expression[2].Data));
            }

            throw new InvalidOperationException();
        }

        private string ParseString(string data)
        {
            var result = data.Substring(1, data.Length - 2);
            result = result.Replace("\\'", "'");
            result = result.Replace("\\\"", "\"");
            return result;
        }

        private List<LogicExpression> ParseArguments(Reduction inputExpressions)
        {
            var expressions = new List<LogicExpression>();

            if (inputExpressions.Count() == 0)
            {
                return expressions;
            }

            while (true)
            {
                var statement = (Reduction)inputExpressions[0].Data;

                expressions.Add(this.ParseExpression(statement));

                if (inputExpressions.Count() == 1)
                {
                    return expressions;
                }

                inputExpressions = (Reduction)inputExpressions[2].Data;
            }
        }

        private LogicStatement ParseAssignStatement(Reduction statement)
        {
            return new AssignLogicStatement(
                this.ParseAssignTarget((Reduction)statement[0].Data),
                (string)statement[1].Data,
                this.ParseExpression((Reduction)statement[2].Data));
        }

        private LogicAssignmentTarget ParseAssignTarget(Reduction assignTarget)
        {
            if (assignTarget.Count() == 1)
            {
                return new IdentifierLogicAssignmentTarget((string)assignTarget[0].Data);
            }

            return new FieldLogicAssignmentTarget(
                this.ParseLookupExpression((Reduction)assignTarget[0].Data),
                (string)assignTarget[2].Data);
        }

        private LogicStatement ParseIfStatement(Reduction statement)
        {
            if (statement.Count() == 7)
            {
                return new IfElseLogicStatement(
                    this.ParseExpression((Reduction)statement[2].Data),
                    this.ParseStatement((Reduction)statement[4].Data),
                    this.ParseStatement((Reduction)statement[6].Data));
            }

            return new IfLogicStatement(
                this.ParseExpression((Reduction)statement[2].Data),
                this.ParseStatement((Reduction)statement[4].Data));
        }

        private LogicStatement ParseStatement(Reduction statement)
        {
            if (statement.Parent.Head().Name() == "TerminatedStatement")
            {
                statement = (Reduction)statement[0].Data;
            }

            if (statement.Parent.Head().Name() == "Statement")
            {
                statement = (Reduction)statement[0].Data;
            }

            switch (statement.Parent.Head().Name())
            {
                case "WhileStatement":
                    return this.ParseWhileStatement(statement);
                case "IfStatement":
                    return this.ParseIfStatement(statement);
                case "AssignStatement":
                    return this.ParseAssignStatement(statement);
                case "ReturnStatement":
                    return this.ParseReturnStatement(statement);
                case "BlockStatement":
                    return this.ParseBlockStatement(statement);
            }

            throw new InvalidOperationException();
        }

        private LogicStatement ParseWhileStatement(Reduction statement)
        {
            return new WhileLogicStatement(
                this.ParseExpression((Reduction)statement[2].Data),
                this.ParseStatement((Reduction)statement[4].Data));
        }
    }
}
