using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IMaterial
    {
        string Name { get; }

        Color? ColorDiffuse { get; }

        Color? ColorAmbient { get; }

        Color? ColorSpecular { get; }

        Color? ColorEmissive { get; }

        Color? ColorTransparent { get; }

        Color? ColorReflective { get; }

        IMaterialTexture TextureDiffuse { get; }

        IMaterialTexture TextureSpecular { get; }

        IMaterialTexture TextureAmbient { get; }

        IMaterialTexture TextureEmissive { get; }

        IMaterialTexture TextureHeight { get; }

        IMaterialTexture TextureNormal { get; }

        IMaterialTexture TextureOpacity { get; }

        IMaterialTexture TextureDisplacement { get; }

        IMaterialTexture TextureLightMap { get; }

        IMaterialTexture TextureReflection { get; }

        float? PowerSpecular { get; }
    }
}
