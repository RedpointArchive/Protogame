#if PLATFORM_WINDOWS || PLATFORM_LINUX

namespace Protogame
{
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Drawing;
    using System.IO;
    using System.Drawing.Imaging;

    public class TextureAtlasAssetGenerator : IAssetGenerator
    {
        public string[] Provides()
        {
            return new[] { "generated.TextureAtlas" };
        }

        public IAsset Generate(IAssetManager assetManager, string name)
        {
            var textures = assetManager.GetAll().OfType<TextureAsset>();

            var texturePacker = new TexturePacker<TextureAsset>();

            foreach (var texture in textures)
            {
                texturePacker.AddTexture(
                    new Vector2(texture.Texture.Width, texture.Texture.Height),
                    texture);
            }

            Vector2 size;
            var packedTextures = texturePacker.Pack(out size);

            var bitmap = new Bitmap((int)size.X, (int)size.Y);

            foreach (var texture in packedTextures)
            {
                var data = new Color[texture.Texture.Texture.Width * texture.Texture.Texture.Height];
                texture.Texture.Texture.GetData(data);

                for (var x = 0; x < texture.Texture.Texture.Width; x++)
                {
                    for (var y = 0; y < texture.Texture.Texture.Height; y++)
                    {
                        bitmap.SetPixel(
                            (int)texture.Position.X + x, 
                            (int)texture.Position.Y + y, 
                            System.Drawing.Color.FromArgb(
                                data[x + y * texture.Texture.Texture.Width].A,
                                data[x + y * texture.Texture.Texture.Width].R,
                                data[x + y * texture.Texture.Texture.Width].G,
                                data[x + y * texture.Texture.Texture.Width].B));
                    }
                }
            }

            // TODO: Finish implementation.
            // bitmap.Save("test.png");

            return null;
        }
    }
}

#endif
