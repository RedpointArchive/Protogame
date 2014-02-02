namespace Protogame
{
#if LEGACY
    public abstract class FadingWorld : IWorld
    {
        private string m_LevelChangeTarget = null;
        private bool m_ChangingLevel = false;
        private float m_FadeLevel = 0f;
        private bool m_FadingIn = false;

        public override void LoadLevel(string name)
        {
            if (this.m_LevelChangeTarget == null)
                this.LoadLevelImmediate(name);
            else
            {
                this.m_ChangingLevel = true;
                this.m_FadingIn = true;
            }
            this.m_LevelChangeTarget = name;
        }

        public override void DrawAbove(GameContext context)
        {
            // Fade the screen.
            if (!context.Textures.Keys.Contains("black"))
                throw new ProtogameException("You must load a texture with the name 'black' in order for FadingWorld to fade to black.  Check to make sure you are doing this in the LoadContent of your game.");
            context.SpriteBatch.Draw(context.Textures["black"], new Rectangle(0, 0, Tileset.TILESET_PIXEL_WIDTH, Tileset.TILESET_PIXEL_HEIGHT), new Color(1.0f, 1.0f, 1.0f, this.Fade));
        }

        public override bool Update(GameContext context)
        {
            // Do our fade effect if needed.
            if (this.m_ChangingLevel || this.m_FadingIn)
            {
                // Check to see what stage of fading we're at.
                if (this.m_FadingIn && this.m_FadeLevel < 1f)
                    this.m_FadeLevel += 0.05f;
                else if (!this.m_FadingIn && this.m_FadeLevel > 0f)
                {
                    this.m_FadeLevel -= 0.05f;
                    return false;
                }
                else if (this.m_FadingIn && this.m_FadeLevel >= 1f)
                {
                    this.LoadLevelImmediate(this.m_LevelChangeTarget);
                    this.m_FadingIn = false;
                }
                else if (!this.m_FadingIn && this.m_FadeLevel <= 0f)
                {
                    this.m_ChangingLevel = false;
                    return false;
                }
            }

            // Get the base class to also update.
            return true;
        }

        public float Fade
        {
            get
            {
                return this.m_FadeLevel;
            }
        }
    }
#endif
}