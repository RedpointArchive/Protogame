namespace Protogame
{
    public interface ICommand
    {
        string[] Names { get; }
        string[] Descriptions { get; }
        
        string Execute(string name, string[] parameters);
    }
}

