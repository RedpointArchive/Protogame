using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Protogame
{
    public class AudioPlayer : IContainer
    {
        private Button m_ToggleButton;
        private SoundEffectInstance m_Instance;
    
        public IContainer[] Children { get { return new[]{ this.m_ToggleButton }; } }
        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public bool Focused { get; set; }
        public AudioAsset Audio { get; set; }

        public AudioPlayer()
        {
            this.Audio = null;
            this.m_ToggleButton = new Button { Text = "Play" };
            this.m_ToggleButton.Click += (sender, e) =>
            {
                if (this.m_Instance == null)
                {
                    this.m_Instance = this.Audio.Audio.CreateInstance();
                    this.m_Instance.Play();
                    this.m_ToggleButton.Text = "Stop";
                }
                else
                {
                    this.m_ToggleButton.Text = "Play";
                    this.m_Instance.Stop();
                    this.m_Instance = null;
                }
            };
        }

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            this.m_ToggleButton.Update(skin, new Rectangle(layout.X, layout.Y, layout.Width, 24), gameTime, ref stealFocus);
            if (this.m_Instance != null && this.m_Instance.State == SoundState.Stopped)
            {
                this.m_ToggleButton.Text = "Play";
                this.m_Instance.Stop();
                this.m_Instance = null;
            }
        }

        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawAudioPlayer(context, layout, this);
            this.m_ToggleButton.Draw(context, skin, new Rectangle(layout.X, layout.Y, layout.Width, 24));
        }
    }
}

