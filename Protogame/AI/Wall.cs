namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a wall which blocks raycasts by AI agents.
    /// </summary>
    /// <remarks>
    /// These are placed such that is start to end were to make a circle, the normals
    /// face outward (on the left hand side from start to end).
    /// </remarks>
    /// <module>AI</module>
    public class Wall : IEntity
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        public Wall(I2DRenderUtilities twoDRenderUtilities, Vector2 start, Vector2 end)
        {
            this.Start = start;
            this.End = end;
            this.DebugRender = false;
            this.m_2DRenderUtilities = twoDRenderUtilities;
            this.DebugRenderWallColor = Color.Black;
            this.DebugRenderWallNormalColor = Color.DarkGray;
            Transform = new DefaultTransform();
        }

        public Vector2 Start { get; set; }

        public Vector2 End { get; set; }

        public bool DebugRender { get; set; }

        public Color DebugRenderWallColor { get; set; }

        public Color DebugRenderWallNormalColor { get; set; }

        public float X
        {
            get
            {
                return this.Start.X;
            }
            set
            {
                this.Start = new Vector2(value, this.Start.Y);
            }
        }

        public float Y
        {
            get
            {
                return this.Start.Y;
            }
            set
            {
                this.Start = new Vector2(this.Start.X, value);
            }
        }

        public float Z { get; set; }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!this.DebugRender)
            {
                return;
            }

            if (renderContext.IsCurrentRenderPass<I3DRenderPass>())
            {
                return;
            }

            this.m_2DRenderUtilities.RenderLine(renderContext, this.Start, this.End, DebugRenderWallColor);

            // Show normal.
            var diff = this.End - this.Start;
            var mid = this.Start + (diff / 2);

            this.m_2DRenderUtilities.RenderLine(renderContext, mid, mid + (this.Normal * 4), DebugRenderWallNormalColor);
        }

        public Vector2 Normal
        {
            get
            {
                var diff = this.End - this.Start;
                return
                    Vector2.Normalize(
                        Vector2.Transform(Vector2.Normalize(diff), Matrix.CreateRotationZ(-MathHelper.PiOver2)));
            }
        }

        public static Vector2 NormalOf(Vector2 start, Vector2 end)
        {
            var diff = end - start;
            return
                Vector2.Normalize(
                    Vector2.Transform(Vector2.Normalize(diff), Matrix.CreateRotationZ(-MathHelper.PiOver2)));
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }

        public ITransform Transform { get; }

        public IFinalTransform FinalTransform
        {
            get { return this.GetDetachedFinalTransformImplementation(); }
        }
    }
}
