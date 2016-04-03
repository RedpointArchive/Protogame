using System.Collections.Generic;

namespace Protogame
{
    public class UnifiedShaderInfo
    {
        public List<ConstantBufferInfo> ConstantBuffers { get; set; }

        public List<ParameterInfo> Parameters { get; set; }

        public Dictionary<string, ShaderBlockInfo> ShaderBlocks { get; set; }

        public List<TechniqueInfo> Techniques { get; set; }
    }
}