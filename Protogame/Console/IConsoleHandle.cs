namespace Protogame
{
    public interface IConsoleHandle
    {
        void Log(string messageFormat, params object[] objects);
    }
}
