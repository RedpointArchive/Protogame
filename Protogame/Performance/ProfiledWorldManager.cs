using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class ProfiledWorldManager : IWorldManager
    {
        private Effect m_Effect;
        private IProfiler m_Profiler;
        
        public ProfiledWorldManager(IProfiler profiler)
        {
            this.m_Profiler = profiler;
        }
        
        public void Render<T>(T game) where T : Game, ICoreGame
        {
            if (this.m_Effect == null)
                this.m_Effect = new BasicEffect(game.GraphicsDevice);
            
            using (this.m_Profiler.Measure("render_context"))
            {
                game.RenderContext.Render(game.GameContext);
            }
            
            using (this.m_Profiler.Measure("sprite_batch_begin"))
            {
                game.RenderContext.SpriteBatch.Begin();
            }
            
            using (this.m_Profiler.Measure("effect_passes"))
            {
                foreach (var pass in this.m_Effect.CurrentTechnique.Passes)
                {
                    using (this.m_Profiler.Measure("apply_pass", pass.Name))
                    {
                        pass.Apply();
                    }
                
                    using (this.m_Profiler.Measure("render_below"))
                    {
                        game.GameContext.World.RenderBelow(game.GameContext, game.RenderContext);
                    }
                
                    using (this.m_Profiler.Measure("render_entities"))
                    {
                        foreach (var entity in game.GameContext.World.Entities)
                            entity.Render(game.GameContext, game.RenderContext);
                    }
                
                    using (this.m_Profiler.Measure("render_above"))
                    {
                        game.GameContext.World.RenderAbove(game.GameContext, game.RenderContext);
                    }
                }
            }
            
            using (this.m_Profiler.Measure("sprite_batch_end"))
            {
                game.RenderContext.SpriteBatch.End();
            }
        }
        
        public void Update<T>(T game) where T : Game, ICoreGame
        {
            using (this.m_Profiler.Measure("update_context"))
            {
                game.UpdateContext.Update(game.GameContext);
            }
            
            using (this.m_Profiler.Measure("update_entities"))
            {
                foreach (var entity in game.GameContext.World.Entities)
                    entity.Update(game.GameContext, game.UpdateContext);
            }
            
            using (this.m_Profiler.Measure("update_world"))
            {
                game.GameContext.World.Update(game.GameContext, game.UpdateContext);
            }
        }
    }
}

