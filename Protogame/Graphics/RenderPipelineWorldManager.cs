using System.Linq;

namespace Protogame
{
    /// <summary>
    /// An implementation of <see cref="IWorldManager" /> which uses the new rendering
    /// pipeline in Protogame.  You should use this as the world manager for all new
    /// games going forward, or make your game inherit from <see cref="CoreGame{T}" />,
    /// which will default to this world manager.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    public class RenderPipelineWorldManager : IWorldManager
    {
        private readonly IRenderPipeline _renderPipeline;

        public RenderPipelineWorldManager(IRenderPipeline renderPipeline)
        {
            _renderPipeline = renderPipeline;
        }

        public IRenderPipeline RenderPipeline
        {
            get { return _renderPipeline; }
        }

        public void Render<T>(T game) where T : ICoreGame
        {
            _renderPipeline.Render(game.GameContext, game.RenderContext);
        }

        public void Update<T>(T game) where T : ICoreGame
        {
            game.UpdateContext.Update(game.GameContext);

            if (game.GameContext.World != null)
            {
                foreach (var entity in game.GameContext.World.GetEntitiesForWorld(game.GameContext.Hierarchy).ToList())
                {
                    entity.Update(game.GameContext, game.UpdateContext);
                }

                game.GameContext.World.Update(game.GameContext, game.UpdateContext);
            }
        }
    }
}

