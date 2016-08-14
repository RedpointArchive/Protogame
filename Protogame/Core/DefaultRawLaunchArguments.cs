namespace Protogame
{
    public class DefaultRawLaunchArguments : IRawLaunchArguments
    {
        public DefaultRawLaunchArguments(string[] args)
        {
            Arguments = args;
        }

        public string[] Arguments { get; }
    }
}