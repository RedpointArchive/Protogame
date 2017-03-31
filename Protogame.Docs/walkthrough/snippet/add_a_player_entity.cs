using System;
using Protogame;

namespace MyGame
{
    public class PlayerEntity : IEntity
    {
        // This is the constructor for the player entity.
        public PlayerEntity()
        {
        }

        // These properties indicate the X, Y and Z position of the entity.
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        // This is the update method called by the world manager during the update loop.
        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }

        // This is the render method called by the world manager during the render loop.
        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }
    }
}