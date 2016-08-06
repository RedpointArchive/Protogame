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
                case "effect.BuiltinSurface:TextureNormalSpecIntMapColDefSkinned":
                case "effect.BuiltinSurface:TextureNormalSpecIntConColDefSkinned":
                case "effect.BuiltinSurface:TextureNormalSpecIntMapColConSkinned":
                case "effect.BuiltinSurface:TextureNormalSpecIntConColConSkinned":
                case "effect.BuiltinSurface:TextureNormalSpecIntMapColMapSkinned":
                case "effect.BuiltinSurface:TextureNormalSpecIntConColMapSkinned":
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
                case "effect.BuiltinSurface:TextureNormalSpecIntMapColDef":
                case "effect.BuiltinSurface:TextureNormalSpecIntConColDef":
                case "effect.BuiltinSurface:TextureNormalSpecIntMapColCon":
                case "effect.BuiltinSurface:TextureNormalSpecIntConColCon":
                case "effect.BuiltinSurface:TextureNormalSpecIntMapColMap":
                case "effect.BuiltinSurface:TextureNormalSpecIntConColMap":
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
