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

            this.PixelOverscan = 2;
        }

        public int PixelOverscan { get; set; }

        public void Compile(TextureAtlasAsset asset, TargetPlatform platform)
        {
            var textures = asset.AssetManager
                .GetAllNames()
                .Where(x => asset.SourceTextureNames.Contains(x))
                .Select(x => asset.AssetManager.Get<TextureAsset>(x))
                .ToList();

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
                    new Vector2(width + this.PixelOverscan * 2, height + this.PixelOverscan * 2),
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
                    for (var x = -this.PixelOverscan; x < fallback.Width + this.PixelOverscan; x++)
                    {
                        for (var y = -this.PixelOverscan; y < fallback.Height + this.PixelOverscan; y++)
                        {
                            bitmap.SetPixel(
                                (int)packedTexture.Position.X + this.PixelOverscan + x, 
                                (int)packedTexture.Position.Y + this.PixelOverscan + y, 
                                fallback.GetPixel(
                                    MathHelper.Clamp(x, 0, fallback.Width - 1),
                                    MathHelper.Clamp(y, 0, fallback.Height - 1)));
                        }
                    }
                }
                else
                {
                    var data = new Microsoft.Xna.Framework.Color[packedTexture.Texture.Texture.Width * packedTexture.Texture.Texture.Height];
                    packedTexture.Texture.Texture.GetData(data);

                    for (var x = -this.PixelOverscan; x < packedTexture.Texture.Texture.Width + this.PixelOverscan; x++)
                    {
                        for (var y = -this.PixelOverscan; y < packedTexture.Texture.Texture.Height + this.PixelOverscan; y++)
                        {
                            var ax = MathHelper.Clamp(x, 0, packedTexture.Texture.Texture.Width - 1);
                            var ay = MathHelper.Clamp(y, 0, packedTexture.Texture.Texture.Height - 1);

                            bitmap.SetPixel(
                                (int)packedTexture.Position.X + this.PixelOverscan + x, 
                                (int)packedTexture.Position.Y + this.PixelOverscan + y, 
                                System.Drawing.Color.FromArgb(
                                    data[ax + ay * packedTexture.Texture.Texture.Width].A,
                                    data[ax + ay * packedTexture.Texture.Texture.Width].R,
                                    data[ax + ay * packedTexture.Texture.Texture.Width].G,
                                    data[ax + ay * packedTexture.Texture.Texture.Width].B));
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

            bitmap.Save("atlas.png");

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
            var pixelVector = new Vector2(this.PixelOverscan, this.PixelOverscan);

            foreach (var texture in packedTextures)
            {
                uvMappings.Add(
                    texture.Texture.Name,
                    new UVMapping
                    {
                        TopLeft = (texture.Position + pixelVector) / new Vector2(size.X, size.Y),
                        BottomRight = (texture.Position + texture.Size - pixelVector) / new Vector2(size.X, size.Y),
                    });
            }

            asset.SetCompiledData(
                textureAsset,
                uvMappings);
        }
    }
}

#endif
