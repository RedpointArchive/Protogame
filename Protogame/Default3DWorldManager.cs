using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Protogame
{
    public class Default3DWorldManager : IWorldManager
    {
        public void Render<T>(T game) where T : Game, ICoreGame
        {
            game.RenderContext.Render(game.GameContext);
            
            game.RenderContext.Is3DContext = true;
            
            game.GameContext.World.RenderBelow(game.GameContext, game.RenderContext);
        
            foreach (var entity in game.GameContext.World.Entities)
                entity.Render(game.GameContext, game.RenderContext);
        
            game.GameContext.World.RenderAbove(game.GameContext, game.RenderContext);
            
            game.RenderContext.Is3DContext = false;
            
            game.RenderContext.SpriteBatch.Begin();
            
            foreach (var pass in game.RenderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                game.GameContext.World.RenderBelow(game.GameContext, game.RenderContext);
            
                foreach (var entity in game.GameContext.World.Entities.OrderBy(x => x.Z))
                    entity.Render(game.GameContext, game.RenderContext);
            
                game.GameContext.World.RenderAbove(game.GameContext, game.RenderContext);
            }
            
            game.RenderContext.SpriteBatch.End();
        }
        
        public void Update<T>(T game) where T : Game, ICoreGame
        {
            game.UpdateContext.Update(game.GameContext);
            
            foreach (var entity in game.GameContext.World.Entities)
                entity.Update(game.GameContext, game.UpdateContext);
            
            game.GameContext.World.Update(game.GameContext, game.UpdateContext);
        }
    }
}

