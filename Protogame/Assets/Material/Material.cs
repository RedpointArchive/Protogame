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
        public float? PowerSpecular { get; set; }

        public Material() { }

        public Material(IMaterial materialToCloneFrom)
        {
            Name = materialToCloneFrom.Name;
            ColorDiffuse = materialToCloneFrom.ColorDiffuse;
            ColorAmbient = materialToCloneFrom.ColorAmbient;
            ColorSpecular = materialToCloneFrom.ColorSpecular;
            ColorEmissive = materialToCloneFrom.ColorEmissive;
            ColorTransparent = materialToCloneFrom.ColorTransparent;
            ColorReflective = materialToCloneFrom.ColorReflective;
            TextureDiffuse = materialToCloneFrom.TextureDiffuse;
            TextureSpecular = materialToCloneFrom.TextureSpecular;
            TextureAmbient = materialToCloneFrom.TextureAmbient;
            TextureEmissive = materialToCloneFrom.TextureEmissive;
            TextureHeight = materialToCloneFrom.TextureHeight;
            TextureNormal = materialToCloneFrom.TextureNormal;
            TextureOpacity = materialToCloneFrom.TextureOpacity;
            TextureDisplacement = materialToCloneFrom.TextureDisplacement;
            TextureLightMap = materialToCloneFrom.TextureLightMap;
            TextureReflection = materialToCloneFrom.TextureReflection;
            PowerSpecular = materialToCloneFrom.PowerSpecular;
        }
    }

    public interface IMaterialTexture
    {
        string HintPath { get; }

        IAssetReference<TextureAsset> TextureAsset { get; }
    }

    public class MaterialTexture : IMaterialTexture
    {
        public string HintPath { get; set; }

        public IAssetReference<TextureAsset> TextureAsset { get; set; }
    }
}
