namespace Protogame
{
    using System;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Ninject.Planning.Targets;

    public class WanderAI : IAI
    {
        private Vector2 m_WanderTarget;

        public float WanderDistance { get; set; }

        public float WanderRadius { get; set; }

        public float WanderJitter { get; set; }

        public float WanderWeight { get; set; }

        private static Random m_Random = new Random();

        public WanderAI()
        {
            this.m_WanderTarget = new Vector2(1, 0);
            this.WanderDistance = 30f;
            this.WanderRadius = 10f;
            this.WanderJitter = 2f;
            this.WanderWeight = 0.003f;
        }

        public Vector2 Update(IGameContext gameContext, IUpdateContext updateContext, Agent agent)
        {
            var wt = this.m_WanderTarget;

            wt += new Vector2(((float)m_Random.NextDouble() * 2f) - 1f, ((float)m_Random.NextDouble() * 2f) - 1f) * this.WanderJitter;

            wt = Vector2.Normalize(wt) * this.WanderRadius;

            this.m_WanderTarget = wt;

            var target = wt + new Vector2(this.WanderDistance, 0);

            var wldTarget = agent.Project(target);

            return (wldTarget - agent.Position) * this.WanderWeight;
        }

        public void RenderDebug(IGameContext gameContext, IRenderContext renderContext, Agent agent, I2DRenderUtilities twoDRenderUtilities)
        {
            var wndPos = new Vector2(this.WanderDistance, 0);
            var wldPos = agent.Project(wndPos);

            for (var i = 0; i < 10; i++)
            {
                var r = MathHelper.ToRadians(36 * i);
                var r2 = MathHelper.ToRadians(36 * (i + 1));
                twoDRenderUtilities.RenderLine(
                    renderContext,
                    wldPos + (new Vector2((float)Math.Sin(r), (float)Math.Cos(r)) * this.WanderRadius),
                    wldPos + (new Vector2((float)Math.Sin(r2), (float)Math.Cos(r2)) * this.WanderRadius),
                    Color.Green);
            }

            wndPos = this.m_WanderTarget + new Vector2(this.WanderDistance, 0);
            wldPos = agent.Project(wndPos);

            for (var i = 0; i < 10; i++)
            {
                var r = MathHelper.ToRadians(36 * i);
                var r2 = MathHelper.ToRadians(36 * (i + 1));
                twoDRenderUtilities.RenderLine(
                    renderContext,
                    wldPos + (new Vector2((float)Math.Sin(r), (float)Math.Cos(r)) * 3),
                    wldPos + (new Vector2((float)Math.Sin(r2), (float)Math.Cos(r2)) * 3),
                    Color.Red);
            }
        }
    }
}