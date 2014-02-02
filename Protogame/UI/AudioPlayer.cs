namespace Protogame
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;

    /// <summary>
    /// The audio player.
    /// </summary>
    public class AudioPlayer : IContainer
    {
        /// <summary>
        /// The m_ toggle button.
        /// </summary>
        private readonly Button m_ToggleButton;

        /// <summary>
        /// The m_ instance.
        /// </summary>
        private SoundEffectInstance m_Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioPlayer"/> class.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the audio.
        /// </summary>
        /// <value>
        /// The audio.
        /// </value>
        public AudioAsset Audio { get; set; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public IContainer[] Children
        {
            get
            {
                return new[] { this.m_ToggleButton };
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether focused.
        /// </summary>
        /// <value>
        /// The focused.
        /// </value>
        public bool Focused { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public IContainer Parent { get; set; }

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawAudioPlayer(context, layout, this);
            this.m_ToggleButton.Draw(context, skin, new Rectangle(layout.X, layout.Y, layout.Width, 24));
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        /// <param name="stealFocus">
        /// The steal focus.
        /// </param>
        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            this.m_ToggleButton.Update(
                skin, 
                new Rectangle(layout.X, layout.Y, layout.Width, 24), 
                gameTime, 
                ref stealFocus);
            if (this.m_Instance != null && this.m_Instance.State == SoundState.Stopped)
            {
                this.m_ToggleButton.Text = "Play";
                this.m_Instance.Stop();
                this.m_Instance = null;
            }
        }
    }
}