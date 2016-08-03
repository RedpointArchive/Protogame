using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class DefaultRenderRequest : IRenderRequest
    {
        public DefaultRenderRequest(
            RasterizerState rasterizerState,
            BlendState blendState,
            DepthStencilState depthStencilState,
            Effect effect,
            string techniqueName,
            EffectParameter[] effectParameters,
            VertexBuffer meshVertexBuffer,
            IndexBuffer meshIndexBuffer,
            PrimitiveType primitiveType,
            Matrix[] instances)
        {
            RasterizerState = rasterizerState;
            BlendState = blendState;
            DepthStencilState = depthStencilState;
            Effect = effect;
            TechniqueName = techniqueName;
            EffectParameters = effectParameters;
            MeshVertexBuffer = meshVertexBuffer;
            MeshIndexBuffer = meshIndexBuffer;
            PrimitiveType = primitiveType;
            Instances = instances;

            var effectParameterState = EffectParameters.Length.GetHashCode() ^ 397;
            for (var i = 0; i < EffectParameters.Length; i++)
            {
                if (EffectParameters[i].Name == "World")
                {
                    // The world is an instanced parameter.
                    continue;
                }

                effectParameterState += HashEffectParameter(EffectParameters[i]);
            }

            StateHash =
                RasterizerState.GetHashCode() ^ 397 +
                BlendState.GetHashCode() ^ 397 +
                DepthStencilState.GetHashCode() ^ 397 +
                Effect.GetHashCode() ^ 397 +
                TechniqueName.GetHashCode() ^ 397 +
                effectParameterState +
                MeshVertexBuffer.GetHashCode() ^ 397 +
                MeshIndexBuffer.GetHashCode() ^ 397 +
                PrimitiveType.GetHashCode() ^ 397;
        }

        private int HashEffectParameter(EffectParameter effectParameter)
        {
            var initialHash =
                effectParameter.ColumnCount.GetHashCode() ^ 397 +
                effectParameter.RowCount.GetHashCode() ^ 397 +
                effectParameter.ParameterClass.GetHashCode() ^ 397 +
                effectParameter.ParameterType.GetHashCode() ^ 397;
            
            switch (effectParameter.ParameterType)
            {
                case EffectParameterType.Bool:
                    initialHash += effectParameter.GetValueBoolean().GetHashCode() ^ 397;
                    break;
                case EffectParameterType.Int32:
                    initialHash += effectParameter.GetValueInt32().GetHashCode() ^ 397;
                    break;
                case EffectParameterType.Single:
                    if (effectParameter.ParameterClass == EffectParameterClass.Scalar)
                    {
                        initialHash += effectParameter.GetValueSingle().GetHashCode() ^ 397;
                    }
                    else if (effectParameter.ParameterClass == EffectParameterClass.Matrix)
                    {
                        if (effectParameter.Elements.Count > 0)
                        {
                            var elems = effectParameter.GetValueMatrixArray(effectParameter.Elements.Count);
                            initialHash += elems.Length.GetHashCode() ^ 397;
                            for (var e = 0; e < elems.Length; e++)
                            {
                                initialHash += elems[e].GetHashCode() ^ 397;
                            }
                        }
                        else
                        {
                            initialHash += effectParameter.GetValueMatrix().GetHashCode() ^ 397;
                        }
                    }
                    else if (effectParameter.ParameterClass == EffectParameterClass.Vector)
                    {
                        if (effectParameter.ColumnCount == 4)
                        {
                            if (effectParameter.Elements.Count > 0)
                            {
                                var elems = effectParameter.GetValueVector4Array();
                                initialHash += elems.Length.GetHashCode() ^ 397;
                                for (var e = 0; e < elems.Length; e++)
                                {
                                    initialHash += elems[e].GetHashCode() ^ 397;
                                }
                            }
                            else
                            {
                                initialHash += effectParameter.GetValueVector4().GetHashCode() ^ 397;
                            }
                        }
                        else if (effectParameter.ColumnCount == 3)
                        {
                            if (effectParameter.Elements.Count > 0)
                            {
                                var elems = effectParameter.GetValueVector3Array();
                                initialHash += elems.Length.GetHashCode() ^ 397;
                                for (var e = 0; e < elems.Length; e++)
                                {
                                    initialHash += elems[e].GetHashCode() ^ 397;
                                }
                            }
                            else
                            {
                                initialHash += effectParameter.GetValueVector3().GetHashCode() ^ 397;
                            }
                        }
                        else if (effectParameter.ColumnCount == 2)
                        {
                            if (effectParameter.Elements.Count > 0)
                            {
                                var elems = effectParameter.GetValueVector2Array();
                                initialHash += elems.Length.GetHashCode() ^ 397;
                                for (var e = 0; e < elems.Length; e++)
                                {
                                    initialHash += elems[e].GetHashCode() ^ 397;
                                }
                            }
                            else
                            {
                                initialHash += effectParameter.GetValueVector2().GetHashCode() ^ 397;
                            }
                        }
                    }
                    else
                    {
                        var arr = effectParameter.GetValueSingleArray();
                        initialHash += arr.Length.GetHashCode() ^ 397;
                        for (var e = 0; e < arr.Length; e++)
                        {
                            initialHash += arr[e].GetHashCode() ^ 397;
                        }
                    }
                    break;
                case EffectParameterType.Texture2D:
                    initialHash += effectParameter.GetValueTexture2D().GetHashCode() ^ 397;
                    break;
                case EffectParameterType.Texture3D:
                    initialHash += effectParameter.GetValueTexture3D().GetHashCode() ^ 397;
                    break;
                case EffectParameterType.Void:
                    break;
                default:
                    break;
            }

            return initialHash;
        }

        public int StateHash { get; }

        public RasterizerState RasterizerState { get; }

        public BlendState BlendState { get; }

        public DepthStencilState DepthStencilState { get; }

        public Effect Effect { get; }

        public string TechniqueName { get; }

        public EffectParameter[] EffectParameters { get; }

        public VertexBuffer MeshVertexBuffer { get; set; }

        public IndexBuffer MeshIndexBuffer { get; set; }

        public PrimitiveType PrimitiveType { get; set; }

        public Matrix[] Instances { get; }
    }
}