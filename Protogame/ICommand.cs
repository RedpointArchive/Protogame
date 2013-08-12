namespace Protogame
{
    public interface ICommand
    {
        string[] Names { get; }
        string[] Descriptions { get; }
        
        string Execute(IGameContext gameContext, string name, string[] parameters);
    }
}

