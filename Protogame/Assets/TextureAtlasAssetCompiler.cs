#if PLATFORM_WINDOWS || PLATFORM_LINUX

namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using Microsoft.Xna.Framework;

    public class TextureAtlasAssetCompiler : IAssetCompiler<TextureAtlasAsset>
    {
        private readonly IAssetContentManager m_AssetContentManager;

        private readonly TextureAssetCompiler m_TextureAssetCompiler;

        public TextureAtlasAssetCompiler(
            IAssetContentManager assetContentManager,
            TextureAssetCompiler textureAssetCompiler)
        {
            this.m_AssetContentManager = assetContentManager;
            this.m_TextureAssetCompiler = textureAssetCompiler;
        }

        public void Compile(TextureAtlasAsset asset, TargetPlatform platform)
        {
            var allTextures = asset.GetAssetsLambda().OfType<TextureAsset>();

            var textures = asset.SourceTextureNames.Length == 0 
                           ? allTextures 
                           : allTextures.Where(x => asset.SourceTextureNames.Contains(x.Name));

            var textureFallbacks = new Dictionary<TextureAsset, Bitmap>();
            var texturePacker = new TexturePacker<TextureAsset>();

            foreach (var texture in textures)
            {
                if (texture.PlatformData == null)
                {
                    this.m_TextureAssetCompiler.Compile(texture, platform);

                    texture.ReloadTexture();
                }

                // We can end up with textures that have no Texture set
                // here if we are running inside the compilation tool (where
                // there's no graphics device to load textures).  In that
                // case we fall back to loading the texture's raw data with
                // System.Drawing.Bitmap.
                float width, height;
                if (texture.Texture == null)
                {
                    if (texture.CompiledOnly)
                    {
                        throw new InvalidOperationException("No graphics device service and no source data for texture.");
                    }

                    var fallback = new Bitmap(new MemoryStream(texture.RawData));
                    width = fallback.Width;
                    height = fallback.Height;
                    textureFallbacks.Add(texture, fallback);
                }
                else
                {
                    width = texture.Texture.Width;
                    height = texture.Texture.Height;
                }

                texturePacker.AddTexture(
                    new Vector2(width, height),
                    texture);
            }

            Vector2 size;
            var packedTextures = texturePacker.Pack(out size);
            var bitmap = new Bitmap((int)size.X, (int)size.Y);

            foreach (var packedTexture in packedTextures)
            {
                if (packedTexture.Texture.Texture == null)
                {
                    if (!textureFallbacks.ContainsKey(packedTexture.Texture))
                    {
                        throw new InvalidOperationException();
                    }

                    var fallback = textureFallbacks[packedTexture.Texture];
                    for (var x = 0; x < fallback.Width; x++)
                    {
                        for (var y = 0; y < fallback.Height; y++)
                        {
                            bitmap.SetPixel(
                                (int)packedTexture.Position.X + x, 
                                (int)packedTexture.Position.Y + y, 
                                fallback.GetPixel(
                                    x,
                                    y));
                        }
                    }
                }
                else
                {
                    var data = new Microsoft.Xna.Framework.Color[packedTexture.Texture.Texture.Width * packedTexture.Texture.Texture.Height];
                    packedTexture.Texture.Texture.GetData(data);

                    for (var x = 0; x < packedTexture.Texture.Texture.Width; x++)
                    {
                        for (var y = 0; y < packedTexture.Texture.Texture.Height; y++)
                        {
                            bitmap.SetPixel(
                                (int)packedTexture.Position.X + x, 
                                (int)packedTexture.Position.Y + y, 
                                System.Drawing.Color.FromArgb(
                                    data[x + y * packedTexture.Texture.Texture.Width].A,
                                    data[x + y * packedTexture.Texture.Texture.Width].R,
                                    data[x + y * packedTexture.Texture.Texture.Width].G,
                                    data[x + y * packedTexture.Texture.Texture.Width].B));
                        }
                    }
                }
            }

            byte[] bitmapData;
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                var length = stream.Position;
                stream.Seek(0, SeekOrigin.Begin);
                bitmapData = new byte[length];
                stream.Read(bitmapData, 0, (int)length);
            }

            var textureAsset = new TextureAsset(
                this.m_AssetContentManager,
                asset.Name,
                bitmapData,
                null,
                true);

            this.m_TextureAssetCompiler.Compile(
                textureAsset,
                platform);

            var uvMappings = new Dictionary<string, UVMapping>();

            foreach (var texture in packedTextures)
            {
                uvMappings.Add(
                    texture.Texture.Name,
                    new UVMapping
                    {
                        TopLeft = texture.Position / new Vector2(size.X, size.Y),
                        BottomRight = (texture.Position + texture.Size) / new Vector2(size.X, size.Y),
                    });
            }

            asset.SetCompiledData(
                textureAsset,
                uvMappings);
        }
    }
}

#endif
