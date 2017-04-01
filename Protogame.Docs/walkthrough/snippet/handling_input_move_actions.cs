using System;
using Protogame;

namespace MyProject
{
    public class MoveUpwardAction : IEventEntityAction<PlayerEntity>
    {
        public void Handle(IGameContext context, PlayerEntity entity, Event @event)
        {
            entity.Y -= 4;
        }
    }

    public class MoveDownwardAction : IEventEntityAction<PlayerEntity>
    {
        public void Handle(IGameContext context, PlayerEntity entity, Event @event)
        {
            entity.Y += 4;
        }
    }

    public class MoveLeftAction : IEventEntityAction<PlayerEntity>
    {
        public void Handle(IGameContext context, PlayerEntity entity, Event @event)
        {
            entity.X -= 4;
        }
    }

    public class MoveRightAction : IEventEntityAction<PlayerEntity>
    {
        public void Handle(IGameContext context, PlayerEntity entity, Event @event)
        {
            entity.X += 4;
        }
    }
}