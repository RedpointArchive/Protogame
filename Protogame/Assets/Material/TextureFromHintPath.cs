using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class TextureFromHintPath : ITextureFromHintPath
    {
        private readonly IAssetManager _assetManager;

        private Dictionary<string, TextureAsset> _hintPathCache;

        public TextureFromHintPath(IAssetManagerProvider assetManagerProvider)
        {
            _assetManager = assetManagerProvider.GetAssetManager();
            _hintPathCache = new Dictionary<string, TextureAsset>();
        }

        public TextureAsset GetTextureFromHintPath(string hintPath)
        {
            if (_hintPathCache.ContainsKey(hintPath))
            {
                return _hintPathCache[hintPath];
            }

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
                    _hintPathCache[hintPath] = texture;
                    return texture;
                }
            }

            _hintPathCache[hintPath] = null;
            return null;
        }

        public TextureAsset GetTextureFromHintPath(IMaterialTexture materialTexture)
        {
            return GetTextureFromHintPath(materialTexture.HintPath);
        }
    }
}