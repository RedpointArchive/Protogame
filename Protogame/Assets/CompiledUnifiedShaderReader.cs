using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    internal class CompiledUnifiedShaderReader : IEffectReader
    {
        public CompiledUnifiedShaderReader(byte[] data)
        {
            
        }

        public int GetEffectKey()
        {
            throw new System.NotImplementedException();
        }

        public int GetConstantBufferCount()
        {
            throw new System.NotImplementedException();
        }

        public string GetConstantBufferName(int constantBufferIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetConstantBufferSize(int constantBufferIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetConstantBufferParameterCount(int constantBufferIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetConstantBufferParameterValue(int constantBufferIndex, int parameterIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetConstantBufferParameterOffset(int constantBufferIndex, int parameterIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetShaderCount()
        {
            throw new System.NotImplementedException();
        }

        public IShaderReader GetShaderReader(int shaderIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetParameterCount(object parameterContext)
        {
            throw new System.NotImplementedException();
        }

        public EffectParameterClass GetParameterClass(object parameterContext, int parameterIndex)
        {
            throw new System.NotImplementedException();
        }

        public EffectParameterType GetParameterType(object parameterContext, int parameterIndex)
        {
            throw new System.NotImplementedException();
        }

        public string GetParameterName(object parameterContext, int parameterIndex)
        {
            throw new System.NotImplementedException();
        }

        public string GetParameterSemantic(object parameterContext, int parameterIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetParameterAnnotationCount(object parameterContext, int parameterIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetParameterRowCount(object parameterContext, int parameterIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetParameterColumnCount(object parameterContext, int parameterIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetParameterInt32Buffer(object parameterContext, int parameterIndex, int bufferIndex)
        {
            throw new System.NotImplementedException();
        }

        public float GetParameterFloatBuffer(object parameterContext, int parameterIndex, int bufferIndex)
        {
            throw new System.NotImplementedException();
        }

        public object GetParameterElementsContext(object parameterContext, int parameterIndex)
        {
            throw new System.NotImplementedException();
        }

        public object GetParameterStructMembersContext(object parameterContext, int parameterIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetTechniqueCount()
        {
            throw new System.NotImplementedException();
        }

        public string GetTechniqueName(int techniqueIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetTechniqueAnnotationCount(int techniqueIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetPassCount(int techniqueIndex)
        {
            throw new System.NotImplementedException();
        }

        public string GetPassName(int techniqueIndex, int passIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetPassAnnotationCount(int techniqueIndex, int passIndex)
        {
            throw new System.NotImplementedException();
        }

        public int? GetPassVertexShaderIndex(int techniqueIndex, int passIndex)
        {
            throw new System.NotImplementedException();
        }

        public int? GetPassPixelShaderIndex(int techniqueIndex, int passIndex)
        {
            throw new System.NotImplementedException();
        }

        public BlendState GetPassBlendState(int techniqueIndex, int passIndex)
        {
            throw new System.NotImplementedException();
        }

        public DepthStencilState GetPassDepthStencilState(int techniqueIndex, int passIndex)
        {
            throw new System.NotImplementedException();
        }

        public RasterizerState GetPassRasterizerState(int techniqueIndex, int passIndex)
        {
            throw new System.NotImplementedException();
        }
    }
}