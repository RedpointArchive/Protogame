namespace Protogame
{
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// The default 3 d world manager.
    /// </summary>
    public class Default3DWorldManager : IWorldManager
    {
        /// <summary>
        /// The m_ console.
        /// </summary>
        private readonly IConsole m_Console;

        /// <summary>
        /// Initializes a new instance of the <see cref="Default3DWorldManager"/> class.
        /// </summary>
        /// <param name="console">
        /// The console.
        /// </param>
        public Default3DWorldManager(IConsole console)
        {
            this.m_Console = console;
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="game">
        /// The game.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public void Render<T>(T game) where T : Game, ICoreGame
        {
            game.RenderContext.Render(game.GameContext);

#if PLATFORM_WINDOWS
            if (game.RenderContext.GraphicsDevice != null)
            {
                game.RenderContext.GraphicsDevice.Viewport = new Viewport(
                    0,
                    0,
                    game.Window.ClientBounds.Width,
                    game.Window.ClientBounds.Height);
            }
#endif

            game.RenderContext.Is3DContext = true;

            game.GameContext.World.RenderBelow(game.GameContext, game.RenderContext);

            foreach (var entity in game.GameContext.World.Entities.ToList())
            {
                entity.Render(game.GameContext, game.RenderContext);
            }

            game.GameContext.World.RenderAbove(game.GameContext, game.RenderContext);

            game.RenderContext.Is3DContext = false;

            game.RenderContext.SpriteBatch.Begin();

            foreach (var pass in game.RenderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                game.GameContext.World.RenderBelow(game.GameContext, game.RenderContext);

                foreach (var entity in game.GameContext.World.Entities.OrderBy(x => x.Z))
                {
                    entity.Render(game.GameContext, game.RenderContext);
                }

                game.GameContext.World.RenderAbove(game.GameContext, game.RenderContext);
            }

            this.m_Console.Render(game.GameContext, game.RenderContext);

            game.RenderContext.SpriteBatch.End();

            game.GraphicsDevice.BlendState = BlendState.Opaque;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="game">
        /// The game.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public void Update<T>(T game) where T : Game, ICoreGame
        {
            game.UpdateContext.Update(game.GameContext);

            foreach (var entity in game.GameContext.World.Entities.ToList())
            {
                entity.Update(game.GameContext, game.UpdateContext);
            }

            game.GameContext.World.Update(game.GameContext, game.UpdateContext);

            this.m_Console.Update(game.GameContext, game.UpdateContext);
        }
    }
}