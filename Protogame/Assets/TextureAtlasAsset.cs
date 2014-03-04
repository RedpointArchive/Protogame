namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// This is an asset representing a generated texture atlas.
    /// </summary>
    public class TextureAtlasAsset : MarshalByRefObject, IAsset
    {
        public TextureAtlasAsset(
            string name,
            Texture2D textureAtlas,
            Dictionary<string, UVMapping> mappings)
        {
            this.Name = name;
            this.TextureAtlas = new TextureAsset(textureAtlas);
            this.Mappings = mappings;
        }

        public bool SourceOnly 
        {
            get
            {
                return false;
            }
        }

        public bool CompiledOnly
        {
            get
            {
                return true;
            }
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
        public TextureAsset TextureAtlas { get; private set; }

        /// <summary>
        /// Gets the mappings of texture names to rectangle bounds (as pixels).
        /// </summary>
        /// <value>The mappings.</value>
        public Dictionary<string, UVMapping> Mappings { get; private set; } 

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

