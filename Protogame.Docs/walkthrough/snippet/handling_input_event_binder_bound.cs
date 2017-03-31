using System;
using Microsoft.Xna.Framework.Input;
using Protogame;

namespace MyProject
{
    public class MyProjectEventBinder : StaticEventBinder<IGameContext>
    {
        public override void Configure()
        {
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.Up).On<PlayerEntity>().To<MoveUpwardAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.Down).On<PlayerEntity>().To<MoveDownwardAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.Left).On<PlayerEntity>().To<MoveLeftAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.Right).On<PlayerEntity>().To<MoveRightAction>();
        }
    }
}