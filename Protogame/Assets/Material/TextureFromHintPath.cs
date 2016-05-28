using System;
using System.Linq;

namespace Protogame
{
    public class TextureFromHintPath : ITextureFromHintPath
    {
        private readonly IAssetManager _assetManager;

        public TextureFromHintPath(IAssetManagerProvider assetManagerProvider)
        {
            _assetManager = assetManagerProvider.GetAssetManager();
        }

        public TextureAsset GetTextureFromHintPath(string hintPath)
        {
            var baseName = hintPath.Split('/').Last();
            var baseNameWithoutExtension = baseName.Substring(0, baseName.LastIndexOf(".", StringComparison.InvariantCulture));
            var fullName = hintPath.Replace('/', '.');
            var fullNameWithoutExtension = fullName.Substring(0, fullName.LastIndexOf(".", StringComparison.InvariantCulture));
            var hintPathsToAttempt = new[]
            {
                baseNameWithoutExtension,
                "texture." + baseNameWithoutExtension,
                baseName,
                "texture." + baseName,
                fullNameWithoutExtension,
                "texture." + fullNameWithoutExtension,
                fullName,
                "texture." + fullName,
            };

            foreach (var name in hintPathsToAttempt)
            {
                var texture = _assetManager.TryGet<TextureAsset>(name);
                if (texture != null)
                {
                    return texture;
                }
            }

            return null;
        }

        public TextureAsset GetTextureFromHintPath(IMaterialTexture materialTexture)
        {
            return GetTextureFromHintPath(materialTexture.HintPath);
        }
    }
}