using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Prototest.Library.Version1;

namespace Protogame.Tests
{
    public class UnifiedShaderTests
    {
        private readonly IAssert _assert;

        public UnifiedShaderTests(IAssert assert)
        {
            _assert = assert;
        }

        public void ParseExampleCode()
        {
            /*
            var parser = new UnifiedShaderParser();
            var codeStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("Protogame.Tests.UnifiedShaderExample.usl");
            _assert.NotNull(codeStream);
            var codeReader = new StreamReader(codeStream);
            var effect = parser.Parse(codeReader.ReadToEnd());
            */
        }
    }
}
