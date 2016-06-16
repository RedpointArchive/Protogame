using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Protogame
{
    public class DefaultModelRenderConfiguration : IModelRenderConfiguration
    {
        public ModelVertexMapping GetVertexMappingToGPU(Model modelAsset, Effect effectAsset)
        {
            switch (effectAsset.Name)
            {
                case "effect.TextureSkinned":
                    return ModelVertexMapping.Create(
                        src => new VertexPositionNormalTextureBlendable(
                            src.Position ?? Vector3.Zero,
                            src.Normal ?? Vector3.Zero,
                            (src.TexCoordsUV != null && src.TexCoordsUV.Length >= 1) ? src.TexCoordsUV[0] : Vector2.Zero,
                            src.BoneWeights ?? Vector4.Zero,
                            src.BoneIndices ?? new Byte4(0, 0, 0, 0)));
                case "effect.ColorSkinned":
                    return ModelVertexMapping.Create(
                        src => new VertexPositionNormalColorBlendable(
                            src.Position ?? Vector3.Zero,
                            src.Normal ?? Vector3.Zero,
                            (src.Colors != null && src.Colors.Length >= 1) ? src.Colors[0]: new Color(),
                            src.BoneWeights ?? Vector4.Zero,
                            src.BoneIndices ?? new Byte4(0, 0, 0, 0)));
                case "effect.Texture":
                    return ModelVertexMapping.Create(
                        src => new VertexPositionNormalTexture(
                            src.Position ?? Vector3.Zero,
                            src.Normal ?? Vector3.Zero,
                            (src.TexCoordsUV != null && src.TexCoordsUV.Length >= 1) ? src.TexCoordsUV[0] : Vector2.Zero));
                case "effect.Color":
                    return ModelVertexMapping.Create(
                        src => new VertexPositionNormalColor(
                            src.Position ?? Vector3.Zero,
                            src.Normal ?? Vector3.Zero,
                            (src.Colors != null && src.Colors.Length >= 1) ? src.Colors[0] : new Color()));
                case "effect.PointLight":
                    return ModelVertexMapping.Create(
                        src => new VertexPosition(src.Position ?? Vector3.Zero));
                default:
                    return null;
            }
        }
    }
}
