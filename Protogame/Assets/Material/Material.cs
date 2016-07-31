using Microsoft.Xna.Framework;

namespace Protogame
{
    public class Material : IMaterial
    {
        public string Name { get; set; }
        public Color? ColorDiffuse { get; set; }
        public Color? ColorAmbient { get; set; }
        public Color? ColorSpecular { get; set; }
        public Color? ColorEmissive { get; set; }
        public Color? ColorTransparent { get; set; }
        public Color? ColorReflective { get; set; }
        public IMaterialTexture TextureDiffuse { get; set; }
        public IMaterialTexture TextureSpecular { get; set; }
        public IMaterialTexture TextureAmbient { get; set; }
        public IMaterialTexture TextureEmissive { get; set; }
        public IMaterialTexture TextureHeight { get; set; }
        public IMaterialTexture TextureNormal { get; set; }
        public IMaterialTexture TextureOpacity { get; set; }
        public IMaterialTexture TextureDisplacement { get; set; }
        public IMaterialTexture TextureLightMap { get; set; }
        public IMaterialTexture TextureReflection { get; set; }
    }

    public interface IMaterialTexture
    {
        string HintPath { get; }

        TextureAsset TextureAsset { get; }
    }

    public class MaterialTexture : IMaterialTexture
    {
        public string HintPath { get; set; }

        public TextureAsset TextureAsset { get; set; }
    }
}
