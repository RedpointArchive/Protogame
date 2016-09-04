using System;

namespace Protogame
{
    public interface ISynchronisationApi
    {
        /// <summary>
        /// Synchronises a network property to clients and servers.  In the case of server authoritive entities, this
        /// causes the network data to be synchronised to the clients.  In the case of client authoritive entities,
        /// this causes the network data to be synchronised to the server.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="name">The name of the network property.</param>
        /// <param name="frameInterval">The interval of frames at which this property will be synchronised.  Smaller values lead to higher network traffic.</param>
        /// <param name="currentValue">The current value of the property.</param>
        /// <param name="setValue">The action which sets the property based on the value.</param>
        /// <param name="timeMachineHistory">The number of frames of history to store in a time machine, or null if no time machine should be used.</param>
        void Synchronise<T>(string name, int frameInterval, T currentValue, Action<T> setValue, int? timeMachineHistory);

        /// <summary>
        /// Synchronises a network property ONLY to the client which owns the entity.  This can be used to synchronise
        /// private data that only the owning client should know about, as well as send an affirmation that the client
        /// owns the entity using a call such as:
        /// <code>
        /// SynchroniseToOwner("owned", 60, true, x => _localOwned = x, null);
        /// </code>
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="name">The name of the network property.</param>
        /// <param name="frameInterval">The interval of frames at which this property will be synchronised.  Smaller values lead to higher network traffic.</param>
        /// <param name="currentValue">The current value of the property.</param>
        /// <param name="setValue">The action which sets the property based on the value.</param>
        /// <param name="timeMachineHistory">The number of frames of history to store in a time machine, or null if no time machine should be used.</param>
        void SynchroniseToOwner<T>(string name, int frameInterval, T currentValue, Action<T> setValue, int? timeMachineHistory);

        /// <summary>
        /// Synchronises a network property ONLY to clients which do not own it.  This can be used to synchronise shadow data that clients
        /// that don't own the entity should know about, such as the state of the rendered character for another player in the 
        /// world.  It can also be used to synchronise confirmation that the client does NOT own the entity using a call such as:
        /// <code>
        /// SynchroniseToNonOwner("owned", 60, false, x => _localOwned = x, null);
        /// </code>
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="name">The name of the network property.</param>
        /// <param name="frameInterval">The interval of frames at which this property will be synchronised.  Smaller values lead to higher network traffic.</param>
        /// <param name="currentValue">The current value of the property.</param>
        /// <param name="setValue">The action which sets the property based on the value.</param>
        /// <param name="timeMachineHistory">The number of frames of history to store in a time machine, or null if no time machine should be used.</param>
        void SynchroniseToNonOwner<T>(string name, int frameInterval, T currentValue, Action<T> setValue, int? timeMachineHistory);
    }
}
