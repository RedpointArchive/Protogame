using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    class DefaultWorldManager : IWorldManager
    {
        private Effect m_Effect;
        
        public void Render<T>(T game) where T : Game, ICoreGame
        {
            if (this.m_Effect == null)
                this.m_Effect = new BasicEffect(game.GraphicsDevice);
            
            game.RenderContext.Render(game.GameContext);
            
            foreach (var pass in this.m_Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                game.GameContext.World.RenderBelow(game.GameContext, game.RenderContext);
            
                // TODO: Do entity rendering here.
            
                game.GameContext.World.RenderAbove(game.GameContext, game.RenderContext);
            }
        }
        
        public void Update<T>(T game) where T : Game, ICoreGame
        {
            game.UpdateContext.Update(game.GameContext);
            
            game.GameContext.World.Update(game.GameContext, game.UpdateContext);
        }
    }
}

