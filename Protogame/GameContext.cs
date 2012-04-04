using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Protogame
{
    public class GameContext
    {
        public ContentManager Content { get; set; }
        public GraphicsDeviceManager Graphics { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public World World { get; set; }
        public Dictionary<string, Texture2D> Textures { get; set; }
        public Dictionary<string, SpriteFont> Fonts { get; set; }
        public Dictionary<string, SoundEffect> Sounds { get; set; }

        internal GameContext()
        {
            this.Textures = new Dictionary<string, Texture2D>();
            this.Sounds = new Dictionary<string, SoundEffect>();
            this.Fonts = new Dictionary<string, SpriteFont>();
        }

        public void LoadFont(string name)
        {
            this.Fonts.Add(name, this.Content.Load<SpriteFont>(name.Replace('.', '/')));
        }

        public void LoadTexture(string name)
        {
            this.Textures.Add(name, this.Content.Load<Texture2D>(name.Replace('.', '/')));
        }

        public void LoadTextureAnim(string name, int end)
        {
            for (int i = 1; i <= end; i++)
                this.LoadTexture(name + i.ToString());
        }

        public void LoadAudio(string name)
        {
            this.Sounds.Add(name, this.Content.Load<SoundEffect>(name.Replace('.', '/')));
        }
    }
}
