namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// This is an asset representing a generated texture atlas.
    /// </summary>
    public class TextureAtlasAsset : MarshalByRefObject, IAsset
    {
        public TextureAtlasAsset(
            IAssetManager assetManager,
            string name,
            string[] sourceTextureNames)
        {
            this.AssetManager = assetManager;
            this.Name = name;
            this.SourceTextureNames = sourceTextureNames;

            this.SourceOnly = true;
            this.CompiledOnly = false;
        }

        public TextureAtlasAsset(
            IAssetContentManager assetContentManager,
            string name,
            PlatformData data)
        {
            this.Name = name;
            this.SourceOnly = false;
            this.CompiledOnly = true;

            var memory = new MemoryStream(data.Data);
            using (var reader = new BinaryReader(memory))
            {
                var textureSize = reader.ReadInt32();
                var texturePlatformData = reader.ReadBytes(textureSize);

                var textureAsset = new TextureAsset(
                    assetContentManager,
                    name,
                    null,
                    new PlatformData
                    {
                        Platform = data.Platform,
                        Data = texturePlatformData
                    },
                    false);
                this.AtlasTexture = textureAsset;

                this.Mappings = new Dictionary<string, UVMapping>();
                var uvMappingCount = reader.ReadInt32();
                for (var i = 0; i < uvMappingCount; i++)
                {
                    var mappingName = reader.ReadString();
                    var topLeftU = reader.ReadSingle();
                    var topLeftV = reader.ReadSingle();
                    var bottomRightU = reader.ReadSingle();
                    var bottomRightV = reader.ReadSingle();
                    this.Mappings.Add(
                        mappingName, 
                        new UVMapping 
                        { 
                            TopLeft = new Vector2(topLeftU, topLeftV), 
                            BottomRight = new Vector2(bottomRightU, bottomRightV)
                        });
                }
            }
        }

        public TextureAtlasAsset(
            string name,
            Texture2D textureAtlas,
            Dictionary<string, UVMapping> mappings)
        {
            this.Name = name;
            this.AtlasTexture = new TextureAsset(textureAtlas);
            this.Mappings = mappings;

            this.SourceOnly = false;
            this.CompiledOnly = true;
        }

        public bool SourceOnly 
        {
            get;
            private set;
        }

        public bool CompiledOnly
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the game of the asset.
        /// </summary>
        /// <value>The name of the asset.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the texture for this texture atlas.
        /// </summary>
        /// <value>The texture atlas.</value>
        public TextureAsset AtlasTexture { get; private set; }

        public string[] SourceTextureNames { get; private set; }

        public IAssetManager AssetManager { get; private set; }

        /// <summary>
        /// Gets the mappings of texture names to rectangle bounds (as pixels).
        /// </summary>
        /// <value>The mappings.</value>
        public Dictionary<string, UVMapping> Mappings { get; private set; } 

        public void SetCompiledData(
            TextureAsset texture, 
            Dictionary<string, UVMapping> uvMappings)
        {
            this.AtlasTexture = texture;
            this.Mappings = uvMappings;
            this.SourceOnly = false;
        }

        public byte[] GetCompiledData()
        {
            var memory = new MemoryStream();
            using (var writer = new BinaryWriter(memory))
            {
                writer.Write((int)this.AtlasTexture.PlatformData.Data.Length);
                writer.Write(this.AtlasTexture.PlatformData.Data);

                writer.Write((int)this.Mappings.Count);
                foreach (var kv in this.Mappings)
                {
                    writer.Write(kv.Key);
                    writer.Write(kv.Value.TopLeft.X);
                    writer.Write(kv.Value.TopLeft.Y);
                    writer.Write(kv.Value.BottomRight.X);
                    writer.Write(kv.Value.BottomRight.Y);
                }

                var result = new byte[memory.Position];
                memory.Seek(0, SeekOrigin.Begin);
                memory.Read(result, 0, result.Length);
                return result;
            }
        }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(TextureAtlasAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to TextureAtlasAsset.");
        }

        /// <summary>
        /// Returns the UV bounds for a specified texture.
        /// </summary>
        /// <returns>The UV bounds.</returns>
        /// <param name="name">The name of the texture to lookup in the texture atlas.</param>
        public UVMapping GetUVBounds(string name)
        {
            return this.Mappings[name];
        }
    }
}

