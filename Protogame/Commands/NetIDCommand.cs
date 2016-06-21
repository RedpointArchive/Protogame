namespace Protogame
{
    using System;

    /// <summary>
    /// The "netid" command, which assists looking up and
    /// finding information about network tracked objects.
    /// </summary>
    public class NetIDCommand : ICommand
    {
        private readonly INetworkEngine _networkEngine;

        public NetIDCommand(INetworkEngine networkEngine)
        {
            _networkEngine = networkEngine;
        }

        public string[] Descriptions
        {
            get
            {
                return new[] { "Commands for looking up and inspecting network tracked objects." };
            }
        }
        
        public string[] Names
        {
            get
            {
                return new[] { "netid" };
            }
        }
        
        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            var helpText = "Expected subcommand, one of: 'type', 'list'.";

            if (parameters.Length == 0)
            {
                return helpText;
            }

            switch (parameters[0])
            {
                case "type":
                    if (parameters.Length == 1)
                    {
                        return "Expected numeric ID to lookup.";
                    }

                    try
                    {
                        var lookup = _networkEngine.FindObjectByNetworkId(int.Parse(parameters[1]));
                        return lookup.GetType().FullName;
                    }
                    catch (Exception ex)
                    {
                        return "Error: " + ex.Message;
                    }
                case "list":
                    var result = string.Empty;
                    try
                    {
                        var entries = _networkEngine.ListObjectsByNetworkId();
                        foreach (var kv in entries)
                        {
                            result += kv.Key + " ";

                            try
                            {
                                result += kv.Value.GetType().FullName;
                            }
                            catch (Exception)
                            {
                                result += "(exception while getting object)";
                            }

                            result += "\n";
                        }

                        return result;
                    }
                    catch (Exception ex)
                    {
                        return "Error: " + ex.Message;
                    }
            }

            return helpText;
        }
    }
}