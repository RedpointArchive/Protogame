using System;
using System.Collections.Generic;
using System.Linq;

namespace Protogame
{
    /// <summary>
    /// A group of Mx clients, identified by a user specified ID.  This allows you
    /// to ensure you have correct behaviour in your system, even when a client
    /// connects from multiple endpoints.
    /// </summary>
    public class MxClientGroup : IEquatable<MxClientGroup>
    {
        private readonly MxDispatcher _dispatcher;

        public const string Ungrouped = "";

        public MxClientGroup(MxDispatcher dispatcher, string identifier)
        {
            _dispatcher = dispatcher;
            Identifier = identifier;
            RealtimeClients = new List<MxClient>();
            ReliableClients = new List<MxReliability>();
        }

        public string Identifier { get; }

        /// <summary>
        /// The list of clients in this group.  You should not modify
        /// this collection directly; instead use <see cref="MxDispatcher.PlaceInGroup"/>.
        /// </summary>
        public List<MxClient> RealtimeClients { get; }

        public List<MxReliability> ReliableClients { get; }

        /// <summary>
        /// Isolates the group to use only one, connected Mx client, and disconnects
        /// the rest.
        /// </summary>
        public void Isolate()
        {
            if (RealtimeClients.Count == 0)
            {
                return;
            }

            var selectedClient = RealtimeClients.First(x => x.HasReceivedPacket);
            foreach (var client in RealtimeClients.ToArray())
            {
                if (!client.Endpoint.Equals(selectedClient.Endpoint))
                {
                    _dispatcher.Disconnect(client);
                }
            }
        }

        public override string ToString()
        {
            var id = Identifier;
            if (id == Ungrouped)
            {
                id = "<UNGROUPED>";
            }

            if (RealtimeClients.Count == 0)
            {
                return "(" + id + "@<DISCONNECTED>)";
            }
            
            return "(" + id + "@" + RealtimeClients.Select(e => e.Endpoint.Address + ":" + e.Endpoint.Port).Aggregate((a, b) => a +"," + b) + ")";
        }

        public bool Equals(MxClientGroup other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Identifier, other.Identifier, StringComparison.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MxClientGroup) obj);
        }

        public override int GetHashCode()
        {
            return (Identifier != null ? StringComparer.InvariantCulture.GetHashCode(Identifier) : 0);
        }

        public static bool operator ==(MxClientGroup left, MxClientGroup right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MxClientGroup left, MxClientGroup right)
        {
            return !Equals(left, right);
        }
    }
}
