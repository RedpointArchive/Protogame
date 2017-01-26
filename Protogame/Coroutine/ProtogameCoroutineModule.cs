using System;
using Protoinject;

namespace Protogame
{
    [Obsolete("Coroutines are now included in the core module, and this module no longer creates any bindings.  It can be safely removed.")]
    public class ProtogameCoroutineModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
        }
    }
}
