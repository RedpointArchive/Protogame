using System;
using System.Collections.Generic;

namespace Protogame
{
    public interface IQueryableComponent
    {
        HashSet<Type> EnabledInterfaces { get; }
    }
}
