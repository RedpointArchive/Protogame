using System;
using System.Linq;

namespace Protogame
{
    public class UnifiedShaderParsingUnexpectedTokenException : Exception
    {
        public UnifiedShaderParsingUnexpectedTokenException(UnifiedShaderParserV1.UnifiedShaderToken token, params string[] allowedStringTokens) : base(
            "Unexpected token " + token + ", expected string token with value in " + allowedStringTokens.Select(x => "'" + x + "'").Aggregate((a, b) => a + ", " + b))
        {
        }

        public UnifiedShaderParsingUnexpectedTokenException(UnifiedShaderParserV1.UnifiedShaderToken token, params UnifiedShaderParserV1.UnifiedShaderTokenType[] allowedTokens) : base(
            "Unexpected token " + token + ", expected token with type in " + allowedTokens.Select(x => x.ToString()).Aggregate((a, b) => a + ", " + b))
        {
        }
    }
}