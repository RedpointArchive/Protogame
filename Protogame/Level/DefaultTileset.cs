namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The default tileset.
    /// </summary>
    /// <module>Level</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.ITileset</interface_ref>
    public class DefaultTileset : ITileset
    {
        private readonly IFinalTransform _finalTransform;

        /// <summary>
        /// The m_ cell size height.
        /// </summary>
        private int m_CellSizeHeight;

        /// <summary>
        /// The m_ cell size width.
        /// </summary>
        private int m_CellSizeWidth;

        /// <summary>
        /// The m_ entities.
        /// </summary>
        private IEntity[] m_Entities;

        /// <summary>
        /// The m_ tileset height.
        /// </summary>
        private int m_TilesetHeight;

        /// <summary>
        /// The m_ tileset width.
        /// </summary>
        private int m_TilesetWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTileset"/> class.
        /// </summary>
        public DefaultTileset()
        {
            _finalTransform = new DefaultFinalTransform(this, null);
            this.m_Entities = new IEntity[0];
            this.m_CellSizeWidth = 0;
            this.m_CellSizeHeight = 0;
            this.m_TilesetWidth = 0;
            this.m_TilesetHeight = 0;
        }

        

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="IEntity"/>.
        /// </returns>
        public IEntity this[int x, int y]
        {
            get
            {
                return this.m_Entities[x + y * this.m_TilesetWidth];
            }

            set
            {
                this.m_Entities[x + y * this.m_TilesetWidth] = value;
            }
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            for (var i = 0; i < this.m_Entities.Length; i++)
            {
                this.m_Entities[i].Render(gameContext, renderContext);
            }
        }

        /// <summary>
        /// The set size.
        /// </summary>
        /// <param name="cellSize">
        /// The cell size.
        /// </param>
        /// <param name="tilesetSize">
        /// The tileset size.
        /// </param>
        public void SetSize(Vector2 cellSize, Vector2 tilesetSize)
        {
            this.m_Entities = new IEntity[(int)tilesetSize.X * (int)tilesetSize.Y];
            this.m_CellSizeWidth = (int)cellSize.X;
            this.m_CellSizeHeight = (int)cellSize.Y;
            this.m_TilesetWidth = (int)tilesetSize.X;
            this.m_TilesetHeight = (int)tilesetSize.Y;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="updateContext">
        /// The update context.
        /// </param>
        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            for (var i = 0; i < this.m_Entities.Length; i++)
            {
                this.m_Entities[i].Update(gameContext, updateContext);
            }
        }

        public ITransform Transform { get; }

        public IFinalTransform FinalTransform => _finalTransform;
    }
}