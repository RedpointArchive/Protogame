using System;
using System.Text;

namespace Protogame
{
    public interface IConsoleInput
    {
        void Update(IGameContext gameContext, IUpdateContext updateContext, Action<string> logInternal);

        StringBuilder InputBuffer { get; }
    }
}
