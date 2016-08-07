using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Protogame
{
    public class DefaultModelRenderConfiguration : IModelRenderConfiguration
    {
        public ModelVertexMapping GetVertexMappingToGPU(Model modelAsset, IEffect effectAsset)
        {
            switch (effectAsset.Name)
            {
                case "effect.BuiltinSurface:TextureNormalSkinned":
                case "effect.BuiltinSurface:TextureNormalSpecColDefSkinned":
                case "effect.BuiltinSurface:TextureNormalSpecColConSkinned":
                case "effect.BuiltinSurface:TextureNormalSpecColMapSkinned":
                    return ModelVertexMapping.Create(
                        src => new VertexPositionNormalBinormalTangentTextureBlendable(
                            src.Position ?? Vector3.Zero,
                            src.Normal ?? Vector3.Zero,
                            src.BiTangent ?? Vector3.Zero,
                            src.Tangent ?? Vector3.Zero,
                            (src.TexCoordsUV != null && src.TexCoordsUV.Length >= 1) ? src.TexCoordsUV[0] : Vector2.Zero,
                            src.BoneWeights ?? Vector4.Zero,
                            src.BoneIndices ?? new Byte4(0, 0, 0, 0)));
                case "effect.BuiltinSurface:TextureSkinned":
                    return ModelVertexMapping.Create(
                        src => new VertexPositionNormalTextureBlendable(
                            src.Position ?? Vector3.Zero,
                            src.Normal ?? Vector3.Zero,
                            (src.TexCoordsUV != null && src.TexCoordsUV.Length >= 1) ? src.TexCoordsUV[0] : Vector2.Zero,
                            src.BoneWeights ?? Vector4.Zero,
                            src.BoneIndices ?? new Byte4(0, 0, 0, 0)));
                case "effect.BuiltinSurface:ColorSkinned":
                    return ModelVertexMapping.Create(
                        src => new VertexPositionNormalColorBlendable(
                            src.Position ?? Vector3.Zero,
                            src.Normal ?? Vector3.Zero,
                            (src.Colors != null && src.Colors.Length >= 1) ? src.Colors[0]: new Color(),
                            src.BoneWeights ?? Vector4.Zero,
                            src.BoneIndices ?? new Byte4(0, 0, 0, 0)));
                case "effect.BuiltinSurface:DiffuseSkinned":
                    return ModelVertexMapping.Create(
                        src => new VertexPositionNormalBlendable(
                            src.Position ?? Vector3.Zero,
                            src.Normal ?? Vector3.Zero,
                            src.BoneWeights ?? Vector4.Zero,
                            src.BoneIndices ?? new Byte4(0, 0, 0, 0)));
                case "effect.BuiltinSurface:TextureNormal":
                case "effect.BuiltinSurface:TextureNormalSpecColDef":
                case "effect.BuiltinSurface:TextureNormalSpecColCon":
                case "effect.BuiltinSurface:TextureNormalSpecColMap":
                    return ModelVertexMapping.Create(
                        src => new VertexPositionNormalBinormalTangentTexture(
                            src.Position ?? Vector3.Zero,
                            src.Normal ?? Vector3.Zero,
                            src.BiTangent ?? Vector3.Zero,
                            src.Tangent ?? Vector3.Zero,
                            (src.TexCoordsUV != null && src.TexCoordsUV.Length >= 1) ? src.TexCoordsUV[0] : Vector2.Zero));
                case "effect.BuiltinSurface:Texture":
                    return ModelVertexMapping.Create(
                        src => new VertexPositionNormalTexture(
                            src.Position ?? Vector3.Zero,
                            src.Normal ?? Vector3.Zero,
                            (src.TexCoordsUV != null && src.TexCoordsUV.Length >= 1) ? src.TexCoordsUV[0] : Vector2.Zero));
                case "effect.BuiltinSurface:Color":
                    return ModelVertexMapping.Create(
                        src => new VertexPositionNormalColor(
                            src.Position ?? Vector3.Zero,
                            src.Normal ?? Vector3.Zero,
                            (src.Colors != null && src.Colors.Length >= 1) ? src.Colors[0] : new Color()));
                case "effect.BuiltinSurface:Diffuse":
                    return ModelVertexMapping.Create(
                        src => new VertexPositionNormal(
                            src.Position ?? Vector3.Zero,
                            src.Normal ?? Vector3.Zero));
                case "effect.PointLight":
                    return ModelVertexMapping.Create(
                        src => new VertexPosition(src.Position ?? Vector3.Zero));
                default:
                    return null;
            }
        }
    }
}
