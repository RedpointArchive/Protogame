using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Protogame
{
    public class AudioPlayer : IContainer
    {
        private readonly Button _toggleButton;
        
        private SoundEffectInstance _instance;
        
        public AudioPlayer()
        {
            Audio = null;
            _toggleButton = new Button { Text = "Play" };
            _toggleButton.Click += (sender, e) =>
            {
                if (_instance == null)
                {
                    _instance = Audio.Audio.CreateInstance();
                    _instance.Play();
                    _toggleButton.Text = "Stop";
                }
                else
                {
                    _toggleButton.Text = "Play";
                    _instance.Stop();
                    _instance = null;
                }
            };
        }
        
        public AudioAsset Audio { get; set; }
        
        public IContainer[] Children => new[] { _toggleButton };
        
        public bool Focused { get; set; }

        public int Order { get; set; }

        public IContainer Parent { get; set; }

        public object Userdata { get; set; }

        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
            _toggleButton.Render(context, skinLayout, skinDelegator, new Rectangle(layout.X, layout.Y, layout.Width, 24));
        }

        public void Update(ISkinLayout skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            _toggleButton.Update(
                skin, 
                new Rectangle(layout.X, layout.Y, layout.Width, 24), 
                gameTime, 
                ref stealFocus);
            if (_instance != null && _instance.State == SoundState.Stopped)
            {
                _toggleButton.Text = "Play";
                _instance.Stop();
                _instance = null;
            }
        }

        public bool HandleEvent(ISkinLayout skin, Rectangle layout, IGameContext context, Event @event)
        {
            return _toggleButton.HandleEvent(skin, layout, context, @event);
        }
    }
}