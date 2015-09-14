using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protogame
{
    public interface IRequireComponentInDescendants<T>
    {
        T Component { get; }
    }
}
