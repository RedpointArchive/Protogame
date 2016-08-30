using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class TextureFromHintPath : ITextureFromHintPath
    {
        private readonly IAssetManager _assetManager;

        private Dictionary<string, IAssetReference<TextureAsset>> _hintPathCache;

        public TextureFromHintPath(IAssetManager assetManager)
        {
            _assetManager = assetManager;
            _hintPathCache = new Dictionary<string, IAssetReference<TextureAsset>>();
        }

        public IAssetReference<TextureAsset> GetTextureFromHintPath(string hintPath)
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

            _hintPathCache[hintPath] = _assetManager.GetPreferred<TextureAsset>(hintPathsToAttempt);
            return _hintPathCache[hintPath];
        }

        public IAssetReference<TextureAsset> GetTextureFromHintPath(IMaterialTexture materialTexture)
        {
            return GetTextureFromHintPath(materialTexture.HintPath);
        }
    }
}