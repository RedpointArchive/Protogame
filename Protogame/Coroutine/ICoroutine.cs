using System;
using System.Threading.Tasks;

namespace Protogame
{
    /// <summary>
    /// A service which allows you to schedule coroutines within a game.  Unlike <see cref="Task.Run(Action)"/>, coroutines
    /// run on the same thread as the main game, incrementally progressing over multiple frames.  This ensures resources
    /// and other thread-dependent objects are created correctly, and that multi-threading errors do not occur.
    /// </summary>
    /// <module>Coroutine</module>
    public interface ICoroutine
    {
        Task Run(Func<Task> coroutine);

        Task<T> Run<T>(Func<Task<T>> coroutine);
    }
}