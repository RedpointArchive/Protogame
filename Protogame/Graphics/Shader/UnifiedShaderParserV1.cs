using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Protogame
{
    public class UnifiedShaderParserV1
    {
        public UnifiedShaderInfo Parse(string input)
        {
            var tokens = Lex(input);
            var info = new UnifiedShaderInfo
            {
                ConstantBuffers = new List<ConstantBufferInfo>(),
                Parameters = new List<ParameterInfo>(),
                ShaderBlocks = new Dictionary<string, ShaderBlockInfo>(),
                Techniques = new List<TechniqueInfo>()
            };

            ParseTopLevel(info, tokens);

            return info;
        }

        private void ParseTopLevel(UnifiedShaderInfo info, List<UnifiedShaderToken> tokens)
        {
            while (tokens.Count > 0)
            {
                var token = ConsumeToken(tokens);
                switch (token.TokenType)
                {
                    case UnifiedShaderTokenType.VersionDeclaration:
                        break;
                    case UnifiedShaderTokenType.StringToken:
                        switch (token.Text)
                        {
                            case "cbuffers":
                                ParseConstantBuffers(info, tokens);
                                break;
                            case "parameters":
                                ParseParameters(info, tokens);
                                break;
                            case "shaders":
                                ParseShaders(info, tokens);
                                break;
                            case "technique":
                                ParseTechnique(info, tokens);
                                break;
                            default:
                                throw new UnifiedShaderParsingUnexpectedTokenException(token, "cbuffers", "parameters",
                                    "shaders", "technique");
                        }
                        break;
                    default:
                        throw new UnifiedShaderParsingUnexpectedTokenException(token,
                            UnifiedShaderTokenType.VersionDeclaration, UnifiedShaderTokenType.StringToken);
                }
            }
        }

        private void ParseTechnique(UnifiedShaderInfo info, List<UnifiedShaderToken> tokens)
        {
            var name = ConsumeToken(tokens);
            ExpectToken(name, UnifiedShaderTokenType.StringToken);
            ExpectToken(ConsumeToken(tokens), UnifiedShaderTokenType.OpenBrace);

            var technique = new TechniqueInfo
            {
                Name = name.Text,
                Passes = new List<PassInfo>()
            };

            while (tokens[0].TokenType == UnifiedShaderTokenType.StringToken &&
                   tokens[0].Text == "pass")
            {
                var pass = new PassInfo();
                ExpectToken(ConsumeToken(tokens), "pass");
                var passName = ConsumeToken(tokens);
                ExpectToken(passName, UnifiedShaderTokenType.StringToken);
                pass.Name = passName.Text;
                ParsePass(pass, tokens);
                technique.Passes.Add(pass);
            }

            info.Techniques.Add(technique);

            ExpectToken(ConsumeToken(tokens), UnifiedShaderTokenType.CloseBrace);
        }

        private void ParsePass(PassInfo pass, List<UnifiedShaderToken> tokens)
        {
            ExpectToken(ConsumeToken(tokens), UnifiedShaderTokenType.OpenBrace);

            while (tokens[0].TokenType == UnifiedShaderTokenType.StringToken)
            {
                var type = ConsumeToken(tokens);
                ExpectToken(type, UnifiedShaderTokenType.StringToken);

                switch (type.Text)
                {
                    case "pixel":
                    case "vertex":
                    case "geometry":
                    case "domain":
                    case "hull":
                        var name = ConsumeToken(tokens);
                        ExpectToken(name, UnifiedShaderTokenType.StringToken);
                        ExpectToken(ConsumeToken(tokens), UnifiedShaderTokenType.Semicolon);
                        switch (type.Text)
                        {
                            case "pixel":
                                pass.PixelShaderName = name.Text;
                                break;
                            case "vertex":
                                pass.VertexShaderName = name.Text;
                                break;
                            case "geometry":
                                pass.GeometryShaderName = name.Text;
                                break;
                            case "domain":
                                pass.DomainShaderName = name.Text;
                                break;
                            case "hull":
                                pass.HullShaderName = name.Text;
                                break;
                        }
                        break;
                    case "compute":
                        // TODO: This implementation is not complete.
                        break;
                    case "blend":
                    case "depth":
                    case "raster":
                        // TODO: This implementation is not complete.
                        break;
                }
            }

            ExpectToken(ConsumeToken(tokens), UnifiedShaderTokenType.CloseBrace);
        }

        private void ParseShaders(UnifiedShaderInfo info, List<UnifiedShaderToken> tokens)
        {
            ExpectToken(ConsumeToken(tokens), UnifiedShaderTokenType.OpenBrace);

            while (tokens[0].TokenType == UnifiedShaderTokenType.LanguageStart)
            {
                var languageStart = ConsumeToken(tokens);
                var languageBlock = ConsumeToken(tokens);
                var languageEnd = ConsumeToken(tokens);
                ExpectToken(languageStart, UnifiedShaderTokenType.LanguageStart);
                ExpectToken(languageBlock, UnifiedShaderTokenType.LanguageBlock);
                ExpectToken(languageEnd, UnifiedShaderTokenType.LanguageEnd);

                if (!info.ShaderBlocks.ContainsKey(languageStart.Text))
                {
                    info.ShaderBlocks[languageStart.Text] = new ShaderBlockInfo
                    {
                        ShaderText = String.Empty
                    };
                }

                info.ShaderBlocks[languageStart.Text].ShaderText += languageBlock.Text;
            }

            ExpectToken(ConsumeToken(tokens), UnifiedShaderTokenType.CloseBrace);
        }

        private void ParseParameters(UnifiedShaderInfo info, List<UnifiedShaderToken> tokens)
        {
            ExpectToken(ConsumeToken(tokens), UnifiedShaderTokenType.OpenBrace);

            ParseParameterList(info.Parameters, tokens);

            ExpectToken(ConsumeToken(tokens), UnifiedShaderTokenType.CloseBrace);
        }

        private void ParseConstantBuffers(UnifiedShaderInfo info, List<UnifiedShaderToken> tokens)
        {
            ExpectToken(ConsumeToken(tokens), UnifiedShaderTokenType.OpenBrace);

            while (tokens[0].TokenType == UnifiedShaderTokenType.StringToken && tokens[0].Text == "cbuffer")
            {
                var buffer = new ConstantBufferInfo();
                ParseConstantBuffer(buffer, tokens);
                info.ConstantBuffers.Add(buffer);
            }

            ExpectToken(ConsumeToken(tokens), UnifiedShaderTokenType.CloseBrace);
        }

        private void ParseConstantBuffer(ConstantBufferInfo buffer, List<UnifiedShaderToken> tokens)
        {
            ExpectToken(ConsumeToken(tokens), "cbuffer");
            var name = ConsumeToken(tokens);
            ExpectToken(name, UnifiedShaderTokenType.StringToken);
            
            buffer.Name = name.Text;
            buffer.Variables = new List<ParameterInfo>();
            
            ExpectToken(ConsumeToken(tokens), UnifiedShaderTokenType.OpenBrace);

            ParseParameterList(buffer.Variables, tokens);

            ExpectToken(ConsumeToken(tokens), UnifiedShaderTokenType.CloseBrace);
        }

        private void ParseParameterList(List<ParameterInfo> variables, List<UnifiedShaderToken> tokens)
        {
            while (tokens[0].TokenType == UnifiedShaderTokenType.StringToken)
            {
                var type = ConsumeToken(tokens);
                var name = ConsumeToken(tokens);
                ExpectToken(type, UnifiedShaderTokenType.StringToken);
                ExpectToken(name, UnifiedShaderTokenType.StringToken);
                ExpectToken(ConsumeToken(tokens), UnifiedShaderTokenType.Semicolon);

                variables.Add(new ParameterInfo
                {
                    Name = name.Text,
                    UnresolvedTypeInfo = type.Text
                });
            }
        }

        private void ExpectToken(UnifiedShaderToken consumeToken, string text)
        {
            if (consumeToken.TokenType != UnifiedShaderTokenType.StringToken ||
                consumeToken.Text != text)
            {
                throw new UnifiedShaderParsingUnexpectedTokenException(consumeToken, text);
            }
        }

        private void ExpectToken(UnifiedShaderToken consumeToken, UnifiedShaderTokenType type)
        {
            if (consumeToken.TokenType != type)
            {
                throw new UnifiedShaderParsingUnexpectedTokenException(consumeToken, type);
            }
        }

        private UnifiedShaderToken ConsumeToken(List<UnifiedShaderToken> tokens)
        {
            var f = tokens[0];
            tokens.RemoveAt(0);
            return f;
        }

        public List<UnifiedShaderToken> Lex(string input)
        {
            var tokens = new List<UnifiedShaderToken>();
            var lineNumber = 1;
            var columnNumber = 1;
            var textMatch = new Regex("^[a-zA-Z0-9\\.]+$");
            var languageMatch = new Regex("^#language ([a-zA-Z]+)$");
            var endLanguageMatch = new Regex("^#endlanguage$");
            var versionMatch = new Regex("^#version (1)$");
            var buffer = string.Empty;
            var lastMatch = Match.Empty;
            UnifiedShaderTokenType? type = null;
            var inLanguage = false;
            var languageBuffer = string.Empty;
            var bufferLineNumber = 1;
            var bufferColumnNumber = 1;
            var position = 0;

            while (position < input.Length)
            {
                var c = input[position++];
                columnNumber++;
                if (c == '\r' || c == '\n')
                {
                    if (inLanguage)
                    {
                        languageBuffer += buffer;
                        languageBuffer += c;
                        buffer = string.Empty;
                    }

                    char? p = null;
                    if (position < input.Length)
                    {
                        p = input[position++];
                        columnNumber++;
                    }

                    if (c == '\r' && p != '\n')
                    {
                        // it's a standalone \r, so treat it as a newline
                        // but don't consume the next character
                        position--;
                        columnNumber--;

                        if (inLanguage)
                        {
                            languageBuffer += p;
                        }
                    }

                    lineNumber++;
                    columnNumber = 1;
                    continue;
                }
                else if (buffer == string.Empty)
                {
                    if (!inLanguage && (c == ' ' || c == '\t'))
                    {
                        continue;
                    }
                    else if (!inLanguage && c == '{')
                    {
                        tokens.Add(new UnifiedShaderToken(lineNumber, columnNumber - 1, UnifiedShaderTokenType.OpenBrace));
                        continue;
                    }
                    else if (!inLanguage && c == '}')
                    {
                        tokens.Add(new UnifiedShaderToken(lineNumber, columnNumber - 1, UnifiedShaderTokenType.CloseBrace));
                        continue;
                    }
                    else if (!inLanguage && c == ';')
                    {
                        tokens.Add(new UnifiedShaderToken(lineNumber, columnNumber - 1, UnifiedShaderTokenType.Semicolon));
                        continue;
                    }
                }

                if (buffer == string.Empty)
                {
                    bufferLineNumber = lineNumber;
                    bufferColumnNumber = columnNumber - 1;
                }
                buffer += c;
                var match = Match.Empty;
                if (!inLanguage && (match = textMatch.Match(buffer)) != Match.Empty)
                {
                    lastMatch = match;
                    type = UnifiedShaderTokenType.StringToken;
                }
                else if (!inLanguage && (match = languageMatch.Match(buffer)) != Match.Empty)
                {
                    lastMatch = match;
                    type = UnifiedShaderTokenType.LanguageStart;
                }
                else if (!inLanguage && (match = versionMatch.Match(buffer)) != Match.Empty)
                {
                    lastMatch = match;
                    type = UnifiedShaderTokenType.VersionDeclaration;
                }
                else if (inLanguage && (match = endLanguageMatch.Match(buffer)) != Match.Empty)
                {
                    lastMatch = match;
                    type = UnifiedShaderTokenType.LanguageEnd;
                }
                else
                {
                    // we can't consume any more characters
                    position--;
                    columnNumber--;
                    var oldBuffer = buffer;
                    buffer = string.Empty;

                    if (lastMatch == Match.Empty)
                    {
                        // no match yet,
                        buffer = oldBuffer;
                        position++;
                        columnNumber++;
                        continue;
                    }
                    else if (type == UnifiedShaderTokenType.StringToken)
                    {
                        tokens.Add(new UnifiedShaderToken(bufferLineNumber, bufferColumnNumber, type.Value, lastMatch.Value));
                    }
                    else if (type == UnifiedShaderTokenType.LanguageStart)
                    {
                        tokens.Add(new UnifiedShaderToken(bufferLineNumber, bufferColumnNumber, type.Value, lastMatch.Groups[1].Value));
                        inLanguage = true;
                    }
                    else if (type == UnifiedShaderTokenType.VersionDeclaration)
                    {
                        tokens.Add(new UnifiedShaderToken(bufferLineNumber, bufferColumnNumber, type.Value, lastMatch.Groups[0].Value));
                    }
                    else if (type == UnifiedShaderTokenType.LanguageEnd)
                    {
                        var languageBufferProcessed = languageBuffer.TrimEnd();
                        if (languageBufferProcessed.EndsWith("#endlanguage"))
                        {
                            languageBufferProcessed = languageBufferProcessed.Substring(0,
                                languageBufferProcessed.Length - "#endlanguage".Length);
                        }
                        tokens.Add(new UnifiedShaderToken(bufferLineNumber, bufferColumnNumber, UnifiedShaderTokenType.LanguageBlock, languageBufferProcessed));
                        tokens.Add(new UnifiedShaderToken(bufferLineNumber, bufferColumnNumber, type.Value, lastMatch.Groups[0].Value));
                        inLanguage = false;
                        languageBuffer = string.Empty;
                    }
                    else
                    {
                        throw new UnifiedShaderLexingException("Unknown state", lineNumber, columnNumber);
                    }

                    lastMatch = Match.Empty;
                }
            }

            return tokens;
        }

        public class UnifiedShaderToken
        {
            public UnifiedShaderToken(int line, int column, UnifiedShaderTokenType unifiedShaderTokenType)
            {
                Line = line;
                Column = column;
                TokenType = unifiedShaderTokenType;
            }

            public UnifiedShaderToken(int line, int column, UnifiedShaderTokenType unifiedShaderTokenType, string value)
            {
                Line = line;
                Column = column;
                TokenType = unifiedShaderTokenType;
                Text = value;
            }

            public int Line { get; set; }
            public int Column { get; set; }
            public UnifiedShaderTokenType TokenType { get; set; }

            public string Text { get; set; }

            public override string ToString()
            {
                return "L" + Line + " C" + Column + " " + TokenType + (Text != null ? ": " + Text : string.Empty);
            }
        }

        public enum UnifiedShaderTokenType
        {
            OpenBrace,
            CloseBrace,
            Semicolon,
            StringToken,
            LanguageStart,
            LanguageBlock,
            LanguageEnd,
            VersionDeclaration
        }
    }
}