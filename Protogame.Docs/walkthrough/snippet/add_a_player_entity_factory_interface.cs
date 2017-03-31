using System;
using Protoinject;

namespace MyGame
{
    public interface IEntityFactory : IGenerateFactory
    {
        ExampleEntity CreateExampleEntity(string name);
        
        PlayerEntity CreatePlayerEntity();
    }
}