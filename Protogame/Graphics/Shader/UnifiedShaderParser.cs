namespace Protogame
{
    public class UnifiedShaderParser
    {
        public UnifiedShaderParser()
        {
        }

        public UnifiedShaderInfo Parse(string input)
        {
            if (input.Replace("\r\n", "\n").StartsWith("#version 1\n"))
            {
                var parser = new UnifiedShaderParserV1();
                return parser.Parse(input);
            }

            throw new UnifiedShaderVersionException();
        }
    }
}
