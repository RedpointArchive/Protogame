using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public abstract class World
    {
        /// <summary>
        /// The current game context.  Only used by audio entities so that they can
        /// access the Sounds property on creation without the developer having to specify
        /// the GameContext (and without the world having to expose a copy of the Sounds
        /// list).
        /// </summary>
        internal GameContext GameContext { get; set; }

        /// <summary>
        /// The current tileset.
        /// </summary>
        public Tileset Tileset { get; private set; }

        /// <summary>
        /// A list of entities that are currently active in the world.
        /// </summary>
        public List<IEntity> Entities { get; private set; }

        /// <summary>
        /// The name of the current level that is loaded.
        /// </summary>
        public string CurrentLevel { get; private set; }

        /// <summary>
        /// The current world tick.
        /// </summary>
        public long Tick { get; internal set; }

        /// <summary>
        /// Creates a new game world.
        /// </summary>
        public World()
        {
            this.Tileset = null;
            this.Entities = new List<IEntity>();
        }

        /// <summary>
        /// Loads the specified Ogmo Editor level from the Resources/ folder, performing transition
        /// effects as this world defines them.
        /// </summary>
        /// <param name="name">The name of the Ogmo Editor level as it appears on disk, without the extension or leading path.</param>
        public virtual void LoadLevel(string name)
        {
            this.LoadLevelImmediate(name);
        }

        /// <summary>
        /// Loads the specified Ogmo Editor level from the Resources/ folder immediately, taking care
        /// of stopping audio entities, clearing the player and recreating the world from the tileset.
        /// </summary>
        /// <param name="name">The name of the Ogmo Editor level as it appears on disk, without the extension or leading path.</param>
        protected void LoadLevelImmediate(string name)
        {
            // Perform the actual level switch.
            foreach (IEntity ee in this.Entities)
                if (ee is AudioEntity)
                    (ee as AudioEntity).Stop();
            this.Entities.Clear();
            this.Tileset = TilesetXmlLoader.Load("Resources/" + name + ".oel");
            this.CurrentLevel = name;

            foreach (Tile t in this.Tileset.AsLinear())
                if (t is EntityTile)
                    this.Entities.Add(t as EntityTile);
        }
        
        /// <summary>
        /// Performs any custom drawing events for this world, prior to everything else being drawn.
        /// </summary>
        /// <param name="context">The game context.</param>
        public abstract void DrawBelow(GameContext context);

        /// <summary>
        /// Performs any custom drawing events for this world, after everything else has been drawn.
        /// </summary>
        /// <param name="context">The game context.</param>
        public abstract void DrawAbove(GameContext context);

        /// <summary>
        /// Updates the game world each tick.  Returns whether processing should continue in this tick or
        /// whether entity processing should be skipped.
        /// </summary>
        /// <param name="context">The current context of the game.</param>
        /// <returns>Whether processing should continue in this tick.</returns>
        public abstract bool Update(GameContext context);
    }
}
